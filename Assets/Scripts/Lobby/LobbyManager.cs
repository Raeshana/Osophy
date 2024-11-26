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

    [Header("Lobby Settings ----------")]
    [HideInInspector]
    public Lobby _hostLobby;
    [HideInInspector]
    public Lobby _joinedLobby;

    [Header("Timer Settings ----------")]
    private float _heartbeatTimer;
    private float _lobbyUpdateTimer;
    private string _playerName;

    [Header("Scene Settings ----------")]
    [SerializeField] 
    private TMP_Text _feedback;
    private SceneController _sceneController;

    public event EventHandler<LobbyEventArgs> OnJoinedLobby; // Triggered when a new player joins a lobby
    public event EventHandler<LobbyEventArgs> OnJoinedLobbyUpdate; // Triggered when the lobby is updated
    public event EventHandler<LobbyEventArgs> OnChoosePlayers; // Triggered when the game updates on player 1 and 2

    public class LobbyEventArgs : EventArgs {
        public Lobby lobby { get; set; }
    }    

    void Awake() {
        // Ensure only one Instance of LobbyManager is running at one time
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Make the LobbyManager persist across scenes
        DontDestroyOnLoad(gameObject); 

        // Populate _sceneController
        _sceneController = GameObject.FindWithTag("SceneController").GetComponent<SceneController>();
    }

    void Update() {
        HandleLobbyHeartbeat();
        HandleLobbyPollForUpdates();
    }

    /// <summary>
    /// Adds players to a Lobby
    /// </summary>
    public async void SignIn() {
        // Sign in using Unity Lobby
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () => {
            Debug.Log("Signed In" + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        // Assign a random username to player
        // Update to text from an input field
        _playerName = "Player" + UnityEngine.Random.Range(10,99);
        Debug.Log(_playerName);
    }

    /// <summary>
    /// Sends periodic pings to keep Lobby active
    /// </summary>
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

    /// <summary>
    /// Periodically checks updates and applies changes as necessary
    /// </summary>
    private async void HandleLobbyPollForUpdates() {
        if (_joinedLobby != null) {
            _lobbyUpdateTimer -= Time.deltaTime;
            if (_lobbyUpdateTimer < 0f) {
                float lobbyUpdateTimerMax = 1.1f;
                _lobbyUpdateTimer = lobbyUpdateTimerMax;

                // Poll for lobby updates
                Lobby lobby = await LobbyService.Instance.GetLobbyAsync(_joinedLobby.Id);

                // Updates profiles displayed in Lobby scene when player joins
                if (lobby != null && _joinedLobby.Players.Count != lobby.Players.Count) {
                    _joinedLobby = lobby;
                    OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = _joinedLobby });
                }

                // Updates Player1 and Player2 metadata 
                // When game chooses Player1
                // When Player1 chooses Player2  
                if (lobby != null && 
                    (_joinedLobby.Data["Player1"].Value != lobby.Data["Player1"].Value ||
                    _joinedLobby.Data["Player2"].Value != lobby.Data["Player2"].Value)) {
                    _joinedLobby = lobby;
                    OnChoosePlayers?.Invoke(this, new LobbyEventArgs { lobby = _joinedLobby });
                }

                // 
                _joinedLobby = lobby;
                OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = _joinedLobby });
            }
        }
    }

    /// <summary>
    /// Creates a Lobby with a join code and the following metadata:
    /// RoundNumber: Begins at 1 and increments at the end of each round
    /// Player1: ID of the player chosen to play first
    /// Player2: ID of the player chosen by Player1
    /// Spectator: ID of the player who is not playing
    /// </summary>
    public async void CreateLobby() {
        try {
            string _lobbyName = "Osophy Trivia Edition";
            int _maxPlayers = 3;
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

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(_lobbyName, _maxPlayers, createLobbyOptions);
            
            _hostLobby = lobby;
            _joinedLobby = _hostLobby;

            _feedback.text = "Created Lobby " + lobby.Name; // Remove 
            Debug.Log("Created Lobby!" + lobby.Name + " " + lobby.MaxPlayers + " " + lobby.Id + " " + lobby.LobbyCode);

            IsLobbyReady = true;  // Now that the lobby is created, flag is set to ready

            // Go to Lobby Scene
            _sceneController.GoToLobby();  
            StartCoroutine(InvokeEventInNewScene());
        } catch (LobbyServiceException e) {
            Debug.Log(e);
        }
    }

    /// <summary>
    /// Waits until the scene transitions from MainMenu to Lobby 
    /// </summary>
    /// <returns></returns>
    private IEnumerator InvokeEventInNewScene() {
        // Wait until the next frame to allow the scene to load and the listener to be ready
        yield return null;
        // Now invoke the event
        OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = _joinedLobby });
    }

    /// <summary>
    /// Adds a player to the Lobby if the code they entered corresponds
    /// to the Lobby code
    /// </summary>
    /// <param name="lobbyCode"> Code entered by user in an input field </param>
    public async void JoinLobbyByCode(string lobbyCode) {
        try {
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions {
                Player = AddPlayer()
            };
            
            // Join the lobby asynchronously
            Lobby lobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);
            _joinedLobby = lobby;
            _hostLobby = _joinedLobby;  // Host is in the same Lobby

            _feedback.text = "Joined Lobby " + lobby.Name;
            Debug.Log("Joined Lobby with code" + lobbyCode);

            IsLobbyReady = true;  // Flag set after the lobby is joined

            // Go to Lobby Scene
            _sceneController.GoToLobby(); 
           StartCoroutine(InvokeEventInNewScene());
        } catch (LobbyServiceException e) {
            Debug.LogError(e);
        }
    }

    /// <summary>
    /// Creates a player with metadata:
    /// PlayerName: Name entered by the player
    /// Osopher1: Name of player's first scanned Osopher
    /// Osopher2: Name of player's second scanned Osopher
    /// Osopher3: Name of player's third scanned Osopher
    /// Debater: Name of player's Osopher card who will be debating
    /// isSpectator: true if player is spectating, false if they are playing
    /// numCards: Number of cards player has in hand (not necessarily playable)
    /// </summary>
    /// <returns> Player </returns>
    private Player AddPlayer() {
        return new Player {
            Data = new Dictionary<string, PlayerDataObject> {
                {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, _playerName)},
                {"Osopher1", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, "")},
                {"Osopher2", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, "")},
                {"Osopher3", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, "")},
                {"Debater", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, "")},
                {"isSpectator", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, "true")},
                {"numCards", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, "3")}
            }
        };
    }

    /// <summary>
    /// Finds a player's name given their ID
    /// </summary>
    /// <param name="playerId"></param>
    /// <returns> Name of a player whose ID matches playerID </returns>
    public string GetPlayer(Lobby lobby, string playerId) {
        foreach (Player player in lobby.Players) {
            if (player.Id == playerId) {
                return player.Data["PlayerName"].Value;
            }
        }
        return null; // Returns null if player name is not found
    }

    /// <summary>
    /// Prints each player's metadata to the terminal
    /// </summary>
    /// <param name="lobby"></param>
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

    /// <summary>
    /// Called by event handler when a player updates their name
    /// so that it can be updated across all clients
    /// </summary>
    /// <param name="newPlayerName"> Changed player name </param>
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

                // Update Lobby with new player information
                string _playerId = AuthenticationService.Instance.PlayerId;
                Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(_joinedLobby.Id, _playerId, options);
                _joinedLobby = lobby;

                // Trigger OnJoinedLobbyUpdate event
                OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = _joinedLobby });
            } catch (LobbyServiceException e) {
                Debug.Log(e); 
            }   
        }
    }

    /// <summary>
    /// Called by event handler when a player's Osophers are updated
    /// so that it can be updated across all clients
    /// </summary>
    /// <param name="osopher1"></param>
    /// <param name="osopher2"></param>
    /// <param name="osopher3"></param>
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

                // Update Lobby with new player information
                string playerId = AuthenticationService.Instance.PlayerId;
                Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(_joinedLobby.Id, playerId, options);
                _joinedLobby = lobby;

                // Trigger OnJoinedLobbyUpdate event
                OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = _joinedLobby });
            } catch (LobbyServiceException e) {
                Debug.Log(e); 
            }   
        }
    }

    /// <summary>
    /// Called by event handler when a player's debater is updated
    /// so that it can be updated across all clients
    /// </summary>
    /// <param name="debater"></param>
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

                // Update Lobby with new player information
                string playerId = AuthenticationService.Instance.PlayerId;
                Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(_joinedLobby.Id, playerId, options);
                _joinedLobby = lobby;

                // Trigger OnJoinedLobbyUpdate event
                OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = _joinedLobby });
            } catch (LobbyServiceException e) {
                Debug.Log(e); 
            }   
        }
    }

    /// <summary>
    /// Called by event handler when a player's spectator status is updated
    /// so that it can be updated across all clients
    /// </summary>
    /// <param name="isSpectator"></param>
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

                // Update Lobby with new player information
                string playerId = AuthenticationService.Instance.PlayerId;
                Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(_joinedLobby.Id, playerId, options);
                _joinedLobby = lobby;

                // Trigger OnJoinedLobbyUpdate event
                OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = _joinedLobby });
            } catch (LobbyServiceException e) {
                Debug.Log(e); 
            }   
        }
    }

    /// <summary>
    /// Called by event handler when a player's card number is updated
    /// so that it can be updated across all clients
    /// </summary>
    /// <param name="numCards"></param>
    public async void UpdatePlayerNumCards(string numCards) {
        if (_joinedLobby != null) {
            try {
                UpdatePlayerOptions options = new UpdatePlayerOptions();

                options.Data = new Dictionary<string, PlayerDataObject>() {
                    {
                        "numCards", new PlayerDataObject(
                            visibility: PlayerDataObject.VisibilityOptions.Public,
                            value: numCards)
                    }
                };

                // Update Lobby with new player information
                string playerId = AuthenticationService.Instance.PlayerId;
                Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(_joinedLobby.Id, playerId, options);
                _joinedLobby = lobby;

                // Trigger OnJoinedLobbyUpdate event
                OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = _joinedLobby });
            } catch (LobbyServiceException e) {
                Debug.Log(e); 
            }   
        }
    }

    /// <summary>
    /// Randomly choose a player to player first from those in the Lobby
    /// </summary>
    /// <returns></returns>
    public void ChoosePlayer1() {
        if (_joinedLobby != null && _joinedLobby.Players.Count > 0 && _joinedLobby.HostId == AuthenticationService.Instance.PlayerId)
        {
            // Get all players
            List<Player> players = _joinedLobby.Players;

            // Generate a random index
            int randomIndex = UnityEngine.Random.Range(0, players.Count);

            // Select a random player
            string player1 = players[randomIndex].Id;

            // Update Player1 metadata
            UpdatePlayer1(player1);
        }
        else
        {
            Debug.LogWarning("No players in the lobby to select from! OR Not host!");
        }
    }

    /// <summary>
    /// Called by event handler when Player1 is chosen
    /// on PlayerTurn scene
    /// </summary>
    /// <param name="player1"></param>
    public async void UpdatePlayer1(string player1) {
        try{
            UpdateLobbyOptions options = new UpdateLobbyOptions();
            options.Name = "testLobbyName";
            options.MaxPlayers = 4;
            options.IsPrivate = false;

            options.HostId = AuthenticationService.Instance.PlayerId;

            options.Data = new Dictionary<string, DataObject>()
            {
                {
                    "Player1", new DataObject(
                        visibility: DataObject.VisibilityOptions.Public,
                        value: player1)
                }
            };

            var lobby = await LobbyService.Instance.UpdateLobbyAsync(_joinedLobby.Id, options);
            _joinedLobby = lobby;

            // Trigger OnChoosePlayers event
            OnChoosePlayers?.Invoke(this, new LobbyEventArgs { lobby = _joinedLobby });

            // Migrate host to Player1 so they can make changes to the Lobby
            // MigrateLobbyHost(_joinedLobby.Data["Player1"].Value);
        } catch (LobbyServiceException e) {
            Debug.Log(e);
        }
    }

    public async void UpdatePlayer2(string player2) {
        try {         
            Lobby lobby = await Lobbies.Instance.UpdateLobbyAsync(_joinedLobby.Id, new UpdateLobbyOptions {
                Data = new Dictionary<string, DataObject> {
                    { "Player2", new DataObject(DataObject.VisibilityOptions.Public, player2) }
                }
            });

            _joinedLobby = lobby;

            OnChoosePlayers?.Invoke(this, new LobbyEventArgs { lobby = _joinedLobby });
            StartCoroutine(GoToChooseOpponentRoutine());
        } catch (LobbyServiceException e) {
            Debug.Log(e);
        }
    }

    /// <summary>
    /// Print all active lobbies to the terminal
    /// </summary>
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
                Debug.Log(lobby.Name + " " + lobby.MaxPlayers + " " + lobby.HostId + " " + lobby.Data["RoundNumber"].Value + " " + lobby.Data["Player1"].Value + " " + lobby.Data["Player2"].Value);
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

    private async void MigrateLobbyHost(string newHostID) {
        try {
            _hostLobby = await Lobbies.Instance.UpdateLobbyAsync(_hostLobby.Id, new UpdateLobbyOptions {
                HostId = newHostID
            });

            _joinedLobby = _hostLobby;

            //PrintPlayers(_hostLobby);

            OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = _joinedLobby });
        } catch (LobbyServiceException e) {
            Debug.Log(e);
        }
    }

    private async void UpdateLobbyHost(string newHostID) {
        try {
            _hostLobby = await Lobbies.Instance.UpdateLobbyAsync(_hostLobby.Id, new UpdateLobbyOptions {
                HostId = newHostID
            });

            _joinedLobby = _hostLobby;

            OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = _joinedLobby });
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

    private IEnumerator GoToChooseOpponentRoutine() {
        yield return new WaitForSeconds(2);
        _sceneController.GoToVersusScene();
    }

    public void GoToChooseOpponent() {
        if (AuthenticationService.Instance.PlayerId == _joinedLobby.Data["Player1"].Value) {
            _sceneController.GoToChooseOpponent();
        }
        else {
            _sceneController.GoToChooseOpponentSpectator();
        }
    }

    public void GoToChooseGameplay() {
        if (AuthenticationService.Instance.PlayerId == _joinedLobby.Data["Player1"].Value) {
            _sceneController.GoToGameplayQRScanningScene();
        }
        else if (AuthenticationService.Instance.PlayerId == _joinedLobby.Data["Player2"].Value){
            _sceneController.GoToGameplayQRScanningScene();
        }
        else {
            _sceneController.GoToSpectatorScene();
        }
    }
}
