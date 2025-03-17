using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Assertions;

public class ProjectialBehavoir : NetworkBehaviour
{
    //movement
    public float speed = 5f;
    [SerializeField] private Rigidbody2D rigidBody;

    //set damage for varible
    public int damageAmount = 5;

    //set varible for object6 pool
    public static int numProjectials = 0;

    //id for owner of projectial
    public ulong ownerId = 0;

    public override void OnNetworkSpawn()
    {
        //add to total
        numProjectials += 1;
        //get rigidbody
        rigidBody = GetComponent<Rigidbody2D>();
        //set movement
        rigidBody.velocity = this.transform.up * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Only take action if it is the server and the object is spawned
        if ((!IsServer) || (NetworkObject.IsSpawned == false))
        {
            return;
        }

        //only distroy the object if it hits something other than the owner
        if (collision.gameObject.GetComponent<PlayerBehavior>())
        {
            if (collision.gameObject.GetComponent<PlayerBehavior>().PlayerId == ownerId)
            {
                return;
            }
        }

        //remove object
        RemoveObject();
    }

    private void RemoveObject()
    {
        //only take action if it is spawned
        if ((!IsServer) || (!NetworkObject.IsSpawned))
        {
            return;
        }

        //subtract from total
        numProjectials -= 1;

        //despawn and destroy object
        NetworkObject.Despawn(true);
        Destroy(gameObject);
    }
}
