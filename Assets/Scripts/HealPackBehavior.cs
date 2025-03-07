using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealPackBehavior : MonoBehaviour
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
        if ((Time.time - spawnTime) >= despawnCooldDown)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //
        if ((collision.gameObject.tag == "Player"))
        {
            collision.gameObject.SendMessage("applyHeal", healAmount);
            Destroy(gameObject);
        }
    }
}
