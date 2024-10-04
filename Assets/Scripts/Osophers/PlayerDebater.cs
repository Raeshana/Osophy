using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDebater : MonoBehaviour
{
    public static string debater;

    public void AssignDebater(string osopherName) {
        debater = osopherName;
    }

    public void Update() {
        //Debug.Log(debater);
    }
}
