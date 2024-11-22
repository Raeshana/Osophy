using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class ChooseOpponentButton : MonoBehaviour
{
    public void ClickButton() {
        LobbyManager.Instance.GoToChooseOpponent();
    }
}
