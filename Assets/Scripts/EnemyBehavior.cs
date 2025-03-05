using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    //health
    public float curHealth;
    public float maxHealth = 1000f;
    public ValueBar healthBar;
    
    public float healthThreshold = 0.5f;

    //attacking
    public ProjectialBehavoir projectial;
    public Transform launchOffset;
    [SerializeField] private float damageAmount = 10f;

    //For Movement
    [SerializeField] private List<GameObject> balconies = new List<GameObject>();
    [SerializeField] private List<GameObject> boxes = new List<GameObject>();

    //
    [SerializeField] private List<Transform> constantLaunch = new List<Transform>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Attack()
    {
        //
        ProjectialBehavoir enemyProjectial = Instantiate(projectial, launchOffset.position, launchOffset.rotation);
        int damageUpAmount = 1;

        //on half health increase damage and speed of projectials
        if (curHealth <= healthThreshold * maxHealth)
        {
            damageUpAmount = 2;
        }

        enemyProjectial.damageAmount = damageAmount * damageUpAmount;
        enemyProjectial.speed *= damageUpAmount;
    }

    public void Move()
    {
        //Moves the Enemy to a Random Location
    }
}
