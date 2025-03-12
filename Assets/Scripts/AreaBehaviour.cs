using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class AreaBehaviour : NetworkBehaviour
{
    //reference to enemy object
    [SerializeField] private GameObject enemy;
    //location enemy will be located
    public Transform enemyLoc;
    //list of lauch locations for projectials
    public List<Transform> launchLocs = new List<Transform>();
    //indicator for location - REMOVE???
    public GameObject glowObj;
    //varible to denote if enemy is present at that location
    public NetworkVariable<bool> isEnemy = new NetworkVariable<bool>(false);

    // Start is called before the first frame update
    void Start()
    {
        isEnemy.OnValueChanged += IsEnemyChanged;//subscribe to health change on network varible
    }

    // Update is called once per frame
    void Update()
    {
        if (isEnemy.Value)
        {
            //only rotate if the enemy is at this location
            foreach (Transform launch in launchLocs)
            {
                //rotate each lauch pos from a max of 45 to -45 degrees from orignonal position
                launch.rotation = Quaternion.Euler(0, 0, enemyLoc.rotation.eulerAngles.z + 45 * Mathf.Sin(Time.time));
            }
        }
    }

    private void IsEnemyChanged(bool previousValue, bool newValue)
    {
        //reset rotation of launch positions
        foreach (Transform launch in launchLocs)
        {
            launch.rotation = Quaternion.Euler(0, 0, enemyLoc.rotation.eulerAngles.z);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //allows server control only
        if (!IsServer)
        {
            return;
        }

        //verifys that the enemy is at this location
        if (isEnemy.Value)
        {
            //checks that the object is a projectial and is not from the enemy
            if (collision.gameObject.GetComponent<ProjectialBehavoir>() && !(collision.gameObject.GetComponent<NetworkBehaviour>().OwnerClientId == enemy.GetComponent<EnemyBehavior>().id))
            {
                ProjectialBehavoir projectial = collision.gameObject.GetComponent<ProjectialBehavoir>();
                enemy.SendMessage("applyDamage", projectial.damageAmount);
            }
        }
    }
}
