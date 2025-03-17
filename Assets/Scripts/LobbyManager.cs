using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using System.Threading.Tasks;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;

public class LobbyManager : MonoBehaviour
{
    //instance to be called in other objects
    public static LobbyManager LobbyManagerInstance;
    private Lobby joinedLobby;

    //timer to keep lobby alive
    private float heartbeatTimer = 0f;

    //max players per lobby
    public static int maxPlayers = 4;

    //key for code retrevial
    private const string KEY_RELAY_JOIN_CODE = "RelayJoinCode"; 

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        //use unity to authenticate users
        InitializeUnityAuthentication();

        //set instance
        LobbyManagerInstance = this;
    }

    private async void InitializeUnityAuthentication()
    {
        //verify is able to authenticate
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            //set a new authenication with random log in
            InitializationOptions intOptions = new InitializationOptions();
            intOptions.SetProfile(Random.Range(0, 100000).ToString());
            await UnityServices.InitializeAsync(intOptions);

            //user signin
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    private async Task<Allocation> AllocateRelay()
    {
        try
        {
            //create a new relay service instance
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers - 1);
            return allocation;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);

            return default;
        }
    }

    private async Task<string> getRelayJoinCode(Allocation allocation)
    {
        try
        {
            //get the join code for the lobby
            string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            return relayJoinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);

            return default;
        }
    }

    private async Task<JoinAllocation> JoinRelay(string joinCode)
    {
        try
        {
            //get the allaction from the relay service
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            return joinAllocation;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
            return default;
        }
    }

    public async void CreateLobby(string lobbyName, bool isPrivate)
    {
        try
        {
            //set joined lobby
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, new CreateLobbyOptions { IsPrivate = isPrivate, });

            //get resources for relay services
            Allocation allocation = await AllocateRelay();
            string relayJoinCode = await getRelayJoinCode(allocation);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));
            await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, 
                new UpdateLobbyOptions 
                {
                Data = new Dictionary<string, 
                    DataObject>
                        {
                             { KEY_RELAY_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) } 
                        } 
                });

            //start user as host and load main game scene
            NetworkManager.Singleton.StartHost();
            SceneManager.LoadScene(GameManager.lobbySceneID);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void QuickJoin()
    {
        try
        {
            //get joined lobby
            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

            //join relay
            string joinRelayCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await JoinRelay(joinRelayCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            //start user as client
            NetworkManager.Singleton.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void JoinLobbyWIthCode(string lobbyCode)
    {
        try
        {
            //find lobby by code
            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);

            // join relay
            string joinRelayCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await JoinRelay(joinRelayCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            //start user as client
            NetworkManager.Singleton.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public Lobby getJoinedLobby()
    {
        //return lobby
        return joinedLobby;
    }

    private void Update()
    {
        HandleHeartbeat();
    }

    public void HandleHeartbeat()
    {
        //keep lobby alive before timeout
        if (IsLobbyHost())
        {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer <= 0f)
            {
                float heartbeatTimerMax = 15f;
                heartbeatTimer = heartbeatTimerMax;

                LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            }
        }
    }

    public bool IsLobbyHost()
    {
        //return if user is lobby host
        return (joinedLobby != null) && (joinedLobby.HostId == AuthenticationService.Instance.PlayerId);
    }

    public async void LeaveLobby()
    {
        //remove player from lobby
        if (joinedLobby != null)
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
                joinedLobby = null;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    public void DeleteLobby()
    {
        //delete the lobby when it is no longer in use
        if (joinedLobby != null)
        {
            try
            {
                LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }
}
