using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Services.Lobbies.Models;

public class PopulateChooseOpponentSpectator : MonoBehaviour
{
    private LobbyManager _lobbyManager; // Reference to the Lobby Manager

    [SerializeField]
    private TMP_Text _player1Text; // Displays Player1 PlayerName

    void Awake() {
        // Populate _lobbyManager
        _lobbyManager = GameObject.FindWithTag("LobbyManager").GetComponent<LobbyManager>();
    }

    void Start() {
        // Get PlayerName for Player1 and reflects that in _player1Text
        string _playerID = _lobbyManager._joinedLobby.Data["Player1"].Value;
        Player _player = _lobbyManager.GetPlayer(_lobbyManager._joinedLobby, _playerID);
        _player1Text.text = _player.Data["PlayerName"].Value;
    }
}
