using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OsopherSO", menuName = "ScriptableObjects/Osopher", order = 1)]
public class OsopherSO : ScriptableObject
{
    public string osopherName;
    public string osopherTitle;
    // array of question-answer scriptable objects
}
