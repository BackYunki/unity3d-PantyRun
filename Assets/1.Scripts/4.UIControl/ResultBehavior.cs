using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/*
 * 만든이: 백윤기
 * 캔버스에 있는 UI 컴포넌트가 자신을 페이드인 하거나 페이드 아웃하는데 사용됩니다.
 * 1. img_Mask 파일에 마스킹시킬 검정색 이미지를 등록하세요.
 */

public class ResultBehavior : MonoBehaviour {

    [Tooltip("페이드인에 사용할 검정색 이미지를 준비해주세요.")]
    public Image img_Mask;

    [Tooltip("사용할 음악을 넣어주세요.")]
    public AudioClip audio_Clip;
    private AudioSource audio;

    float fade_in = 0.0f;
    float fade_out = 1.0f;
    float fades = 1.0f;

    bool is_fade_in = false;

    void Start()
    {
        audio = gameObject.GetComponent<AudioSource>();
        audio.PlayOneShot(audio_Clip);
        InvokeRepeating("fade_In", 1, 0.1f);
    }

    private void fade_In()
    {
        if (fades > fade_in && !is_fade_in)
        {
            fades -= 0.1f;
            img_Mask.color = new Color(0, 0, 0, fades);
        }
        else if (fades <= 0.0f)
        {
            is_fade_in = true;
            // 버튼을 눌러야 하기 때문에 마스킹을 중지한다.
            img_Mask.enabled = false;
        }
    }
}