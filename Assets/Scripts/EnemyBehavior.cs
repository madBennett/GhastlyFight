using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyBehavior : MonoBehaviour
{
    //health
    public float curHealth;
    public float maxHealth = 1000f;
    public ValueBar healthBar;
    
    public float healthThreshold = 500f;

    //attacking
    public ProjectialBehavoir projectial;
    [SerializeField] private float damageAmount = 10f;
    private float lastAttackTime;
    [SerializeField] private float attackCoolDown = 0.25f;

    //For Movement
    [SerializeField] private List<AreaBehaviour> balconies = new List<AreaBehaviour>();
    [SerializeField] private List<AreaBehaviour> boxes = new List<AreaBehaviour>();

    [SerializeField] private AreaBehaviour curLocation;
    private float lastMoveTime;
    [SerializeField] private float moveCoolDown = 5f;
    private float coolDownReduction = 1f;
    [SerializeField] private float maxRedution = 0.3f;

    [SerializeField] private TMP_Text healthBarText;

    // Start is called before the first frame update
    void Start()
    {
        //set health
        curHealth = maxHealth;
        healthBar.setMaxValue(maxHealth);

        //set times for cooldown
        lastAttackTime = Time.time;
        lastMoveTime = Time.time;

        Move();
    }

    // Update is called once per frame
    void Update()
    {
        if (curHealth > 0)
        {
            coolDownReduction = ((curHealth / maxHealth) > maxRedution) ? (curHealth / maxHealth) : maxRedution;
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
    }

    public void Attack()
    {
        //
        int damageUpAmount = 1;

        //on half health increase damage and speed of projectials
        if (curHealth <= healthThreshold)
        {
            damageUpAmount = 2;
        }

        foreach (Transform launch in curLocation.launchLocs)
        {
            LaunchProjectile(launch, damageUpAmount);
        }
    }

    private void LaunchProjectile(Transform launchOffset, int damageUpAmount)
    {
        ProjectialBehavoir enemyProjectial = Instantiate(projectial, launchOffset.position, launchOffset.rotation);

        enemyProjectial.damageAmount = damageAmount * damageUpAmount;
        enemyProjectial.speed *= damageUpAmount;
    }

    public void Move()
    {
        //remove marker that enemy is at previous location
        curLocation.isEnemy = false;

        //Moves the Enemy to a Random Location
        if (curHealth > healthThreshold)
        {
            //move to the normal loc
            int randIndex = Random.Range(0, balconies.Capacity);
            curLocation = balconies[randIndex];
        }
        else
        {
            //move to the specail loc
            int randIndex = Random.Range(0, boxes.Capacity);
            curLocation = boxes[randIndex];
        }

        //set new location
        transform.position = curLocation.enemyLoc.position;
        transform.rotation = curLocation.enemyLoc.rotation;

        //set new loc marker
        curLocation.isEnemy = true;
        //play sound  on move
    }

    public void applyDamage(int value)
    {
        //
        curHealth -= value;
        healthBar.setValue(curHealth);
    }

    public void Death()
    {
        //
        //play death animation and sound
        healthBarText.text = "ONLY ONE SURVIVES";

        gameObject.GetComponent<Renderer>().enabled = false;
    }
}
