using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulateTriviaScene : MonoBehaviour
{
    // UNCOMMENT AND DEFINE THE FOLLOWING VARIABLES AND HEADERS *************************************************

    // [Header("Question Settings ----------")]
    // [SerializeField] 
    // private ... Declare the question textbox here

    // [Header("Button Settings ----------")]
    // [SerializeField] 
    // private ... Declare the right button here
    // [SerializeField] 
    // private ... Declare the wrong button here

    // [Header("Scene Settings ----------")]
    // [SerializeField]
    // private SceneController _sceneController;

    // [Header("Player Settings ----------")]
    // [SerializeField]
    // private PlayerOsopherDict _playerOsopherDict;

    // TO DO *******************************************************************************************************
    void Start()
    {
        // We will use Socrates for testing purposes

        // Get Osopher
        // Declare a OsopherSO _osopher
        // Populate _osopher with SocratesSO from _playerOsopherDict
        // You may find the GetOsopherSO function useful from the PlayerOsopherDict script

        // Get Question
        // Randomly generate a number x between 0 and 2 inclusive
        // Declare a QuestionSO _question
        // Populate _question with the questionSO of index x under _socrates

        // Populate Question Text
        // Populate the question text with the question text field
        // under QuestionSO under SocratesSO

        // Populate Right Answer Text
        // Populate the right button text with the right answer text field
        // under AnswerSO under _question under _socrates

        // Populate Wrong Answer Text
        // Populate the wrong button text with the wrong answer text field
        // under AnswerSO under _question under _socrates
    }
}
