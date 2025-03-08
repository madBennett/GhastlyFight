using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Assertions;

public class PlayerBehavior : NetworkBehaviour
{
    public static int numPlayers = 0;

    //Movement
    [SerializeField] private float normSpeed = 5f;
    [SerializeField] private float dashSpeed = 10f;
    private float currSpeed;
    [SerializeField] private GameObject PlayerObj;

    [SerializeField] private Rigidbody2D rigidBody;

    private Vector2 movement;

    private float lastDashTime;
    [SerializeField] private float dashCooldDown = 5f;
    [SerializeField] private float dashTime = 0.1f;

    private bool isDashing = false;

    //for damage
    private bool isVulenerable = true;

    //health
    public float curHealth;
    public float maxHealth = 100f;
    public ValueBar healthBar;

    //attacking
    public ProjectialBehavoir projectial;
    public Transform launchOffset;
    [SerializeField] private float damageAmount = 5f;


    // Start is called before the first frame update
    void Start()
    {
        numPlayers += 1;

        //set speed and dash time
        currSpeed = normSpeed;
        lastDashTime = Time.time;

        //set health
        curHealth = maxHealth;
        healthBar.setMaxValue(maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        if (curHealth > 0)
        {
            //move the player based on user input
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");
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

            if (Input.GetMouseButtonDown(0) && !isDashing)
            {
                Attack();
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

    public void Attack()
    {
        //
        ProjectialBehavoir playerProjectial = Instantiate(projectial, launchOffset.position, launchOffset.rotation);
        playerProjectial.damageAmount = damageAmount;
    }

    private void changeHealth(int value)
    {
        curHealth += value;
        healthBar.setValue(curHealth);
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
        if (curHealth < maxHealth)
        {
            changeHealth(value);
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
