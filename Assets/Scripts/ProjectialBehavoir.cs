using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectialBehavoir : MonoBehaviour
{
    public float speed = 5f;
    public float damageAmount = 5f;
    // Update is called once per frame
    void Update()
    {
        transform.position += transform.up * Time.deltaTime * speed;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        //
        if ((collision.gameObject.tag == "Player") || (collision.gameObject.tag == "Area"))
        {
            collision.gameObject.SendMessage("applyDamage", damageAmount);
        }
        Destroy(gameObject);
    }
}
