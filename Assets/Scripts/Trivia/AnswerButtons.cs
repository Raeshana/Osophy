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

    [SerializeField] 
    private float _remainingTime;  

    void Awake() {
        isCorrectAnswer = false;
    }

    void Update() {
        if (_remainingTime > 0) {
            _remainingTime -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Triggers the OnNameChanged event when button is clicked
    /// </summary>
    public void CorrectAnswerButton() {
        // Update answer to false
        isCorrectAnswer = true;
        LobbyManager.Instance.UpdatePlayerCorrect("true");

        // Give player feedback that they answered
        _answerText.text = "Answered!";

        // Update answer time
        UpdatePlayerAnswerTime();
    }

    /// <summary>
    /// Triggers the OnNameChanged event when button is clicked
    /// </summary>
    public void WrongAnswerButton() {
        // Update answer to false
        isCorrectAnswer = false;
        LobbyManager.Instance.UpdatePlayerCorrect("false");
        
        // Give player feedback that they answered
        _answerText.text = "Answered!";

        // Update answer time
        UpdatePlayerAnswerTime();
    }

    private void UpdatePlayerAnswerTime() {
        string _answerTime = _remainingTime.ToString();
        LobbyManager.Instance.UpdatePlayerAnswerTime(_answerTime); 
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
