using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<PlayerBehavior> Players = new List<PlayerBehavior>();
    [SerializeField] private EnemyBehavior Enemy;

    [SerializeField] private float healSpawnCooldDown = 5f;
    [SerializeField] private float lastHealSpawnTime;
    public GameObject healPack;
    private Vector3 randLoc = new Vector3(0, 0, 1);
    private Vector2 xRange = new Vector2(-7, 7);
    private Vector2 yRange = new Vector2(-3, 3);

    // Start is called before the first frame update
    void Start()
    {
        lastHealSpawnTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (PlayerBehavior Player in Players)
        {
            if (Player.curHealth <= 0)
            {
                Player.Death();
            }
        }

        if (Enemy.curHealth <= 0)
        {
            Enemy.Death();
        }

        if ((Time.time - lastHealSpawnTime) >= healSpawnCooldDown)
        {
            randLoc.x = Random.Range(xRange.x, xRange.y);
            randLoc.y = Random.Range(yRange.x, yRange.y);
            Instantiate(healPack, randLoc, transform.rotation);
            lastHealSpawnTime = Time.time;
        }
    }
}
