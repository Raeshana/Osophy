using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOsopherDict : MonoBehaviour
{
    [SerializeField]
    private OsopherSO[] _osopherSOArr;

    public Dictionary<string, OsopherSO> osopherDict = new Dictionary<string, OsopherSO>();

    void Awake() {
        // Populate Osopher dictionary using Osopher Array
        foreach (OsopherSO _osopher in _osopherSOArr) {
            osopherDict.Add(_osopher.osopherName, _osopher);
        }
    }

    public OsopherSO getOsopherSO(string osopherName) {
        return osopherDict[osopherName];
    }

    void Update() {
        //Debug.Log(osopherDict["Socrates"].osopherName);
    }
}
