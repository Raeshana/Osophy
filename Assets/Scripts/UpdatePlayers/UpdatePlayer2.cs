using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using System;

public class UpdatePlayer2 : MonoBehaviour
{
    [HideInInspector]
    public string opponentID; // Corresponding opponent's ID for this button

    private LobbyManager _lobbyManager; // Reference to the LobbyManger

    private SceneController _sceneController; // Reference to the SceneController

    public event EventHandler OnUpdateOpponent;

    void Awake() {
        // Populate _lobbyManager
        _lobbyManager = GameObject.FindWithTag("LobbyManager").GetComponent<LobbyManager>();

        // Populate _sceneController
        _sceneController = GameObject.FindWithTag("SceneController").GetComponent<SceneController>();
    }

    void Start() {
        // Subscribe to OnChoosePlayer2 event
        _lobbyManager.OnChoosePlayer2  += HandleOnUpdatePlayer2;

        // Subscribe to HandleOnAddOsopher
        OnUpdateOpponent += HandleOnUpdateOpponent;
    }

    /// <summary>
    /// When OnChoosePlayer2 event is triggered,
    /// all clients try to update Player2
    /// and the host successfuly does this
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void HandleOnUpdatePlayer2(object sender, LobbyManager.LobbyEventArgs e) {
        Player player = _lobbyManager.GetPlayer(e.lobby, e.lobby.Data["Player1"].Value);
        string _playerOpponent = player.Data["PlayerOpponent"].Value;
        _lobbyManager.UpdatePlayer2(_playerOpponent);
    }

    /// <summary>
    /// When the button is clicked, update PlayerOpponent metadata
    /// </summary>
    public void ChooseOpponentButton() {
        OnUpdateOpponent?.Invoke(this, EventArgs.Empty);
    }

    private void HandleOnUpdateOpponent(object sender, EventArgs e) {
        LobbyManager.Instance.UpdatePlayerOpponent(opponentID);
        StartCoroutine(GoToVersusRoutine());
    }

    /// <summary>
    /// Go to versus scene 3 seconds after button is clicked
    /// </summary>
    /// <returns></returns>
    private IEnumerator GoToVersusRoutine() {
        yield return new WaitForSeconds(3);
        _sceneController.GoToVersusScene();
    }

    private void OnDestroy() {
        if (_lobbyManager != null) {
            _lobbyManager.OnJoinedLobby -= HandleOnUpdatePlayer2;
        }
    }
}
