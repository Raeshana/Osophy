using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopulateChooseOpponent : MonoBehaviour
{
    private LobbyManager lobbyManager;

    [SerializeField]
    private GameObject[] playerButtons;
    [SerializeField]
    private TMP_Text[] playerButtonText;
    [SerializeField]
    private StoreOpponentID[] storePlayerID;

    void Awake() {
        lobbyManager = GameObject.FindWithTag("LobbyManager").GetComponent<LobbyManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        int i = 0;
        foreach(var player in lobbyManager._joinedLobby.Players) {
            if (player.Id != lobbyManager._joinedLobby.Data["Player1"].Value) {
                playerButtons[i].SetActive(true);
                playerButtonText[i].text = player.Data["PlayerName"].Value;
                storePlayerID[i].opponentID = player.Id;
                i++;
            }
        }
    }
}
