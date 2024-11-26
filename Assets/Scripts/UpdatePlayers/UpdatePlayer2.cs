using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class UpdatePlayer2 : MonoBehaviour
{
    [HideInInspector]
    public string opponentID; // Corresponding opponent's ID for this button

    private LobbyManager _lobbyManager; // Reference to the LobbyManger

    private SceneController _sceneController; // Reference to the SceneController

    void Awake() {
        // Populate _lobbyManager
        _lobbyManager = GameObject.FindWithTag("LobbyManager").GetComponent<LobbyManager>();

        // Populate _sceneController
        _sceneController = GameObject.FindWithTag("SceneController").GetComponent<SceneController>();
    }

    void Start() {
        // Subscribe to OnChoosePlayer2 event
        _lobbyManager.OnChoosePlayer2  += HandleOnUpdatePlayer2;
    }

    /// <summary>
    /// When OnChoosePlayers event is triggered,
    /// all clients try to update player 2
    /// and the host successfuly does this
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void HandleOnUpdatePlayer2(object sender, LobbyManager.LobbyEventArgs e) {
        _lobbyManager.UpdatePlayer2(opponentID);
    }

    /// <summary>
    /// When the button is clicked, trigger OnChoosePlayers event
    /// </summary>
    public void ChooseOpponentButton() {
        _lobbyManager.CallUpdatePlayer2();
        StartCoroutine(GoToVersusRoutine());
    }

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
