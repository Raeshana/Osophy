using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class ChooseOpponentButton : MonoBehaviour
{
    private StoreOpponentID _storeOpponentID;
    private string _opponentID;

    private LobbyManager _lobbyManager;

    void Awake() {
        _lobbyManager = GameObject.FindWithTag("LobbyManager").GetComponent<LobbyManager>();
        _storeOpponentID = GetComponent<StoreOpponentID>();
        _opponentID = _storeOpponentID.opponentID;
    }

    public void ClickButton() {
        _lobbyManager.UpdatePlayer2(_opponentID);
    }
}
