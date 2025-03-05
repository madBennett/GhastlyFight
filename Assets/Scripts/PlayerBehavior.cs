using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    //Movement
    [SerializeField] private float normSpeed = 5f;
    [SerializeField] private float dashSpeed = 10f;
    private float currSpeed;

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
        //move the player based on user input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement.Normalize();

        rigidBody.velocity = movement * currSpeed;

        
        //rotate player accordingly
        if (movement.x != 0)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, movement.x * -90));
        }

        if (movement.y == 1)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
        else if (movement.y == -1)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Attack();
        }
    }

    public void Attack()
    {
        //
        ProjectialBehavoir playerProjectial = Instantiate(projectial, launchOffset.position, launchOffset.rotation);
        playerProjectial.damageAmount = damageAmount;
    }

    public void applyDamage(float value) 
    {
        //
        if (isVulenerable)
        {
            curHealth -= value;
            healthBar.setValue(curHealth);
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //
        if (collision.gameObject.tag == "Projectial")
        {
            //
            
        }
    }

}
