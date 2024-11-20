using System.Collections;
using UnityEngine;
using TMPro;

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
        lobbyManager = GameObject.FindWithTag("LobbyManager").GetComponent<LobbyManager>();
    }

    void Start() {
        // Start a coroutine to check if the lobby data is ready
        StartCoroutine(WaitForLobbyData());
    }

    private IEnumerator WaitForLobbyData() {
        // Wait until the lobby data is ready
        while (!lobbyManager.IsLobbyReady) {
            yield return null;  // Wait for the next frame before checking again
        }

        // Once the data is ready, update the lobby display
        UpdateLobbyDisplay();
    }

    private void UpdateLobbyDisplay() {
        // Display player profiles
        lobbyManager.DisplayPlayers(lobbyManager._hostLobby, _profiles, _names);

        // Set join code
        _code.text = lobbyManager._joinedLobby.LobbyCode;
    }
}
