using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulateEndOfRound : MonoBehaviour
{
    [Header("Winner Settings ---------")]
    [SerializeField]
    private string[] _winnerInstructions;

    [Header("Loser Settings ---------")]
    [SerializeField]
    private string[] _loserInstructions;

    [Header("Spectator Settings ---------")]
    [SerializeField]
    private string _spectatorInstructions;

    // Start is called before the first frame update
    void Start()
    {
        
    }
}
