using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateRoundWinner : MonoBehaviour
{
    /// <summary>
    /// If player is the host, decides the round winner, loads engine scene
    /// If player is not the host, loads engine scene
    /// </summary>
    public void DecideRoundWinnerButton()
    {
        LobbyManager.Instance.DecideRoundWinner();
    }
}
