using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This class is for UI buttons to have functions
/// </summary>
public class MenuActions : MonoBehaviour
{
    /// <summary>
    /// Restarts the Level
    /// </summary>
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// This quits the game
    /// </summary>
    public void Quit()
    {
        Application.Quit();
    }

    /// <summary>
    /// This loads the specified scene
    /// </summary>
    public void LoadScene(int index)
    {
        SceneManager.LoadScene(index);
    }
}
