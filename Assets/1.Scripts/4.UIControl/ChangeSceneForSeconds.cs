using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneForSeconds : MonoBehaviour {
    [Tooltip("대기시간")]
    public float waitTime = 5.0f;
    [Tooltip("씬넘버")]
    public int sceneNumber = 0;

    void Start()
    {
        InvokeRepeating("SceneChange", 5, 1);
    }

    void SceneChange()
    {
        SceneManager.LoadScene(sceneNumber);
    }
}
