using System.Collections;
using UnityEngine;
using TMPro;
using Unity.Services.Lobbies.Models;

public class PopulateLobby : MonoBehaviour
{
    private LobbyManager lobbyManager;

    [Header("Player Profiles ----------")]
    [SerializeField] 
    private GameObject[] _profiles;
    [SerializeField] 
    private TMP_Text[] _names;

    [Header("Coop Settings ----------")]
    [SerializeField] 
    private TMP_Text _code;

    void Awake() {
        // Find the LobbyManager
        lobbyManager = GameObject.FindWithTag("LobbyManager").GetComponent<LobbyManager>();
    }

    void Start() {
        StartCoroutine(WaitForLobbyData());
    }

    private IEnumerator WaitForLobbyData() {
        // Wait until the lobby data is ready
        while (!lobbyManager.IsLobbyReady) {
            yield return null;  // Wait for the next frame before checking again
        }
        // Once the data is ready, subscribe to the OnJoinedLobby event
        lobbyManager.OnJoinedLobby += HandleLobbyJoined;
    }

    private void HandleLobbyJoined(object sender, LobbyManager.LobbyEventArgs e) {
        // Update the UI when the player joins the lobby
        UpdateLobbyDisplay(e.lobby);
    }

    private void UpdateLobbyDisplay(Lobby lobby) {
        // Display player profiles
        lobbyManager.DisplayPlayers(lobby, _profiles, _names);

        // Set the join code
        _code.text = lobby.LobbyCode;
    }

    private void OnDestroy() {
        // Unsubscribe to prevent memory leaks
        if (lobbyManager != null) {
            lobbyManager.OnJoinedLobby -= HandleLobbyJoined;
        }
    }
}

    