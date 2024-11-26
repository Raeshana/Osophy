using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerName : MonoBehaviour {

    public static PlayerName Instance { get; private set; }

    public event EventHandler OnNameChanged;

    private string _playerName = "Code Monkey"; // Replace with text from imput field

    /// <summary>
    /// Triggers the OnNameChanged event when button is clicked
    /// </summary>
    public void ClickButton() {
        OnNameChanged?.Invoke(this, EventArgs.Empty);
    }

    private void Start() {
        // Subscribe to OnNameChanged event
        OnNameChanged += PlayerName_OnNameChanged;
    }

    /// <summary>
    /// Calls UpdatePlayerName to ensure changes are made across all clients
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void PlayerName_OnNameChanged(object sender, EventArgs e) {
        LobbyManager.Instance.UpdatePlayerName(_playerName);
    }

    private void OnDestroy() {
        OnNameChanged -= PlayerName_OnNameChanged;
    }
}