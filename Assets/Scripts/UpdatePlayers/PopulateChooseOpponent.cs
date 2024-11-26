using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopulateChooseOpponent : MonoBehaviour
{
    private LobbyManager lobbyManager;

    [SerializeField]
    private GameObject[] _playerButtons;
    [SerializeField]
    private TMP_Text[] _playerButtonText;
    [SerializeField]
    private StoreOpponentID[] _storePlayerID;

    void Awake() {
        lobbyManager = GameObject.FindWithTag("LobbyManager").GetComponent<LobbyManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        int i = 0;
        foreach(var player in lobbyManager._joinedLobby.Players) {
            if (player.Id != lobbyManager._joinedLobby.Data["Player1"].Value) {
                _playerButtons[i].SetActive(true);
                _playerButtonText[i].text = player.Data["PlayerName"].Value;
                _storePlayerID[i].opponentID = player.Id;
                i++;
            }
        }
    }
}
