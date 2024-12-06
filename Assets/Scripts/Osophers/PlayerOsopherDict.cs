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
    public event EventHandler OnRemoveOsopher;

    private List<string> _osophers = new List<string>();

    /// <summary>
    /// Initializes the gameObject that GameOsopherDict is attached to
    /// </summary>
    void Awake() {
        _gameOsopherDict = gameObject.GetComponent<GameOsopherDict>();
    }

    void Start() {
        // Subscribe to HandleOnAddOsopher
        OnAddOsopher += HandleOnAddOsopher;
        OnRemoveOsopher += HandleOnRemoveOsopher;
    }

    /// <summary>
    /// Inserts a <string, OsopherSO> kvp into player Osopher dict 
    /// If it does not already exist
    /// Does not check if Osopher is valid, wrap in if statement:
    /// if (GameOsopherDict.gameOsopherDict.FindOsopher(result.Text))
    /// </summary>
    /// <param name="osopherName"> Name of Osopher you would like to add </param>
    public void AddOsopher(string osopherName) {
        if (!FindOsopher(osopherName)) {
            osopherDict.Add(osopherName, _gameOsopherDict.GetOsopherSO(osopherName));
            _osophers.Add(osopherName);
            OnAddOsopher?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Removes an Osopher from the player Osopher dict
    /// </summary>
    /// <param name="osopherName"> Name of Osopher you would like to add </param>
    public void RemoveOsopher(string osopherName) {
        if (!FindOsopher(osopherName)) {
            osopherDict.Remove(osopherName);
            if (_osophers.Contains(osopherName))
            {
                _osophers.Remove(osopherName);
            }
            OnRemoveOsopher?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Updates Osopher1, Osopher2, and Osopher3 player metadata
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void HandleOnAddOsopher(object sender, EventArgs e) {
        // Update Player Osophers 
        LobbyManager.Instance.UpdatePlayerOsophers(_osophers[0], _osophers[1], _osophers[2]);
        // Increment Player NumCards
        LobbyManager.Instance.IncrementPlayerNumCards();
    }

    /// <summary>
    /// Updates Osopher1, Osopher2, and Osopher3 player metadata
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void HandleOnRemoveOsopher(object sender, EventArgs e) {
        // Decrement Player NumCards
        LobbyManager.Instance.DecrementPlayerNumCards();
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
        if (osopherDict.ContainsKey(osopherName))
        {
            return osopherDict[osopherName];
        }
        // Return false if the key does not exist
        return false;
    }

    public void UpdateOsopherQuestion(QuestionSO question) {
        osopherQuestion = question;
    }

    // Currently used for testing
    void Update() {
        //Debug.Log(osopherDict["Socrates"].osopherName);
    }
}
