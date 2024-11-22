using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditPlayerName : MonoBehaviour {


    public static EditPlayerName Instance { get; private set; }


    public event EventHandler OnNameChanged;


    private string playerName = "Code Monkey";

    public void ClickButton() {
        OnNameChanged?.Invoke(this, EventArgs.Empty);
    }

    private void Start() {
        OnNameChanged += EditPlayerName_OnNameChanged;
    }

    private void EditPlayerName_OnNameChanged(object sender, EventArgs e) {
        LobbyManager.Instance.UpdatePlayerName(GetPlayerName());
    }

    public string GetPlayerName() {
        return playerName;
    }


}