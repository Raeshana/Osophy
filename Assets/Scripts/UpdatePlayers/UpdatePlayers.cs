using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpdatePlayers : MonoBehaviour
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

        // Subscribe to OnChoosePlayers event
        _lobbyManager.OnChoosePlayers  += HandleOnUpdatePlayer1;
        _lobbyManager.OnChoosePlayers  += HandleOnUpdatePlayer2;
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

    private void HandleOnUpdatePlayer2(object sender, EventArgs e) {
        //lobbyManager.UpdatePlayer1(lobbyManager.ChoosePlayer1());
    }
}
