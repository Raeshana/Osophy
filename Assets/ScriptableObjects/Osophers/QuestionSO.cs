using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "QuestionSO", menuName = "ScriptableObjects/Question", order = 1)]
public class QuestionSO : ScriptableObject
{  
    public string question;
    public AnswerSO rightAnswer;
    public AnswerSO wrongAnswer;
    public VideoClip engineVideo;
}
