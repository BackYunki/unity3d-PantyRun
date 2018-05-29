using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public int sceneNumber = 0;

    public void ClickStart()
    {
        SceneManager.LoadScene(sceneNumber);
    }
}
