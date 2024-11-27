using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class UpdatePlayer1 : MonoBehaviour
{
    private LobbyManager _lobbyManager; // Reference to Lobby Manager

    [SerializeField] 
    private TMP_Text _playerNameUI; // // Displays Player1 PlayerName

    void Awake() {
        // Populate _lobbyManager
        _lobbyManager = GameObject.FindWithTag("LobbyManager").GetComponent<LobbyManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Host chooses Player1 when scene is called
        _lobbyManager.ChoosePlayer1();

        // Subscribe to OnChoosePlayer1 event
        _lobbyManager.OnChoosePlayer1  += HandleOnUpdatePlayer1;
    }

    /// <summary>
    /// Gets PlayerName from Player1 and updates _playerNameUI to match
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void HandleOnUpdatePlayer1(object sender, LobbyManager.LobbyEventArgs e) {
        Player _player = _lobbyManager.GetPlayer(e.lobby, e.lobby.Data["Player1"].Value);
        string _playerName = _player.Data["PlayerName"].Value;
        _playerNameUI.text = _playerName;
    }

    private void OnDestroy() {
        if (_lobbyManager != null) {
            _lobbyManager.OnJoinedLobby -= HandleOnUpdatePlayer1;
        }
    }
}
