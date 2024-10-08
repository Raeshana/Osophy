using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    /// <summary>
    /// When called, transitions to the next scene in 
    /// scene manager
    /// </summary>
    public void GoToNextScene() 
    {
        StartCoroutine(GoToNextSceneRoutine());
    }

    private IEnumerator GoToNextSceneRoutine()
    {
        int current = SceneManager.GetActiveScene().buildIndex;
        int next = current + 1;

        yield return new WaitForSeconds(0.35f);

        SceneManager.LoadScene(next, LoadSceneMode.Single);
    }

    // DEFINE THE FOLLOWING FUNCTIONS ****************************************************************************

    /// <summary>
    /// When called, transitions to the 
    /// celebration scene for getting an answer
    /// right
    /// </summary>
    public void GoToRightScene(){
        // Ensure RightScene is added to the list of scenes in SceneManager
        // Use SceneManager to navigate to this scene
        SceneManager.LoadScene("RightScene");
    }
    
    /// <summary>
    /// When called, transitions to the 
    /// sad scene for getting an answer
    /// wrong
    /// </summary>
    public void GoToWrongScene(){
        // Ensure CorrectScene is added to the list of scenes in SceneManager
        // Use SceneManager to navigate to this scene
        SceneManager.LoadScene("WrongScene");

    }
}
