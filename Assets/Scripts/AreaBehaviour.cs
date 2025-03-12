using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class AreaBehaviour : NetworkBehaviour
{
    [SerializeField] private GameObject enemy;
    public Transform enemyLoc;
    public List<Transform> launchLocs = new List<Transform>();
    public GameObject glowObj;

    public bool isEnemy = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isEnemy)
        {
            foreach (Transform launch in launchLocs)
            {
                launch.rotation = Quaternion.Euler(0, 0, enemyLoc.rotation.eulerAngles.z + 45 * Mathf.Sin(Time.time));
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //

        if (!IsServer)
        {
            return;
        }

        if (collision.gameObject.GetComponent<ProjectialBehavoir>() && !(collision.gameObject.GetComponent<NetworkBehaviour>().OwnerClientId == enemy.GetComponent<EnemyBehavior>().id))
        {
            ProjectialBehavoir projectial = collision.gameObject.GetComponent<ProjectialBehavoir>();
            applyDamage(projectial.damageAmount);
        }
    }

    public void applyDamage(int value)
    {
        //
        if (isEnemy)
        {
            enemy.SendMessage("applyDamage", value);
        }

    }
}
