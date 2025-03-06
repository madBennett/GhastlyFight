using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject enemy;
    public Transform enemyLoc;
    public List<Transform> launchLocs = new List<Transform>();

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

    public void applyDamage(float value)
    {
        //
        if (isEnemy)
        {
            enemy.SendMessage("applyDamage", value);
        }

    }
}
