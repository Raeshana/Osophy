using System;
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

    public event EventHandler<LobbyEventArgs> OnJoinedLobby;
    public event EventHandler<LobbyEventArgs> OnJoinedLobbyUpdate;
    public event EventHandler<LobbyEventArgs> OnChoosePlayers;

    public class LobbyEventArgs : EventArgs {
        public Lobby lobby { get; set; }
    }

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
        _playerName = "Player" + UnityEngine.Random.Range(10,99);
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

                // Poll for lobby updates
                Lobby lobby = await LobbyService.Instance.GetLobbyAsync(_joinedLobby.Id);

                if (lobby != null && _joinedLobby.Players.Count != lobby.Players.Count) {
                    // Trigger UI update when players count changes
                    _joinedLobby = lobby;
                    OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = _joinedLobby });
                }

                if (lobby != null && 
                    (_joinedLobby.Data["Player1"].Value != lobby.Data["Player1"].Value ||
                    _joinedLobby.Data["Player2"].Value != lobby.Data["Player2"].Value)) {
                    // Trigger UI update when players count changes
                    _joinedLobby = lobby;
                    OnChoosePlayers?.Invoke(this, new LobbyEventArgs { lobby = _joinedLobby });
                }

                _joinedLobby = lobby;
                OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = _joinedLobby });
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
                    {"RoundNumber", new DataObject(DataObject.VisibilityOptions.Public, "1")},
                    {"Player1", new DataObject(DataObject.VisibilityOptions.Public, "")},
                    {"Player2", new DataObject(DataObject.VisibilityOptions.Public, "")},
                    {"Spectator", new DataObject(DataObject.VisibilityOptions.Public, "")},
                }
            };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);
            
            _hostLobby = lobby;
            _joinedLobby = _hostLobby;

            _feedback.text = "Created Lobby " + lobby.Name;
            Debug.Log("Created Lobby!" + lobby.Name + " " + lobby.MaxPlayers + " " + lobby.Id + " " + lobby.LobbyCode);
            
            //PrintPlayers(lobby);
            IsLobbyReady = true;  // Now that the lobby is created, flag is set to ready
            _sceneController.GoToLobby();  // Proceed to lobby scene
            StartCoroutine(InvokeEventInNewScene());
        } catch (LobbyServiceException e) {
            Debug.Log(e);
        }
    }

    private IEnumerator InvokeEventInNewScene() {
        // Wait until the next frame to allow the scene to load and the listener to be ready
        yield return null;
        // Now invoke the event
        OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = _joinedLobby });
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
                Debug.Log(lobby.Name + " " + lobby.MaxPlayers + " " + lobby.Data["RoundNumber"].Value + lobby.Data["Player1"].Value);
                //PrintPlayers(lobby);
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

            IsLobbyReady = true;  // Flag set after the lobby is joined
            _sceneController.GoToLobby();  // Transition to lobby scene
           StartCoroutine(InvokeEventInNewScene());
        } catch (LobbyServiceException e) {
            Debug.LogError(e);
        }
    }

    private Player AddPlayer() {
        return new Player {
            Data = new Dictionary<string, PlayerDataObject> {
                {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, _playerName)},
                {"Osopher1", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, "")},
                {"Osopher2", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, "")},
                {"Osopher3", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, "")},
                {"Debater", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, "")},
                {"isSpectator", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, "")},
                {"isAlive", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, "true")}
            }
        };
    }

    public string GetPlayer(Lobby lobby) {
        foreach (Player player in lobby.Players) {
            if (player.Id == AuthenticationService.Instance.PlayerId) {
                return player.Data["PlayerName"].Value;
            }
        }
        return null; // Returns null if player name is not found
    }

    public void PrintPlayers(Lobby lobby) {
        Debug.Log("Players in Lobby " + lobby.Name + " " + lobby.Data["RoundNumber"].Value);
        foreach(Player player in lobby.Players) {
            Debug.Log("Player ID: " + player.Id); 
            Debug.Log("Player Name: " + player.Data["PlayerName"].Value);
            Debug.Log("Osopher 1: " + player.Data["Osopher1"].Value);
            Debug.Log("Osopher 2: " + player.Data["Osopher2"].Value);
            Debug.Log("Osopher 3: " + player.Data["Osopher3"].Value);
            Debug.Log("Player Debater: " + player.Data["Debater"].Value);
            Debug.Log("isSpectator: " + player.Data["isSpectator"].Value);
            Debug.Log("isAlive: " + player.Data["isAlive"].Value);
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

    // REDO
    private async void UpdateLobbyRound(string round) {
        try {
            _hostLobby = await Lobbies.Instance.UpdateLobbyAsync(_hostLobby.Id, new UpdateLobbyOptions {
                Data = new Dictionary<string, DataObject> {
                    {"RoundNumber", new DataObject(DataObject.VisibilityOptions.Public, (Convert.ToInt32(round) + 1).ToString())}
                }
            });

            _joinedLobby = _hostLobby;

            //PrintPlayers(_hostLobby);
        } catch (LobbyServiceException e) {
            Debug.Log(e);
        }
    }

    public async void UpdatePlayerName(string newPlayerName) {
        this._playerName = newPlayerName;
        if (_joinedLobby != null) {
            try {
                UpdatePlayerOptions options = new UpdatePlayerOptions();

                options.Data = new Dictionary<string, PlayerDataObject>() {
                    {
                        "PlayerName", new PlayerDataObject(
                            visibility: PlayerDataObject.VisibilityOptions.Public,
                            value: newPlayerName)
                    }
                };

                string playerId = AuthenticationService.Instance.PlayerId;
                Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(_joinedLobby.Id, playerId, options);
                _joinedLobby = lobby;

                OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = _joinedLobby });
            } catch (LobbyServiceException e) {
                Debug.Log(e); 
            }   
        }
    }

    public async void UpdatePlayerOsophers(string osopher1,
                                            string osopher2,
                                            string osopher3) {
        if (_joinedLobby != null) {
            try {
                UpdatePlayerOptions options = new UpdatePlayerOptions();

                options.Data = new Dictionary<string, PlayerDataObject>() {
                    {
                        "Osopher1", new PlayerDataObject(
                            visibility: PlayerDataObject.VisibilityOptions.Public,
                            value: osopher1)
                    },
                    {
                        "Osopher2", new PlayerDataObject(
                            visibility: PlayerDataObject.VisibilityOptions.Public,
                            value: osopher2)
                    },
                    {
                        "Osopher3", new PlayerDataObject(
                            visibility: PlayerDataObject.VisibilityOptions.Public,
                            value: osopher3)
                    }
                };

                string playerId = AuthenticationService.Instance.PlayerId;
                Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(_joinedLobby.Id, playerId, options);
                _joinedLobby = lobby;

                OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = _joinedLobby });
            } catch (LobbyServiceException e) {
                Debug.Log(e); 
            }   
        }
    }

    public async void UpdatePlayerDebater(string debater) {
        if (_joinedLobby != null) {
            try {
                UpdatePlayerOptions options = new UpdatePlayerOptions();

                options.Data = new Dictionary<string, PlayerDataObject>() {
                    {
                        "Debater", new PlayerDataObject(
                            visibility: PlayerDataObject.VisibilityOptions.Public,
                            value: debater)
                    }
                };

                string playerId = AuthenticationService.Instance.PlayerId;
                Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(_joinedLobby.Id, playerId, options);
                _joinedLobby = lobby;

                OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = _joinedLobby });
            } catch (LobbyServiceException e) {
                Debug.Log(e); 
            }   
        }
    }

    public async void UpdatePlayerSpectatorStatus(string isSpectator) {
        if (_joinedLobby != null) {
            try {
                UpdatePlayerOptions options = new UpdatePlayerOptions();

                options.Data = new Dictionary<string, PlayerDataObject>() {
                    {
                        "isSpectator", new PlayerDataObject(
                            visibility: PlayerDataObject.VisibilityOptions.Public,
                            value: isSpectator)
                    }
                };

                string playerId = AuthenticationService.Instance.PlayerId;
                Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(_joinedLobby.Id, playerId, options);
                _joinedLobby = lobby;

                OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = _joinedLobby });
            } catch (LobbyServiceException e) {
                Debug.Log(e); 
            }   
        }
    }

    public async void UpdatePlayerAliveStatus(string isAlive) {
        if (_joinedLobby != null) {
            try {
                UpdatePlayerOptions options = new UpdatePlayerOptions();

                options.Data = new Dictionary<string, PlayerDataObject>() {
                    {
                        "isAlive", new PlayerDataObject(
                            visibility: PlayerDataObject.VisibilityOptions.Public,
                            value: isAlive)
                    }
                };

                string playerId = AuthenticationService.Instance.PlayerId;
                Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(_joinedLobby.Id, playerId, options);
                _joinedLobby = lobby;

                OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = _joinedLobby });
            } catch (LobbyServiceException e) {
                Debug.Log(e); 
            }   
        }
    }

    public string ChoosePlayer1() {
        if (_joinedLobby != null && _joinedLobby.Players.Count > 0 && _joinedLobby.HostId == AuthenticationService.Instance.PlayerId)
        {
            // Get all players
            List<Player> players = _joinedLobby.Players;

            // Generate a random index
            int randomIndex = UnityEngine.Random.Range(0, players.Count);

            // Select a random player
            string player1 = players[randomIndex].Id;

            UpdatePlayer1(player1);

            return players[randomIndex].Data["PlayerName"].Value;
        }
        else
        {
            Debug.LogWarning("No players in the lobby to select from! OR Not host!");
            return _joinedLobby.Data["Player1"].Value;
        }
    }

    public void ChoosePlayer2() {
        if (_joinedLobby != null && _joinedLobby.Players.Count > 0 && _joinedLobby.HostId == AuthenticationService.Instance.PlayerId)
        {
            // Get all players
            List<Player> players = _joinedLobby.Players;

            // Generate a random index
            int randomIndex = UnityEngine.Random.Range(0, players.Count);

            // Select a random player
            string player2 = players[randomIndex].Id;

            UpdatePlayer2(player2);
        }
        else
        {
            Debug.LogWarning("No players in the lobby to select from! OR Not host!");
        }
    }

    public async void UpdatePlayer1(string player1) {
        // Check if the current player is the host
        if (_joinedLobby != null && _joinedLobby.HostId == AuthenticationService.Instance.PlayerId)
        {
            Lobby lobby = await Lobbies.Instance.UpdateLobbyAsync(_joinedLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    { "Player1", new DataObject(DataObject.VisibilityOptions.Public, player1) }
                }
            });

            _joinedLobby = lobby;

            OnChoosePlayers?.Invoke(this, new LobbyEventArgs { lobby = _joinedLobby });
        }
        else
        {
            Debug.LogWarning("Only the host can update Player1!");
        }
    }

    public async void UpdatePlayer2(string player2) {
        try { 
            if (_joinedLobby != null && _joinedLobby.HostId == AuthenticationService.Instance.PlayerId)
            {           
                Lobby lobby = await Lobbies.Instance.UpdateLobbyAsync(_joinedLobby.Id, new UpdateLobbyOptions {
                    Data = new Dictionary<string, DataObject> {
                        { "Player2", new DataObject(DataObject.VisibilityOptions.Public, player2) }
                    }
                });

                _joinedLobby = lobby;

                OnChoosePlayers?.Invoke(this, new LobbyEventArgs { lobby = _joinedLobby });
            }
            else
            {
                Debug.LogWarning("Only the host can update Player2!");
            }
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

            //PrintPlayers(_hostLobby);
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

    public void GoToChooseOpponent() {
        if (AuthenticationService.Instance.PlayerId == _joinedLobby.Data["Player1"].Value) {
            _sceneController.GoToChooseOpponent();
        }
        else {
            _sceneController.GoToChooseOpponentSpectator();
        }
    }
}
