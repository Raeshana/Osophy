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
    private string player1;

    void Awake() {
        lobbyManager = GameObject.FindWithTag("LobbyManager").GetComponent<LobbyManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        player1 = lobbyManager.ChoosePlayer1();
        lobbyManager.OnChoosePlayers  += HandleOnUpdatePlayer1;
        lobbyManager.OnChoosePlayers  += HandleOnUpdatePlayer2;
    }

    // when entering this scene, we need to call choosePlayer1 which triggers the event to update player1
    private void HandleOnUpdatePlayer1(object sender, EventArgs e) {
        _playerName.text = player1;
    }

    private void HandleOnUpdatePlayer2(object sender, EventArgs e) {
        //lobbyManager.UpdatePlayer1(lobbyManager.ChoosePlayer1());
    }
}
