using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeButtonLoc : MonoBehaviour
{

    [SerializeField] private GameObject _rightAnswerButton;
    [SerializeField] private GameObject _wrongAnswerButton;
    [SerializeField] private Transform[] _buttonLocs;

    // Start is called before the first frame update
    void Start()
    {        
        private int _idx = Random.Range(0, 2);   // Generate a random index between 0 and 1

        // Assign the right answer button to a location
        _rightAnswerButton.transform.position = _buttonLocs[_idx].position;

        // Assign the wrong answer button to the other location
        _wrongAnswerButton.transform.position = _buttonLocs[1 - _idx].position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
