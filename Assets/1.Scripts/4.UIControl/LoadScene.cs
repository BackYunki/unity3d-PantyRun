using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LoadScene : MonoBehaviour {

    [Tooltip("씬넘버입니다.")]
    public int num = 0;

	// Use this for initialization
	public void OnClick () {
        SceneManager.LoadScene(num);		
	}
}
