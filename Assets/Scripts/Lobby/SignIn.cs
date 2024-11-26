using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignIn : MonoBehaviour
{
    private LobbyManager _lobbyManager;

    void Awake() {
        _lobbyManager = GameObject.FindWithTag("LobbyManager").GetComponent<LobbyManager>();
    }

    void Start()
    {
        // Ensures each player is signed in when they open the game
        _lobbyManager.SignIn();
    }
}
