using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/*
 * 1. 페이드 효과
 * 2. 승패별 음악 재생
 * 3. 메인화면 버튼 클릭시 타이틀 신으로 넘어가기
 */
public class ResultBehavior : MonoBehaviour {

    [Tooltip("마스크 패널을 등록하세요.")]
    public Image mask;
    float alpha = 1.0f; //패널의 알파값

    [Tooltip("음악을 등록하세요.")]
    public AudioClip sfx;
    private AudioSource AUDIO;

	void Start () {
        AUDIO = gameObject.GetComponent<AudioSource>();
        AUDIO.PlayOneShot(sfx);
        InvokeRepeating("FadeIn",1,0.06f);	    	
	}

    public void OnClick()
    {
        SceneManager.LoadScene(0);
    }
	
    void FadeIn()
    {
        if ( alpha > 0.1f )
        {
            alpha -= 0.05f;
            mask.color = new Color(0, 0, 0, alpha);
        }
        else
        {
            mask.enabled = false;
        }
    }
}
