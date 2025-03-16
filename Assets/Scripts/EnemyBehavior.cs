using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using UnityEngine.Assertions;

public class EnemyBehavior : NetworkBehaviour
{
    //ID for projectial spawning - very high number to prevent overlap with player ids
    public ulong id = 999999;
    [SerializeField] private TMP_Text healthBarText;// for id and a fun message >:)

    //health
    [SerializeField] private NetworkVariable<float> curHealth = new NetworkVariable<float>(0);
    public float maxHealth = 1000f;
    public ValueBar healthBar;
    
    public float healthThreshold = 500f;//threshold for phase 2

    //attacking
    public ProjectialBehavoir projectial;
    [SerializeField] private int damageAmount = 10;
    private float lastAttackTime;
    [SerializeField] private float attackCoolDown = 0.25f;

    //For Movement

    //list of areas that can be moved to
    [SerializeField] private List<AreaBehaviour> balconies = new List<AreaBehaviour>();
    [SerializeField] private List<AreaBehaviour> boxes = new List<AreaBehaviour>();
    [SerializeField] private AreaBehaviour curLocation;

    [SerializeField] private NetworkVariable<int> randIndex = new NetworkVariable<int>(0);//random index of area within list to travel to

    private float lastMoveTime;
    [SerializeField] private float moveCoolDown = 5f;
    private float coolDownReduction = 1f;

    //varible for a max reduction on cooldowns for playablity
    [SerializeField] private float maxRedution = 0.3f;

    //Audio
    [SerializeField] private AudioClip HurtClip;
    [SerializeField] private AudioClip AttackClip;
    [SerializeField] private AudioClip DeathClip;
    [SerializeField] private AudioClip MoveClip;

    public AudioSource audioSource;
    public float volume = 1f;

    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        Debug.Log("SPAWNED ENEMY");
        //set health
        curHealth.Value = maxHealth;
        healthBar.setMaxValue(maxHealth);
        curHealth.OnValueChanged += HealthChanged;//subscribe to health change on network varible

        //set times for cooldown
        lastAttackTime = Time.time;
        lastMoveTime = Time.time;

        Move();
    }
    
    private void HealthChanged(float previousValue, float newValue)
    {
        //visually display health
        healthBar.setValue(newValue);

        if (newValue <= healthThreshold)
        {
            //shift phase
            GameManager.gameState = GameStates.GAME_PHASE2;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.gameState == GameStates.GAME_PHASE1 
            || GameManager.gameState == GameStates.GAME_PHASE2)
        {
            //Calc a reduction on all cooldowns for an extra challenge based on the current health, not exceeding the threshold
            coolDownReduction = ((curHealth.Value / maxHealth) > maxRedution) ? (curHealth.Value / maxHealth) : maxRedution;

            if ((Time.time - lastMoveTime) >= moveCoolDown * coolDownReduction)
            {
                //move when cooldown is up
                Move();
                lastMoveTime = Time.time;
            }
            if ((Time.time - lastAttackTime) >= attackCoolDown * coolDownReduction)
            {
                //attack when cooldown up
                Attack();
                lastAttackTime = Time.time;
            }
        }

        if (curHealth.Value <= 0)
        {
            //trigger death if health is too low and shift game state
            DeathServerRPC();
        }
    }

    public void Attack()
    {
        //vauble to increase damage on entering phase 2
        int damageUpAmount = 1;

        //on half health increase damage and speed of projectials
        if (GameManager.gameState == GameStates.GAME_PHASE2)
        {
            damageUpAmount = 2;
        }

        //for every projectial launch one projectial at that location
        foreach (Transform launch in curLocation.launchLocs)
        {
            LaunchProjectileServerRPC(launch.position, launch.rotation, damageAmount, damageUpAmount);
        }

        //play audio
        audioSource.PlayOneShot(AttackClip, volume);
    }

    [ServerRpc]
    private void LaunchProjectileServerRPC(Vector3 pos, Quaternion rot, int damageAmount, int damageUpAmount)
    {
        //spawn projectial
        ProjectialBehavoir enemyProjectial = Instantiate(projectial, pos, rot);

        //set the ownsership
        enemyProjectial.GetComponent<NetworkObject>().SpawnWithOwnership(id);

        //set projectials varibles
        enemyProjectial.damageAmount = damageAmount * damageUpAmount;
        enemyProjectial.speed *= damageUpAmount;
        enemyProjectial.ownerId = id;

    }

    public void Move()
    {
        //remove marker that enemy is at previous location
        curLocation.isEnemy.Value = false;

        //Moves the Enemy to a Random Location
        if (curHealth.Value > healthThreshold)
        {
            //move to the normal loc
            randIndex.Value = Random.Range(0, balconies.Capacity);
            curLocation = balconies[randIndex.Value];
        }
        else
        {
            //move to the specail loc
            randIndex.Value = Random.Range(0, boxes.Capacity);
            curLocation = boxes[randIndex.Value];
        }

        //move on server
        SetNewPosServerRPC(curLocation.enemyLoc.position, curLocation.enemyLoc.rotation);

        //set new loc marker
        curLocation.isEnemy.Value = true;
    }

    [ServerRpc]
    private void SetNewPosServerRPC(Vector3 pos, Quaternion rot)
    {

        //play sound  on move
        audioSource.PlayOneShot(MoveClip, volume);

        //set new location and rotations
        transform.position = pos;
        transform.rotation = rot;
    }

    public void applyDamage(int value)
    {
        //subtract amount from health
        curHealth.Value -= value;

        //play audio
        audioSource.PlayOneShot(HurtClip, volume*2);
    }

    [ServerRpc]
    public void DeathServerRPC()
    {
        //

        Debug.Log("DEATH");

        GameManager.gameState = GameStates.GAME_PHASE3;

        //play death animation and sound

        audioSource.PlayOneShot(DeathClip, volume);

        //change Healthbar text to new message for players
        healthBarText.text = "ONLY ONE SURVIVES";

        //verify object is spawned
        if (NetworkObject.IsSpawned == false)
        {
            return;
        }
        Assert.IsTrue(NetworkManager.IsServer);

        //notify players that the enemy is dead
        GameManager.isEnemyDead = true;

        //Change value of area to prevent unnessary changes
        curLocation.isEnemy.Value = false;

        //despawn object
        NetworkObject.Despawn(true);
    }
}
