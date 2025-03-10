using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
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
        if ((Time.time - lastHealSpawnTime) >= healSpawnCooldDown)
        {
            randLoc.x = Random.Range(xRange.x, xRange.y);
            randLoc.y = Random.Range(yRange.x, yRange.y);
            SpawnHealServerRPC(randLoc, transform.rotation);
            lastHealSpawnTime = Time.time;
        }
    }

    [ServerRpc]
    public void SpawnHealServerRPC(Vector3 pos, Quaternion rot)
    {
        //
        GameObject spawnedHealPack = Instantiate(healPack, pos, rot);
        spawnedHealPack.GetComponent<NetworkObject>().Spawn(true);
    }

}
