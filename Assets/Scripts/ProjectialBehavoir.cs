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
        Debug.Log("Total Projectials: " + numProjectials);
        rigidBody.velocity = this.transform.up * speed;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position += transform.up * Time.deltaTime * speed;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        //

        if (!NetworkManager.Singleton.IsServer || !NetworkObject.IsSpawned)
        {
            return;
        }

        numProjectials -= 1;
        NetworkObject.Despawn(true);
        Destroy(gameObject);
    }
}
