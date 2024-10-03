using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopulateTriviaScene : MonoBehaviour
{
    // UNCOMMENT AND DEFINE THE FOLLOWING VARIABLES AND HEADERS *************************************************

    [Header("Question Settings ----------")]
    [SerializeField] 
    // private ... Declare the question textbox here
    private TextMeshProUGUI questionText;


    [Header("Button Settings ---------")]
    [SerializeField] 
    // private ... Declare the right button here
    private TextMeshProUGUI rightButtonText;
    [SerializeField] 
    // private ... Declare the wrong button here
    private TextMeshProUGUI wrongButtonText;

    [Header("Scene Settings ----------")]
    [SerializeField]
    private SceneController _sceneController;

    [Header("Player Settings ----------")]
    [SerializeField]
    private PlayerOsopherDict _playerOsopherDict;

    // TO DO *******************************************************************************************************
    void Start()
    {
        // We will use Socrates for testing purposes

        // Get Osopher
        // Declare a OsopherSO _osopher
        // Populate _osopher with SocratesSO from _playerOsopherDict
        // You may find the GetOsopherSO function useful from the PlayerOsopherDict script

        // gets osopher "Socrates" and makes an osopher instance (object)
        // "Socrates" is just for alpha testing
        OsopherSO _osopher;
        _osopher = _playerOsopherDict.GetOsopherSO("Socrates"); 


        // Get Question
        // Randomly generate a number x between 0 and 2 inclusive
        int rand = Random.Range(0, 2);
        // Declare a QuestionSO _question

        // Populate _question with the questionSO of index x under _socrates
        QuestionSO _question;
        _question = _osopher.osopherQuestions[rand];

        // Populate Question Text
        // Populate the question text with the question text field
        // under QuestionSO under SocratesSO
        questionText.text = _question.question;

        // Populate Right Answer Text
        // Populate the right button text with the right answer text field
        // under AnswerSO under _question under _socrates
        rightButtonText.text = _question.rightAnswer.answer;

        // Populate Wrong Answer Text
        // Populate the wrong button text with the wrong answer text field
        // under AnswerSO under _question under _socrates
        wrongButtonText.text = _question.wrongAnswer.answer;

    }
}
