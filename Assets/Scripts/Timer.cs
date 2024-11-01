using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Timers;

public class Timer : MonoBehaviour
{
    private TMP_Text _timer;
    [SerializeField] private float _remainingTime; 
    [SerializeField] private SceneController sceneController;

    void Awake() {
        _timer = GetComponent<TMP_Text>();
    }

   // Update is called once per frame
    void Update()
    {
        if (_remainingTime > 0) {
            _remainingTime -= Time.deltaTime;
        }
        else if (_remainingTime < 0) {
            _remainingTime = 0;
            sceneController.GoToWrongScene();
        }

        int minutes = Mathf.FloorToInt(_remainingTime / 60);
        int seconds = Mathf.FloorToInt(_remainingTime % 60);
        _timer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
