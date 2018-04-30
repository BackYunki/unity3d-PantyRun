using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class OpenBehavior : NetworkBehaviour
{
    private Animator _animator;
    private bool isOpen = false;
    private bool isEnter = false;
    public AudioClip openSfx;
    public AudioClip closeSfx;
    private new AudioSource audio;

    // Use this for initialization
    void Start()
    {
        _animator = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            isEnter = true;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag == "Player")
        {
            isEnter = false;
        }
    }

    void Update()
    {
        if (isEnter && Input.GetKeyDown("f"))
        {
            isOpen = !isOpen;   // 누를 때마다 상태를 바꾸는 거당
            if (isOpen)
            {
                _animator.SetBool("isOpen", true);
                audio.PlayOneShot(openSfx);
            }
            else
            {
                _animator.SetBool("isOpen", false);
                audio.PlayOneShot(closeSfx);
            }
        }
    }
}
