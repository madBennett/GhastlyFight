using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Assertions;

public class ProjectialBehavoir : NetworkBehaviour
{
    public float speed = 5f;
    public float damageAmount = 5f;

    public static int numProjectials = 0;

    [SerializeField] private Rigidbody2D rigidBody;

    public override void OnNetworkSpawn()
    {
        numProjectials += 1;
        rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.velocity = this.transform.up * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //

        if (!NetworkObject.IsSpawned)
        {
            return;
        }

        numProjectials -= 1;
        NetworkObject.Despawn(true);
        Destroy(gameObject);
    }
}
