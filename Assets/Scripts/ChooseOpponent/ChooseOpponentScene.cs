using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class ChooseOpponentScene : MonoBehaviour
{
    public void ClickButton() {
        LobbyManager.Instance.GoToChooseOpponent();
    }
}
