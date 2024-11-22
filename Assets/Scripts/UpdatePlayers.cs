using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpdatePlayers : MonoBehaviour
{
    private LobbyManager lobbyManager;
    [SerializeField] TMP_Text _playerName;

    void Awake() {
        lobbyManager = GameObject.FindWithTag("LobbyManager").GetComponent<LobbyManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        lobbyManager.OnChoosePlayers  += HandleOnUpdatePlayer1;
        lobbyManager.OnChoosePlayers  += HandleOnUpdatePlayer2;
    }

    private void HandleOnUpdatePlayer1(object sender, EventArgs e) {
        string player1 = lobbyManager.ChoosePlayer1();
        _playerName.text = player1;
        lobbyManager.UpdatePlayer1(player1);
    }

    private void HandleOnUpdatePlayer2(object sender, EventArgs e) {
        //lobbyManager.UpdatePlayer1(lobbyManager.ChoosePlayer1());
    }
}
