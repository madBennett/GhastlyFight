using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class HealPackBehavior : NetworkBehaviour
{
    //set default varibles
    [SerializeField] private float healAmount = 10f;
    [SerializeField] private float despawnCooldDown = 2f;
    [SerializeField] private float spawnTime;

    // Start is called before the first frame update
    void Start()
    {
        //set cooldown time
        spawnTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        //only take action if it is spawned
        if (!NetworkObject.IsSpawned)
        {
            return;
        }

        if ((Time.time - spawnTime) >= despawnCooldDown)
        {
            //destroy on cooldown up
            RemoveObject();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //allows server control only
        if (!NetworkObject.IsSpawned)
        {
            return;
        }

        //on collsion with the player heal them and despawn
        if ((collision.gameObject.tag == "Player"))
        {
            collision.gameObject.SendMessage("applyHeal", healAmount);
            RemoveObject();
        }
    }

    private void RemoveObject()
    {
        //only take action if it is spawned
        if ((!IsServer) || (!NetworkObject.IsSpawned))
        {
            return;
        }


        NetworkObject.Despawn(true);
        Destroy(gameObject);
    }
}
