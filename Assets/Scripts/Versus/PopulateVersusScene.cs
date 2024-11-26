using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopulateVersusScene : MonoBehaviour
{
    [SerializeField] TMP_Text player1;
    [SerializeField] TMP_Text player2;

    private LobbyManager lobbyManager;

    void Awake() {
        lobbyManager = GameObject.FindWithTag("LobbyManager").GetComponent<LobbyManager>();
    }

    void Start() {
        // string player1Id = lobbyManager._joinedLobby.Data["Player1"].Value;
        // player1.text = lobbyManager.GetPlayer(player1Id);

        // string player2Id = lobbyManager._joinedLobby.Data["Player2"].Value;
        // player1.text = lobbyManager.GetPlayer(player2Id);
    }
}
