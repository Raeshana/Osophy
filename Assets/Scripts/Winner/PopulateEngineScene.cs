using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using Unity.VisualScripting.AssemblyQualifiedNameParser;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class PopulateEngineScene : MonoBehaviour
{
    private LobbyManager _lobbyManager; // Reference to Lobby Manager

    [SerializeField]
    private VideoPlayer _videoPlayer; // Reference to Video Player

    [SerializeField]
    private GameOsopherDict gameOsopherDict; // Reference to game osopher dictionary

    [SerializeField]
    private TMP_Text _roundWinnerMessage; // TMP_Text displaying message

    void Awake() {
        // Populate _lobbyManager
        _lobbyManager = GameObject.FindWithTag("LobbyManager").GetComponent<LobbyManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Subscribe to HandleOnUpdateRoundWinner event
        _lobbyManager.OnUpdateRoundWinner  += HandleOnUpdateRoundWinner;
    }

    public void HandleOnUpdateRoundWinner(object sender, LobbyManager.LobbyEventArgs e) {
        // Get Round Winner information
        string _roundWinnerID = e.lobby.Data["RoundWinner"].Value; // Round Winner ID
        Player _roundWinner = LobbyManager.Instance.GetPlayer(e.lobby, _roundWinnerID); // Round Winner Player Object
        string _roundWinnerDebater = _roundWinner.Data["Debater"].Value; // Round Winner Debater
        string _roundWinnerQuestion = _roundWinner.Data["QuestionNum"].Value; // Round Winner QuestionNum
        string _roundWinnerName = _roundWinner.Data["PlayerName"].Value; // Round Winner Name

        // Update background
        OsopherSO osopherSO = gameOsopherDict.GetOsopherSO(_roundWinnerDebater); // Round Winner OsopherSO
        QuestionSO questionSO = osopherSO.osopherQuestions[int.Parse(_roundWinnerQuestion)]; // Round Winner Round QuestionSO
        _videoPlayer.clip = questionSO.engineVideo;
        _videoPlayer.Play();

        // Update Round Winner Message
        _roundWinnerMessage.text = _roundWinnerName;
    } 

    private void OnDestroy() {
        if (_lobbyManager != null) {
            _lobbyManager.OnUpdateRoundWinner -= HandleOnUpdateRoundWinner;
        }
    }
}
