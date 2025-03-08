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

    void Start()
    {
        numProjectials += 1;
        Debug.Log("Total Projectials: " + numProjectials);
    }

        // Update is called once per frame
        void Update()
    {
        transform.position += transform.up * Time.deltaTime * speed;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        //

        numProjectials -= 1;
        Destroy(gameObject);
    }
}
