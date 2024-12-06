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

    public void GoToEngineScene(){
        // Ensure CorrectScene is added to the list of scenes in SceneManager
        // Use SceneManager to navigate to this scene
        SceneManager.LoadScene("EngineScene");
    }

    public void GoToCreditsScene(){
        SceneManager.LoadScene("Credits", LoadSceneMode.Additive);
    }
    
    public void CloseCreditsScene(){
        SceneManager.UnloadSceneAsync("Credits");
    }
    
    public void GoToMainMenu(){
        SceneManager.LoadScene("MainMenu");
    }
    public void GoToLobby(){
        SceneManager.LoadScene("Lobby");
    }

    public void GoToJoinLobby(){
        SceneManager.LoadScene("JoinLobby");
    }

    public void GoToInstructionsScene(){
        SceneManager.LoadScene("InstructionsScene");
    }

    public void GoToChooseOpponentPlayer(){
        SceneManager.LoadScene("ChooseOpponentPlayer");
    }

    public void GoToChooseOpponentSpectator(){
        SceneManager.LoadScene("ChooseOpponentSpectator");
    }

    public void GoToChooseOpponentScene() {
        LobbyManager.Instance.GoToChooseOpponentScene();
    }

    public void GoToVersusScene(){
        SceneManager.LoadScene("VersusScene");
    }

    public void GoToGameplayQRScanningScene(){
        SceneManager.LoadScene("GameplayQRScanningScene");
    }

    public void GoToSpectatorScene(){
        SceneManager.LoadScene("SpectatorScene");
    }

    public void GoToWaitingForAnswersScene(){
        SceneManager.LoadScene("WaitingForAnswersScene");
    }

    public void GoToEndOfRoundScene(){
        SceneManager.LoadScene("EndOfRoundScene");
    }

    public void GoToPlayerTurnScene(){
        SceneManager.LoadScene("PlayerTurn");
    }

    public void GoToRoundQRScanningScene(){
        SceneManager.LoadScene("RoundScanningScene");
    }

     public void GoToRoundSpectatorScene(){
        SceneManager.LoadScene("RoundSpectatorScene");
    }
}
