using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopulateVersusScene : MonoBehaviour
{
    [SerializeField] 
    private TMP_Text _player1;
    [SerializeField] 
    private TMP_Text _player2;

    private LobbyManager _lobbyManager;

    void Awake() {
        _lobbyManager = GameObject.FindWithTag("LobbyManager").GetComponent<LobbyManager>();
    }

    void Start() {
        string player1Id = _lobbyManager._joinedLobby.Data["Player1"].Value;
        _player1.text = _lobbyManager.GetPlayer(_lobbyManager._joinedLobby, player1Id);

        string player2Id = _lobbyManager._joinedLobby.Data["Player2"].Value;
        _player2.text = _lobbyManager.GetPlayer(_lobbyManager._joinedLobby, player2Id);
    }
}
