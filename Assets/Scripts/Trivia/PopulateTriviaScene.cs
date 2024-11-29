using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Video;

public class PopulateTriviaScene : MonoBehaviour
{
    // UNCOMMENT AND DEFINE THE FOLLOWING VARIABLES AND HEADERS *************************************************

    [Header("Question Settings ----------")]
    [SerializeField] 
    // private ... Declare the question textbox here
    private TextMeshProUGUI _questionText;

    [Header("Button Settings ---------")]
    [SerializeField] 
    // private ... Declare the right button here
    private TextMeshProUGUI _rightButtonText;
    [SerializeField] 
    // private ... Declare the wrong button here
    private TextMeshProUGUI _wrongButtonText;

    [Header("Scene Settings ----------")]
    [SerializeField]
    private SceneController _sceneController;
    [SerializeField]
    private VideoPlayer _videoPlayer;

    [Header("Player Settings ----------")]
    [SerializeField]
    private PlayerOsopherDict _playerOsopherDict;

    // TO DO *******************************************************************************************************
    void Start()
    {
        // We will use Socrates for testing purposes
        // FOR TESTING:
        // AddOsopher
        // Get Osopher
        // Declare a OsopherSO _osopher
        // Populate _osopher with SocratesSO from _playerOsopherDict
        // You may find the GetOsopherSO function useful from the PlayerOsopherDict script

        // gets osopher "Socrates" and makes an osopher instance (object)
        // "Socrates" is just for alpha testing
        OsopherSO _osopher;
        Debug.Log(PlayerDebater.debater);
        _osopher = _playerOsopherDict.GetOsopherSO(PlayerDebater.debater); 

        // Update background
        _videoPlayer.clip = _osopher.osopherVideo;

        // Get Question
        // Randomly generate a number x between 0 and 2 inclusive
        int rand = Random.Range(0, 2);
        // Declare a QuestionSO _question

        // Populate _question with the questionSO of index x under _socrates
        QuestionSO _question;
        _question = _osopher.osopherQuestions[rand];

        // Update osopher question in player Osopher dict
        _playerOsopherDict.UpdateOsopherQuestion(_question);

        // Populate Question Text
        // Populate the question text with the question text field
        // under QuestionSO under SocratesSO
        _questionText.text = _question.question;
        // questionText.text = "hello there?";
        Debug.Log(_question.question);

        // Populate Right Answer Text
        // Populate the right button text with the right answer text field
        // under AnswerSO under _question under _socrates
        _rightButtonText.text = _question.rightAnswer.answer;
        Debug.Log(_question.rightAnswer.answer);

        // Populate Wrong Answer Text
        // Populate the wrong button text with the wrong answer text field
        // under AnswerSO under _question under _socrates
        _wrongButtonText.text = _question.wrongAnswer.answer;
        Debug.Log(_question.wrongAnswer.answer);

    }
}
