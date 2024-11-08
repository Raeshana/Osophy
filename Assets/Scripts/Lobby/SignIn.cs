using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignIn : MonoBehaviour
{
    private LobbyManager lobbyManager;

    void Awake() {
        lobbyManager = GameObject.FindWithTag("LobbyManager").GetComponent<LobbyManager>();
    }

    void Start()
    {
        lobbyManager.SignIn();
    }
}
