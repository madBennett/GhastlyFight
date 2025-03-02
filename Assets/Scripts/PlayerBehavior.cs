using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehavior : MonoBehaviour
{
    //Movement
    private InputAction moveAction;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private float speed = 5f;
    private Vector2 movement;

    public Rigidbody2D rigidBody;

    private void Awake()
    {
        moveAction = playerInput.actions["Move"];
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //move the player based on user input
        movement = moveAction.ReadValue<Vector2>();
        //movement.Set(InputManager.movement.x, InputManager.movement.y);
        rigidBody.velocity = movement * speed;
    }
}
