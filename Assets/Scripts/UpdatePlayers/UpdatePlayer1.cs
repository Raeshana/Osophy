using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpdatePlayer1 : MonoBehaviour
{
    private LobbyManager _lobbyManager;
    [SerializeField] 
    private TMP_Text _playerNameUI;

    void Awake() {
        _lobbyManager = GameObject.FindWithTag("LobbyManager").GetComponent<LobbyManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Host chooses Player1 when scene is called
        _lobbyManager.ChoosePlayer1();

        // Subscribe to OnChoosePlayer1 event
        _lobbyManager.OnChoosePlayer1  += HandleOnUpdatePlayer1;

        // trigger OnChoosePlayer1 event
        _lobbyManager.CallUpdatePlayer1();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void HandleOnUpdatePlayer1(object sender, LobbyManager.LobbyEventArgs e) {
        string _playerName = _lobbyManager.GetPlayer(e.lobby, e.lobby.Data["Player1"].Value);
        _playerNameUI.text = _playerName;
    }

    private void OnDestroy() {
        if (_lobbyManager != null) {
            _lobbyManager.OnJoinedLobby -= HandleOnUpdatePlayer1;
        }
    }
}
