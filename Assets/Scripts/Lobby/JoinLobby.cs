using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class JoinLobby : MonoBehaviour
{
    private LobbyManager lobbyManager;
    [SerializeField] TMP_InputField _inputCode;

    private void Awake() {
        lobbyManager = GameObject.FindWithTag("LobbyManager").GetComponent<LobbyManager>();
    }

    public void JoinLobbyByCode() {
        lobbyManager.JoinLobbyByCode(_inputCode.text);
    }
}
