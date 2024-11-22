using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class test : MonoBehaviour
{
    private LobbyManager lobbyManager;
    public event EventHandler OnJoinedLobbyUpdate;

    void Awake() {
        lobbyManager = GameObject.FindWithTag("LobbyManager").GetComponent<LobbyManager>();
    }

    public void ClickButton() {
        OnJoinedLobbyUpdate?.Invoke(this, EventArgs.Empty);
    }

    private void Start() {
        lobbyManager.OnJoinedLobbyUpdate += LobbyManager_OnJoinedLobbyUpdate;
    }

    private void LobbyManager_OnJoinedLobbyUpdate(object sender, LobbyManager.LobbyEventArgs e) {
        lobbyManager.PrintPlayers(e.lobby); 
    }
}
