using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This class loads different scenes.
/// </summary>
public class SceneLoader : MonoBehaviour
{
    /// <summary>
    /// Loads the Texture Syntehsis Scene.
    /// </summary>
    public void TextureSynthesisScene()
    {
        SceneManager.LoadScene(1);
    }

    /// <summary>
    /// Loads the Texture Modification Scene.
    /// </summary>
    public void TextureModificationScene()
    {
        SceneManager.LoadScene(2);
    }

    /// <summary>
    /// Loads the Main Menu Scene.
    /// </summary>
    public void MenuScene()
    {
        SceneManager.LoadScene(0);
    }
}
