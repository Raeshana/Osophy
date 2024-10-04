using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOsopherDict : MonoBehaviour
{
    [SerializeField]
    private OsopherSO[] _osopherSOArr;

    public static Dictionary<string, OsopherSO> gameOsopherDict = new Dictionary<string, OsopherSO>();

    /// <summary>
    /// Populates Osopher dict using Osopher Array
    /// </summary>
    void Awake() {
        foreach (OsopherSO _osopher in _osopherSOArr) {
            gameOsopherDict.Add(_osopher.osopherName, _osopher);
            //Debug.Log(_osopher.osopherName);
        }
    }

    /// <summary>
    /// Uses key osopherName to lookup an Osopher in game Osopher dict
    /// Retuns the corresponding value OsopherSO
    /// </summary>
    /// <param name="osopherName"> Name of Osopher you would like to lookup </param>
    /// <returns> OsopherSO if osopherName is found </returns>
    public OsopherSO GetOsopherSO(string osopherName) {
        return gameOsopherDict[osopherName];
    }

    /// <summary>
    /// Uses key osopherName to lookup an Osopher in game Osopher dict
    /// Returns bool if Osopher was found or not
    /// </summary>
    /// <param name="osopherName"> Name of Osopher you would like to lookup </param>
    /// <returns> true if osopherName exists in player Osopher dict, 
    /// false otherwise </returns>
    public bool FindOsopher(string osopherName) {
        return gameOsopherDict[osopherName];
    }

    void Update() {
        //Debug.Log(osopherDict["Socrates"].osopherName);
    }
}
