using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOsopherDict : MonoBehaviour
{
    public Dictionary<string, OsopherSO> osopherDict = new Dictionary<string, OsopherSO>();

    private GameOsopherDict _gameOsopherDict; 

    void Awake() {
        _gameOsopherDict = GameObject.FindWithTag("OsopherManager").GetComponent<GameOsopherDict>();
    }

    public void AddOsopher(string osopherName) {
        osopherDict.Add(osopherName, _gameOsopherDict.getOsopherSO(osopherName));
    }

    void Update() {
        //Debug.Log(osopherDict["Socrates"].osopherName);
    }
}
