using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopulateChooseOpponentSpectator : MonoBehaviour
{
    private LobbyManager _lobbyManager;

    [SerializeField]
    private TMP_Text _playerButtonText;

    void Awake() {
        _lobbyManager = GameObject.FindWithTag("LobbyManager").GetComponent<LobbyManager>();
    }

    void Start() {
        string _playerID = _lobbyManager._joinedLobby.Data["Player1"].Value;
        _playerButtonText.text = _lobbyManager.GetPlayer(_lobbyManager._joinedLobby, _playerID);
    }
}
