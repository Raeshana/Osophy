using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using System;
using TMPro;

public class AnswerButtons : MonoBehaviour
{
    // public event EventHandler OnAnswered;

    [HideInInspector]
    public bool isCorrectAnswer;

    [SerializeField]
    private TMP_Text _answerText;

    void Awake() {
        isCorrectAnswer = false;
    }

    /// <summary>
    /// Triggers the OnNameChanged event when button is clicked
    /// </summary>
    public void CorrectAnswerButton() {
        isCorrectAnswer = true;
        LobbyManager.Instance.UpdatePlayerCorrect("true");
        _answerText.text = "Answered!";
    }

    /// <summary>
    /// Triggers the OnNameChanged event when button is clicked
    /// </summary>
    public void WrongAnswerButton() {
        isCorrectAnswer = false;
        LobbyManager.Instance.UpdatePlayerCorrect("false");
        _answerText.text = "Answered!";
    }

    // private void Start() {
    //     // Subscribe to OnNameChanged event
    //     OnAnswered += PlayerName_OnNameChanged;
    // }

    // /// <summary>
    // /// Calls UpdatePlayerName to ensure changes are made across all clients
    // /// </summary>
    // /// <param name="sender"></param>
    // /// <param name="e"></param>
    // private void PlayerName_OnNameChanged(object sender, EventArgs e) {
    //     LobbyManager.Instance.UpdatePlayerCorrect("true");
    // }

    // private void OnDestroy() {
    //     OnNameChanged -= PlayerName_OnNameChanged;
    // }
}
