using System.Collections;
using UnityEngine;
using TMPro;
using Unity.Services.Lobbies.Models;

public class PopulateLobby : MonoBehaviour
{
    private LobbyManager _lobbyManager;

    [Header("Player Profiles ----------")]
    [SerializeField] 
    private GameObject[] _profiles;
    [SerializeField] 
    private TMP_Text[] _names;

    [Header("Coop Settings ----------")]
    [SerializeField] 
    private TMP_Text _code;

    void Awake() {
        _lobbyManager = GameObject.FindWithTag("LobbyManager").GetComponent<LobbyManager>();
    }

    void Start() {
        // Start a coroutine to check if the lobby data is ready
        StartCoroutine(WaitForLobbyData());
    }

    /// <summary>
    /// Ensure data is ready before subscribing to event
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForLobbyData() {
        while (!_lobbyManager.IsLobbyReady) {
            yield return null; 
        }

        // Subscribe to the OnJoinedLobby event
        _lobbyManager.OnJoinedLobby += HandleLobbyJoined;
    }

    /// <summary>
    /// Handles the OnJoinedLobby event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void HandleLobbyJoined(object sender, LobbyManager.LobbyEventArgs e) {
        UpdateLobbyDisplay(e.lobby);
    }

    /// <summary>
    /// Updates player profiles in Lobby scene
    /// </summary>
    /// <param name="lobby"></param>
    private void UpdateLobbyDisplay(Lobby lobby) {
        int idx = 0; // Counter and indexing
        foreach(Player player in lobby.Players) {
            _profiles[idx].SetActive(true); // Display player profile 
            _names[idx].text = player.Data["PlayerName"].Value; // Update player name 
            idx++;
        }

        // Display join code
        _code.text = lobby.LobbyCode;
    }

    /// <summary>
    /// Unsubscribe to prevent memory leaks
    /// </summary>
    private void OnDestroy() {
        if (_lobbyManager != null) {
            _lobbyManager.OnJoinedLobby -= HandleLobbyJoined;
        }
    }
}
