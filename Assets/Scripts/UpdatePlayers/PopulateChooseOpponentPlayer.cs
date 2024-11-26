using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class PopulateChooseOpponentPlayer : MonoBehaviour
{
    private LobbyManager _lobbyManager;

    [SerializeField]
    private GameObject[] _playerButtons;
    [SerializeField]
    private TMP_Text[] _playerButtonText;

    void Awake() {
        _lobbyManager = GameObject.FindWithTag("LobbyManager").GetComponent<LobbyManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        int i = 0;
        foreach(var player in _lobbyManager._joinedLobby.Players) {
            if (player.Id != _lobbyManager._joinedLobby.Data["Player1"].Value) {
                _playerButtons[i].SetActive(true);
                _playerButtonText[i].text = player.Data["PlayerName"].Value;
                _playerButtons[i].GetComponent<UpdatePlayer2>().opponentID = player.Id;
                i++;
            }
        }
    }
}
