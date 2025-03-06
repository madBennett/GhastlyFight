using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaBehaviour : MonoBehaviour
{
    public Collider2D col;
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //
        if (collision.gameObject.tag == "Projectial")
        {
            //

        }
    }
}
