using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void TextureSynthesisScene()
    {
        SceneManager.LoadScene(1);
    }

    public void TextureModificationScene()
    {
        SceneManager.LoadScene(2);
    }

    public void MenuScene()
    {
        SceneManager.LoadScene(0);
    }
}
