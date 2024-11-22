using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerOsopherDict : MonoBehaviour
{
    public static Dictionary<string, OsopherSO> osopherDict = new Dictionary<string, OsopherSO>();
    public static QuestionSO osopherQuestion;

    private GameOsopherDict _gameOsopherDict; 

    public event EventHandler OnAddOsopher;

    List<string> osophers = new List<string>();

    /// <summary>
    /// Initializes the gameObject that GameOsopherDict is attached to
    /// </summary>
    void Awake() {
        _gameOsopherDict = gameObject.GetComponent<GameOsopherDict>();
    }

    void Start() {
        OnAddOsopher += HandleOnAddOsopher;
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
            osophers.Add(osopherName);
        // }
    }

    public void UpdatePlayerOsophers() {
        OnAddOsopher?.Invoke(this, EventArgs.Empty);
    }

    private void HandleOnAddOsopher(object sender, EventArgs e) {
        LobbyManager.Instance.UpdatePlayerOsophers(osophers[0], osophers[1], osophers[2]);
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
