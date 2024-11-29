using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Timers;

public class Timer : MonoBehaviour
{
    [Header("Timer Settings ---------")]
    private TMP_Text _timerText; // Text to display remaining time
    [SerializeField] 
    private GameObject _timer; // Timer gameobject (Enable after 8 seconds)
    [SerializeField] 
    private float _remainingTime;  // Remaining time

    [Header("Asnwer Settings ---------")]
    [SerializeField]
    private TMP_Text _answertext; // When time runs out, display if answer is correct
    [SerializeField]
    private AnswerButtons answerButtons; // Script containing whether answer was correct or not

    [Header("Scene Settings ---------")]
    [SerializeField] 
    private SceneController sceneController; // Scene Controller reference

    void Awake() {
        _timerText = GetComponent<TMP_Text>();
    }

    void Start() {
        StartCoroutine(GenerateTimerRoutine());
    }

    private IEnumerator GenerateTimerRoutine() {
        yield return new WaitForSeconds(8);
        _timer.SetActive(true);
    } 

   // Update is called once per frame
    void Update()
    {
        if (_remainingTime > 0) {
            // Countdown
            _remainingTime -= Time.deltaTime;
        }
        else if (_remainingTime < 0) {
            // Display if answer is correct
            _remainingTime = 0;
            if (answerButtons.isCorrectAnswer) {
                _answertext.text = "Correct!";
                StartCoroutine(GoToRightSceneRoutine());
            }  
            else {
                _answertext.text = "Wrong!";
                StartCoroutine(GoToWrongSceneRoutine());
            }
        }

        int minutes = Mathf.FloorToInt(_remainingTime / 60);
        int seconds = Mathf.FloorToInt(_remainingTime % 60);
        _timerText.text = string.Format("{0:00}:{1:00} s", minutes, seconds);
    }

    IEnumerator GoToRightSceneRoutine() {
        yield return new WaitForSeconds(6);
        sceneController.GoToRightScene();
    }

    IEnumerator GoToWrongSceneRoutine() {
        yield return new WaitForSeconds(6);
        sceneController.GoToWrongScene();
    }
}
