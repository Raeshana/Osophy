using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class JoinLobby : MonoBehaviour
{
    private LobbyManager _lobbyManager;
    [SerializeField] 
    private TMP_InputField _inputCode;

    private void Awake() {
        _lobbyManager = GameObject.FindWithTag("LobbyManager").GetComponent<LobbyManager>();
    }

    /// <summary>
    /// Allows player to join a Lobby using a code when
    /// button is clicked
    /// </summary>
    public void JoinLobbyByCode() {
        _lobbyManager.JoinLobbyByCode(_inputCode.text);
    }
}
