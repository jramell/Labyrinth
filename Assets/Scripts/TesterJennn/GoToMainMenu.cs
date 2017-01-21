using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GoToMainMenu : MonoBehaviour {

public void GoToMainMenuScene()
    {
       SceneManager.LoadScene("MenúPrincipal");
    }
}
