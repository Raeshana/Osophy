using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestionSO", menuName = "ScriptableObjects/Question", order = 1)]
public class OsopherSO : ScriptableObject
{  
    public string question;
    public string osopherTitle;
    // array of question-answer scriptable objects
}
