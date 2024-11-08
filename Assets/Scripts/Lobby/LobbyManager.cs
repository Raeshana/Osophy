using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance { get; private set; }
    public bool IsLobbyReady { get; private set; }  // Flag to indicate when the lobby is ready

    [HideInInspector]
    public Lobby _hostLobby;

    [HideInInspector]
    public Lobby _joinedLobby;

    private float _heartbeatTimer;
    private float _lobbyUpdateTimer;
    private string _playerName;

    [SerializeField] 
    private TMP_Text _feedback;

    private SceneController _sceneController;

    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Make the LobbyManager persist across scenes

        _sceneController = GameObject.FindWithTag("SceneController").GetComponent<SceneController>();
    }

    void Update() {
        HandleLobbyHeartbeat();
        HandleLobbyPollForUpdates();
    }

    public async void SignIn() {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () => {
            Debug.Log("Signed In" + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        _playerName = "Beans" + UnityEngine.Random.Range(10,99);
        Debug.Log(_playerName);
    }

    private async void HandleLobbyHeartbeat() {
        if (_hostLobby != null && _joinedLobby != null) {
            if (_joinedLobby.HostId == AuthenticationService.Instance.PlayerId) {
                // Only the host can send the heartbeat
                _heartbeatTimer -= Time.deltaTime;
                if (_heartbeatTimer < 0f) {
                    float heartbeatTimerMax = 15f;
                    _heartbeatTimer = heartbeatTimerMax;

                    // Send the heartbeat only if the player is the host
                    await LobbyService.Instance.SendHeartbeatPingAsync(_hostLobby.Id);
                }
            }
        }
    }


    private async void HandleLobbyPollForUpdates() {
        if (_joinedLobby != null) {
            _lobbyUpdateTimer -= Time.deltaTime;
            if (_lobbyUpdateTimer < 0f) {
                float lobbyUpdateTimerMax = 1.1f;
                _lobbyUpdateTimer = lobbyUpdateTimerMax;

                Lobby lobby = await LobbyService.Instance.GetLobbyAsync(_joinedLobby.Id);
                _joinedLobby = lobby;
            }
        }
    }

    public async void CreateLobby() {
        try {
            string lobbyName = "Osophy Trivia Edition";
            int maxPlayers = 3;
            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions {
                IsPrivate = false,
                Player = AddPlayer(),
                Data = new Dictionary<string, DataObject> {
                    {"GameMode", new DataObject(DataObject.VisibilityOptions.Public, "Trivia")},
                    {"Map", new DataObject(DataObject.VisibilityOptions.Public, "de_dust2")}
                }
            };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);
            
            _hostLobby = lobby;
            _joinedLobby = _hostLobby;

            _feedback.text = "Created Lobby " + lobby.Name;
            Debug.Log("Created Lobby!" + lobby.Name + " " + lobby.MaxPlayers + " " + lobby.Id + " " + lobby.LobbyCode);
            
            PrintPlayers(_hostLobby);
            IsLobbyReady = true;  // Set the flag to indicate that the lobby data is now available
            _sceneController.GoToLobby();
        } catch (LobbyServiceException e) {
            Debug.Log(e);
        }
    }

    public async void ListLobbies() {
        try {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions {
                Count = 25,
                Filters = new List<QueryFilter> {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT),
                },
                Order = new List<QueryOrder> {
                    new QueryOrder(false, QueryOrder.FieldOptions.Created)
                }
            };

            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            Debug.Log("Lobbies found: " + queryResponse.Results.Count);
            foreach(Lobby lobby in queryResponse.Results) {
                Debug.Log(lobby.Name + " " + lobby.MaxPlayers + " " + lobby.Data["GameMode"].Value);
                PrintPlayers(lobby);
            }
        } catch (LobbyServiceException e) {
            Debug.Log(e);
        }
    }

    public async void JoinLobbyByCode(string lobbyCode) {
        try {
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions {
                Player = AddPlayer()
            };
            
            // Join the lobby asynchronously
            Lobby lobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);
            _joinedLobby = lobby;
            _hostLobby = _joinedLobby;  // In case you want the host to be the same lobby

            _feedback.text = "Joined Lobby " + lobby.Name;
            Debug.Log("Joined Lobby with code" + lobbyCode);

            IsLobbyReady = true;  // Set the flag to indicate that the lobby data is now available

            PrintPlayers(_joinedLobby);
            _sceneController.GoToLobby();
        } catch (LobbyServiceException e) {
            Debug.LogError(e);
        }
    }

    private Player AddPlayer() {
        return new Player {
            Data = new Dictionary<string, PlayerDataObject> {
                {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, _playerName)}
            }
        };
    }

    // public string GetPlayer(Lobby lobby) {
    //     foreach (Player player in lobby.Players) {
    //         if (player.Id == AuthenticationService.Instance.PlayerId) {
    //             return player.Data["PlayerName"].Value;
    //         }
    //     }
    //     return null; // Returns null if player name is not found
    // }

    public void PrintPlayers(Lobby lobby) {
        Debug.Log("Players in Lobby " + lobby.Name + " " + lobby.Data["GameMode"].Value + " " + lobby.Data["Map"].Value);
        foreach(Player player in lobby.Players) {
            Debug.Log(player.Id + " " + player.Data["PlayerName"].Value);
        }
    }

    public void DisplayPlayers(Lobby lobby, GameObject[] profiles, TMP_Text[] names) {
        int idx = 0;
        foreach(Player player in lobby.Players) {
            profiles[idx].SetActive(true);
            names[idx].text = player.Data["PlayerName"].Value;
            idx++;
        }
    }

    private async void UpdateLobbyGameMode(string gameMode) {
        try {
            _hostLobby = await Lobbies.Instance.UpdateLobbyAsync(_hostLobby.Id, new UpdateLobbyOptions {
                Data = new Dictionary<string, DataObject> {
                    {"GameMode", new DataObject(DataObject.VisibilityOptions.Public, gameMode)}
                }
            });

            _joinedLobby = _hostLobby;

            PrintPlayers(_hostLobby);
        } catch (LobbyServiceException e) {
            Debug.Log(e);
        }
    }

    private async void UpdatePlayerName(string newPlayerName) {
        try{
            _playerName = newPlayerName;
            await LobbyService.Instance.UpdatePlayerAsync(_joinedLobby.Id, AuthenticationService.Instance.PlayerId, new UpdatePlayerOptions {
                Data = new Dictionary<string, PlayerDataObject> {
                    {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, _playerName)}
                }
            });
        } catch (LobbyServiceException e) {
            Debug.Log(e);
        }
    }

    private void LeaveLobby() {
        try {
            LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, AuthenticationService.Instance.PlayerId);
        } catch (LobbyServiceException e) {
            Debug.Log(e);
        }
    }

    private void KickPlayer() {
        try {
            LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, _joinedLobby.Players[1].Id);
        } catch (LobbyServiceException e) {
            Debug.Log(e);
        }
    }

    private async void MigrateLobbyHost() {
        try {
            _hostLobby = await Lobbies.Instance.UpdateLobbyAsync(_hostLobby.Id, new UpdateLobbyOptions {
                HostId = _joinedLobby.Players[1].Id
            });

            _joinedLobby = _hostLobby;

            PrintPlayers(_hostLobby);
        } catch (LobbyServiceException e) {
            Debug.Log(e);
        }
    }

    private void DeleteLobby() {
        try{
            LobbyService.Instance.DeleteLobbyAsync(_joinedLobby.Id);
        } catch (LobbyServiceException e) {
            Debug.Log(e);
        }
    }
}
