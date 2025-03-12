using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using UnityEngine.Assertions;

public class EnemyBehavior : NetworkBehaviour
{
    //health
    [SerializeField] private NetworkVariable<float> curHealth = new NetworkVariable<float>(0);
    public float maxHealth = 1000f;
    public ValueBar healthBar;
    
    public float healthThreshold = 500f;

    //attacking
    public ProjectialBehavoir projectial;
    [SerializeField] private int damageAmount = 10;
    private float lastAttackTime;
    [SerializeField] private float attackCoolDown = 0.25f;

    //For Movement
    [SerializeField] private List<AreaBehaviour> balconies = new List<AreaBehaviour>();
    [SerializeField] private List<AreaBehaviour> boxes = new List<AreaBehaviour>();

    [SerializeField] private AreaBehaviour curLocation;
    [SerializeField] private NetworkVariable<int> randIndex = new NetworkVariable<int>(0);
    private float lastMoveTime;
    [SerializeField] private float moveCoolDown = 5f;
    private float coolDownReduction = 1f;
    [SerializeField] private float maxRedution = 0.3f;

    [SerializeField] private TMP_Text healthBarText;

    //Audio
    [SerializeField] private AudioClip HurtClip;
    [SerializeField] private AudioClip AttackClip;
    [SerializeField] private AudioClip DeathClip;
    [SerializeField] private AudioClip MoveClip;

    // Start is called before the first frame update
    void Start()
    {
        //set health
        curHealth.Value = maxHealth;
        healthBar.setMaxValue(maxHealth);
        curHealth.OnValueChanged += HealthChanged;

        //set times for cooldown
        lastAttackTime = Time.time;
        lastMoveTime = Time.time;

        Move();
    }
    
    private void HealthChanged(float previousValue, float newValue)
    {
        healthBar.setValue(newValue);
    }

    // Update is called once per frame
    void Update()
    {
        if (curHealth.Value > 0)
        {
            coolDownReduction = ((curHealth.Value / maxHealth) > maxRedution) ? (curHealth.Value / maxHealth) : maxRedution;
            if ((Time.time - lastMoveTime) >= moveCoolDown * coolDownReduction)
            {
                Move();
                lastMoveTime = Time.time;
            }
            if ((Time.time - lastAttackTime) >= attackCoolDown * coolDownReduction)
            {
                Attack();
                lastAttackTime = Time.time;
            }
        }
        else
        {
            Death();
        }
        Debug.Log("Testing console output");
    }

    public void Attack()
    {
        //
        int damageUpAmount = 1;

        //on half health increase damage and speed of projectials
        if (curHealth.Value <= healthThreshold)
        {
            damageUpAmount = 2;
        }

        foreach (Transform launch in curLocation.launchLocs)
        {
            LaunchProjectileServerRPC(launch.position, launch.rotation, damageAmount, damageUpAmount);
        }
    }

    [ServerRpc]
    private void LaunchProjectileServerRPC(Vector3 pos, Quaternion rot, int damageAmount, int damageUpAmount)
    {
        ProjectialBehavoir enemyProjectial = Instantiate(projectial, pos, rot);

        enemyProjectial.GetComponent<NetworkObject>().SpawnWithOwnership(0);

        enemyProjectial.damageAmount = damageAmount * damageUpAmount;
        enemyProjectial.speed *= damageUpAmount;
        enemyProjectial.ownerId = 0;

    }

    public void Move()
    {
        //remove marker that enemy is at previous location
        curLocation.isEnemy = false;
        curLocation.glowObj.SetActive(false);

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

        SetNewPosServerRPC(curLocation.enemyLoc.position, curLocation.enemyLoc.rotation);

        //set new loc marker
        curLocation.isEnemy = true;
        curLocation.glowObj.SetActive(true);
        //play sound  on move
    }

    [ServerRpc]
    private void SetNewPosServerRPC(Vector3 pos, Quaternion rot)
    {
        //set new location
        transform.position = pos;
        transform.rotation = rot;
    }

    public void applyDamage(int value)
    {
        //
        curHealth.Value -= value;
    }

    public void Death()
    {
        //
        //play death animation and sound
        healthBarText.text = "ONLY ONE SURVIVES";

        if (NetworkObject.IsSpawned == false)
        {
            return;
        }
        Assert.IsTrue(NetworkManager.IsServer);

        NetworkObject.Despawn(true);
    }
}
