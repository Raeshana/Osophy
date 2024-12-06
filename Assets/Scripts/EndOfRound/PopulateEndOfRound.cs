using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class PopulateEndOfRound : MonoBehaviour
{
    [Header("Winner Settings ---------")]
    [SerializeField]
    private string _winnerInstructions;

    [Header("Loser Settings ---------")]
    [SerializeField]
    private string _loserInstructions;

    [Header("Spectator Settings ---------")]
    [SerializeField]
    private string _spectatorInstructions;

    [SerializeField]
    private TMP_Text _instructions;

    [SerializeField]
    private PlayerOsopherDict _playerOsopherDict;

    [SerializeField]
    private SceneController _sceneController;

    private string _playerID;

    private LobbyManager _lobbyManager; // Reference to the LobbyManger

    void Awake() {
        // Populate _lobbyManager
        _lobbyManager = GameObject.FindWithTag("LobbyManager").GetComponent<LobbyManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _playerID = _lobbyManager.GetCurrentPlayerID();

        // Winner
        if (_playerID == LobbyManager.Instance._joinedLobby.Data["RoundWinner"].Value) {
            _instructions.text = _winnerInstructions;

            // Remove Debating Osopher
            RemoveOsopher();
        } // Loser
        else if ((_playerID == LobbyManager.Instance._joinedLobby.Data["Player1"].Value) ||
                (_playerID == LobbyManager.Instance._joinedLobby.Data["Player2"].Value)) {
            _instructions.text = _loserInstructions;

            // Remove Debating Osopher
            RemoveOsopher();
        }
        else { // Spectator
            _instructions.text = _spectatorInstructions;
        }
    }

    private void RemoveOsopher() {
        Player _player = _lobbyManager.GetPlayer(_lobbyManager._joinedLobby, _playerID);
        string _debater = _player.Data["Debater"].Value;
        _playerOsopherDict.RemoveOsopher(_debater);
    }

    public void EndOfRoundScene() {
        if ((_playerID == LobbyManager.Instance._joinedLobby.Data["Player1"].Value) ||
            (_playerID == LobbyManager.Instance._joinedLobby.Data["Player2"].Value)) {
            _sceneController.GoToRoundQRScanningScene();
        }
        else {
            _sceneController.GoToPlayerTurnScene();
        }
    }
}
