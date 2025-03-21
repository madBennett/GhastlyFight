using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Assertions;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerBehavior : NetworkBehaviour
{
    //Static Varibles
    public static int numPlayers = 0; //total number of alive players 

    //Player Identification
    public ulong PlayerId = 0;
    [SerializeField] private TMP_Text healthBarText; //text box to disaplayer player id

    //Movement
    [SerializeField] private float normSpeed = 5f;
    [SerializeField] private float dashSpeed = 10f;
    private float currSpeed;
    private Vector2 movement;

    [SerializeField] private GameObject PlayerObj;//Object containing the sprite and lauch offset
    [SerializeField] private Rigidbody2D rigidBody;

    //Movement Dash Varibles
    private float lastDashTime;
    [SerializeField] private float dashCooldDown = .15f;
    [SerializeField] private float dashTime = 0.1f;
    private bool isDashing = false;
    public GameObject dashIndicator;//small triagnle to indicate dashing to this player

    //for damage
    private bool isVulenerable = true; //varible to prevent damage to player

    //health
    [SerializeField] private NetworkVariable<float> curHealth = new NetworkVariable<float>(0);
    public float maxHealth = 100f;
    public ValueBar healthBar;
    private bool isDead = false;

    //attacking
    public ProjectialBehavoir projectial; //prefab for player projectial
    public Transform launchOffset; //area where projectials are spwaned
    [SerializeField] private int damageAmount = 5;
    private float lastAttackTime;
    [SerializeField] private float attackCooldDown = 0.5f;

    //Audio
    [SerializeField] private AudioClip HurtClip;
    [SerializeField] private AudioClip AttackClip;
    [SerializeField] private AudioClip HealClip;
    [SerializeField] private AudioClip DashClip;

    public AudioSource audioSource;
    public float volume = 1f;


    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        //Prevent despawn on host being transfered into a different scene if they win
        DontDestroyOnLoad(this.gameObject);

        //itteration of player count
        numPlayers += 1;

        //set player identifcation for ease of play
        PlayerId = OwnerClientId;
        healthBarText.text = "Player: " + (PlayerId + 1);

        //set speed and dash time
        currSpeed = normSpeed;
        lastDashTime = Time.time;

        //set health
        curHealth.Value = maxHealth;
        healthBar.setMaxValue(maxHealth);
        curHealth.OnValueChanged += HealthChanged;//subscribe to health change on network varible

        //verify that the dash indicator is not showing
        dashIndicator.SetActive(false);
    }

    private void HealthChanged(float previousValue, float newValue)
    {
        //visually display health
        healthBar.setValue(newValue);

        //trigger death if players health is too low
        if (newValue <= 0)
        {
            //verify this function is only called once
            if (!isDead)
            {
                Death();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Do nothing if this is the incorrect client object
        if (!IsOwner)
            return;

        //only allow interaction if the game is in play or it is in the lobby
        if ((GameManager.gameState != GameStates.WAITING) || (GameManager.gameState != GameStates.GAME_OVER))
        {
            //While the player is alive allow to interact with the scene
            if (curHealth.Value > 0)
            {
                //move the player based on user input
                movement.x = Input.GetAxisRaw("Horizontal");
                movement.y = Input.GetAxisRaw("Vertical");

                //dash
                if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    if ((Time.time - lastDashTime) >= dashCooldDown)
                    {
                        DashServerRPC(dashSpeed);
                        isDashing = true;
                        isVulenerable = false;///prevent damage to player while dashing
                        lastDashTime = Time.time;//reset for proper cooldown
                        dashIndicator.SetActive(true);
                    }
                }

                if (isDashing)
                {
                    if ((Time.time - lastDashTime) >= dashTime)
                    {
                        isDashing = false;
                        isVulenerable = true;
                        endDashServerRPC(normSpeed);
                        dashIndicator.SetActive(false);
                    }
                }

                //move the player according to inputs on the server
                MoveServerRPC(movement, currSpeed);

                //attack
                if ((Input.GetMouseButtonDown(0)) && (!isDashing) && ((Time.time - lastAttackTime) >= attackCooldDown))
                {
                    AttackServerRPC(launchOffset.position, launchOffset.rotation, damageAmount);
                    lastAttackTime = Time.time;
                }

                //cheat mode (For your ease Prof)
                if (Input.GetKeyDown(KeyCode.K))
                {
                    isVulenerable = false;
                }
            }
        }
    }

    [ServerRpc]
    public void MoveServerRPC(Vector2 movement, float curSpeed)
    {
        //get componates for movement
        rigidBody = GetComponent<Rigidbody2D>();
        //prevent odd input
        movement.Normalize();

        //move through rigid body
        rigidBody.velocity = movement * currSpeed;

        //rotate player accordingly
        if (movement.x != 0)
        {
            //rotate left or right
            PlayerObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, movement.x * -90));
        }

        if (movement.y == 1)
        {
            //rotate up
            PlayerObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
        else if (movement.y == -1)
        {
            //rotate down
            PlayerObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
        }
    }

    [ServerRpc]
    public void DashServerRPC(float dashSpeed)
    {
        //increase speed for dash
        currSpeed = dashSpeed;
        PlayAudioClientRPC(AudioType.DASH, volume);
    }

    [ServerRpc]
    public void endDashServerRPC(float normSpeed)
    {
        //reset on cooldown completion
        currSpeed = normSpeed;
    }

    [ServerRpc]
    public void AttackServerRPC(Vector3 pos,Quaternion rot, int damageAmount)
    {
        //spawn projectial
        ProjectialBehavoir playerProjectial = Instantiate(projectial, pos, rot);
        //give the player id to prevent being hit by a bullet spawned by self
        playerProjectial.GetComponent<NetworkObject>().SpawnWithOwnership(PlayerId);

        //set projectials varibles
        playerProjectial.damageAmount = damageAmount;
        playerProjectial.ownerId = PlayerId;

        //play sound
        PlayAudioClientRPC(AudioType.ATTACK, volume);
    }

    public void applyDamage(int value) 
    {
        //check if the player and be damaged and do so as nessary if it is not in the lobby
        if (isVulenerable && !(GameManager.gameState ==  GameStates.LOBBY))
        {
            curHealth.Value -= value;
            //play sound
            PlayAudioClientRPC(AudioType.HURT, volume);
        }
    }

    public void applyHeal(int value)
    {
        //Heal the player on interaction with a health pack
        if (curHealth.Value < maxHealth)
        {
            curHealth.Value += value;
            //play sound
            PlayAudioClientRPC(AudioType.HEAL, volume);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //allows server control only
        if (!IsServer)
        {
            return;
        }

        //if it is a collision with a projectial verify it is not spawned by this player
        if (collision.gameObject.GetComponent<ProjectialBehavoir>() && !(collision.gameObject.GetComponent<NetworkBehaviour>().OwnerClientId == PlayerId))
        {
            //get the damage amount by the projectial and apply it to the player health
            ProjectialBehavoir projectial = collision.gameObject.GetComponent<ProjectialBehavoir>();
            applyDamage(projectial.damageAmount);
        }
    }
    public void Death()
    {
        //verify the object is spawned
        if (NetworkObject.IsSpawned == false)
        {
            return;
        }

        //reset player count
        numPlayers -= 1;

        //show game over indicator
        healthBarText.text = "Player " + (PlayerId + 1) + ": DEAD";

        isDead = true;
    }

    [ClientRpc]
    private void PlayAudioClientRPC(AudioType audioType, float volume)
    {
        //play audio based on the type on the client side
        switch (audioType)
        {
            case AudioType.ATTACK:
                audioSource.PlayOneShot(AttackClip, volume);
                break;
            case AudioType.DASH:
                audioSource.PlayOneShot(DashClip, volume);
                break;
            case AudioType.HEAL:
                audioSource.PlayOneShot(HealClip, volume);
                break;
            case AudioType.HURT:
                audioSource.PlayOneShot(HurtClip, volume);
                break;
        }
    }
}
