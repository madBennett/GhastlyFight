using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    private Vector2 movement;

    public Rigidbody2D rigidBody;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //move the player based on user input
        movement.Set(InputManager.movement.x, InputManager.movement.y);
        rigidBody.velocity = movement * speed;
    }
}
