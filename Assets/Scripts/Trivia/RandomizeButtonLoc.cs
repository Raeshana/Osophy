using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeButtonLoc : MonoBehaviour
{
    [SerializeField] 
    private GameObject _rightAnswerButton; // Right answer game object
    [SerializeField] 
    private GameObject _wrongAnswerButton; // Wrong answer game object
    [SerializeField] 
    private Transform[] _buttonLocs; // Transforms of right and wrong answer buttons

    // Start is called before the first frame update
    void Start()
    {        
        StartCoroutine(GenerateButtonRoutine());
    }

    /// <summary>
    /// Generate buttons after 8 seconds
    /// </summary>
    /// <returns></returns>
    private IEnumerator GenerateButtonRoutine() {
        yield return new WaitForSeconds (7);

        int _idx = Random.Range(0, 2); // Generate a random index between 0 and 1

        //Enable button active
        _rightAnswerButton.SetActive(true);
        _wrongAnswerButton.SetActive(true);

        // Assign the right answer button to a location
        _rightAnswerButton.transform.position = _buttonLocs[_idx].position;
        // Debug.Log("Right Answer Button Position: " + _rightAnswerButton.transform.position);
        // Debug.Log("_idx:" + _idx);

        // Assign the wrong answer button to the other location
        _wrongAnswerButton.transform.position = _buttonLocs[1 - _idx].position;
        // Debug.Log("Wrong Answer Button Position: " + _wrongAnswerButton.transform.position);
        // Debug.Log("1-_idx:" +  (1 - _idx));
    }
}
