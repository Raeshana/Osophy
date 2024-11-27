using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.Services.Lobbies.Models;

public class PopulateVersusScene : MonoBehaviour
{
    [SerializeField] 
    private TMP_Text _player1Text;
    [SerializeField] 
    private TMP_Text _player2Text;

    private LobbyManager _lobbyManager;

    void Awake() {
        _lobbyManager = GameObject.FindWithTag("LobbyManager").GetComponent<LobbyManager>();
    }

    void Start() {
        // Get PlayerName for Player1 and reflects that in _player1Text
        string _player1Id = _lobbyManager._joinedLobby.Data["Player1"].Value;
        Player _player1 = _lobbyManager.GetPlayer(_lobbyManager._joinedLobby, _player1Id);
        _player1Text.text = _player1.Data["PlayerName"].Value;

        // Get PlayerName for Player2 and reflects that in _player2Text
        string _player2Id = _lobbyManager._joinedLobby.Data["Player2"].Value;
        Player _player2 = _lobbyManager.GetPlayer(_lobbyManager._joinedLobby, _player2Id);
        _player2Text.text = _player2.Data["PlayerName"].Value;
    }
}
