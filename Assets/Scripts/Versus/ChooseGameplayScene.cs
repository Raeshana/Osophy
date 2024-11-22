using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseGameplayScene : MonoBehaviour
{
    public void ClickButton() {
        LobbyManager.Instance.GoToChooseGameplay();
    }
}
