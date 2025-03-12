using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Assertions;
using TMPro;

public class PlayerBehavior : NetworkBehaviour
{
    public static int numPlayers = 0;

    public ulong PlayerId = 0;
    [SerializeField] private TMP_Text healthBarText;

    //Movement
    [SerializeField] private float normSpeed = 5f;
    [SerializeField] private float dashSpeed = 10f;
    private float currSpeed;
    [SerializeField] private GameObject PlayerObj;

    [SerializeField] private Rigidbody2D rigidBody;

    private Vector2 movement;

    private float lastDashTime;
    [SerializeField] private float dashCooldDown = .15f;
    [SerializeField] private float dashTime = 0.1f;

    private bool isDashing = false;

    //for damage
    private bool isVulenerable = true;

    //health
    [SerializeField] private NetworkVariable<float> curHealth = new NetworkVariable<float>(0);
    public float maxHealth = 100f;
    public ValueBar healthBar;

    //attacking
    public ProjectialBehavoir projectial;
    public Transform launchOffset;
    [SerializeField] private int damageAmount = 5;
    private float lastAttackTime;
    [SerializeField] private float attackCooldDown = 0.5f;

    //Audio
    [SerializeField] private AudioClip HurtClip;
    [SerializeField] private AudioClip AttackClip;
    [SerializeField] private AudioClip HealClip;


    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        numPlayers += 1;

        PlayerId = OwnerClientId + 1;

        //set speed and dash time
        currSpeed = normSpeed;
        lastDashTime = Time.time;

        //set health
        curHealth.Value = maxHealth;
        healthBar.setMaxValue(maxHealth);
        curHealth.OnValueChanged += HealthChanged;
    }

    private void HealthChanged(float previousValue, float newValue)
    {
        healthBar.setValue(newValue);
    }

    // Update is called once per frame
    void Update()
    {
        //Do nothing if this is the incorrect client object
        if (!IsOwner)
            return;

        if (curHealth.Value > 0)
        {
            //move the player based on user input
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");

            //dash
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                if ((Time.time - lastDashTime) >= dashCooldDown)
                {
                    isDashing = true;
                    currSpeed = dashSpeed;
                    lastDashTime = Time.time;
                    isVulenerable = false;
                }
            }

            if (isDashing)
            {
                if ((Time.time - lastDashTime) >= dashTime)
                {
                    isDashing = false;
                    currSpeed = normSpeed;
                    isVulenerable = true;
                }
            }

            MoveServerRPC(movement, currSpeed);

            if ((Input.GetMouseButtonDown(0)) && (!isDashing) && ((Time.time - lastAttackTime) >= attackCooldDown))
                {
                AttackServerRPC(launchOffset.position, launchOffset.rotation, damageAmount);
                lastAttackTime = Time.time;
            }

            //cheat mode
            if (Input.GetKeyDown(KeyCode.K))
            {
                isVulenerable = false;
            }
        }
        else
        {
            Death();
        }
    }

    [ServerRpc]
    public void MoveServerRPC(Vector2 movement, float curSpeed)
    {
        //get componates
        rigidBody = GetComponent<Rigidbody2D>();
        movement.Normalize();

        rigidBody.velocity = movement * currSpeed;

        //rotate player accordingly
        if (movement.x != 0)
        {
            PlayerObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, movement.x * -90));
        }

        if (movement.y == 1)
        {
            PlayerObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
        else if (movement.y == -1)
        {
            PlayerObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
        }
    }

    [ServerRpc]
    public void AttackServerRPC(Vector3 pos,Quaternion rot, int damageAmount)
    {
        //
        ProjectialBehavoir playerProjectial = Instantiate(projectial, pos, rot);
        playerProjectial.GetComponent<NetworkObject>().SpawnWithOwnership(PlayerId);
        playerProjectial.damageAmount = damageAmount;
        playerProjectial.ownerId = PlayerId;
    }

    private void changeHealth(int value)
    {
        curHealth.Value += value;
        
    }

    public void applyDamage(int value) 
    {
        //
        if (isVulenerable)
        {
            changeHealth(-1 * value);
        }
    }

    public void applyHeal(int value)
    {
        //
        if (curHealth.Value < maxHealth)
        {
            changeHealth(value);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //

        if (!IsServer)
        {
            return;
        }

        if (collision.gameObject.GetComponent<ProjectialBehavoir>() && !(collision.gameObject.GetComponent<NetworkBehaviour>().OwnerClientId == PlayerId))
        {
            ProjectialBehavoir projectial = collision.gameObject.GetComponent<ProjectialBehavoir>();
            applyDamage(projectial.damageAmount);
        }
    }
    public void Death()
    {
        //
        //play death animation and sound
        //show game over
        if (NetworkObject.IsSpawned == false)
        {
            return;
        }
        Assert.IsTrue(NetworkManager.IsServer);

        numPlayers -= 1;

        NetworkObject.Despawn(true);
        Destroy(gameObject);
    }
}
