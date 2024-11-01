using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "OsopherSO", menuName = "ScriptableObjects/Osopher", order = 1)]
public class OsopherSO : ScriptableObject
{
    public string osopherName;
    public string osopherTitle;
    public Sprite osopherImage;
    public VideoClip osopherVideo;
    public QuestionSO[] osopherQuestions;
}
