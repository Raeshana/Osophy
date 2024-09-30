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
        int current = SceneManager.GetActiveScene().buildIndex;
        int next = current + 1;
        SceneManager.LoadScene(next, LoadSceneMode.Single);
    }
}
