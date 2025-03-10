using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class HealPackBehavior : NetworkBehaviour
{
    [SerializeField] private float healAmount = 10f;
    [SerializeField] private float despawnCooldDown = 2f;
    [SerializeField] private float spawnTime;

    // Start is called before the first frame update
    void Start()
    {
        spawnTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (!NetworkManager.Singleton.IsServer || !NetworkObject.IsSpawned)
        {
            return;
        }

        if ((Time.time - spawnTime) >= despawnCooldDown)
        {
            NetworkObject.Despawn(true);
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //
        if (!NetworkManager.Singleton.IsServer || !NetworkObject.IsSpawned)
        {
            return;
        }

        if ((collision.gameObject.tag == "Player"))
        {
            collision.gameObject.SendMessage("applyHeal", healAmount);
            //NetworkObject.Despawn(true);
            Destroy(gameObject);
        }
    }
}
