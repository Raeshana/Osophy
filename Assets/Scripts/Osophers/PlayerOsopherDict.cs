using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOsopherDict : MonoBehaviour
{
    public static Dictionary<string, OsopherSO> osopherDict = new Dictionary<string, OsopherSO>();
    public static QuestionSO osopherQuestion;

    private GameOsopherDict _gameOsopherDict; 

    /// <summary>
    /// Initializes the gameObject that GameOsopherDict is attached to
    /// </summary>
    void Awake() {
        _gameOsopherDict = gameObject.GetComponent<GameOsopherDict>();
    }

    /// <summary>
    /// Inserts a <string, OsopherSO> kvp into player Osopher dict 
    /// If it does not already exist
    /// Does not check if Osopher is valid, wrap in if statement:
    /// if (GameOsopherDict.gameOsopherDict.FindOsopher(result.Text))
    /// </summary>
    /// <param name="osopherName"> Name of Osopher you would like to add </param>
    public void AddOsopher(string osopherName) {
        // if (!FindOsopher(osopherName)) {
            osopherDict.Add(osopherName, _gameOsopherDict.GetOsopherSO(osopherName));
        // }
    }

    /// <summary>
    /// Prints a list of all Osophers in player Osopher dict
    /// to the terminal
    /// </summary>
    public void PrintOsophers() {
         foreach (KeyValuePair<string, OsopherSO> kvp in osopherDict)
        {
            Debug.Log("Key: " + kvp.Key);
        }
    }

    /// <summary>
    /// Uses key osopherName to lookup an Osopher in player Osopher dict
    /// Retuns the corresponding value OsopherSO
    /// </summary>
    /// <param name="osopherName"> Name of Osopher you would like to lookup </param>
    /// <returns> OsopherSO if osopherName is found </returns>
    public OsopherSO GetOsopherSO(string osopherName) {
        return osopherDict[osopherName];
    }

    /// <summary>
    /// Uses key osopherName to lookup an Osopher in player Osopher dict
    /// Returns bool if Osopher was found or not
    /// </summary>
    /// <param name="osopherName"> Name of Osopher you would like to lookup </param>
    /// <returns> true if osopherName exists in player Osopher dict, 
    /// false otherwise </returns>
    public bool FindOsopher(string osopherName) {
        return osopherDict[osopherName];
    }

    public void UpdateOsopherQuestion(QuestionSO question) {
        osopherQuestion = question;
    }

    // Currently used for testing
    void Update() {
        //Debug.Log(osopherDict["Socrates"].osopherName);
    }
}
