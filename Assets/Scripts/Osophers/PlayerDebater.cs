using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDebater : MonoBehaviour
{
    public static string debater;

    public event EventHandler OnAssignDebater;

    public void AssignDebater(string osopherName) {
        debater = osopherName;
        OnAssignDebater?.Invoke(this, EventArgs.Empty);
    }

    private void HandleOnAssignDebater(object sender, EventArgs e) {
        LobbyManager.Instance.UpdatePlayerDebater(debater);
    }

    public void Update() {
        //Debug.Log(debater);
    }
}
