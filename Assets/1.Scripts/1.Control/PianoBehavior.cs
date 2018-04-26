using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PianoBehavior : MonoBehaviour {

    public AudioClip pianoSfx;

    private bool isPlay = false;
    private bool isEnter = false;

    void Start()
    {
        GetComponent<AudioSource>().enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown("f") && isEnter)
        {
            isPlay = !isPlay;

            if (isPlay)
            {
                GetComponent<AudioSource>().enabled = true;
                GetComponent<AudioSource>().PlayOneShot(pianoSfx);
            }
            else
            {
                GetComponent<AudioSource>().enabled = false;
            }
        }
        else if (!isEnter)
        {
            GetComponent<AudioSource>().enabled = false;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if ( col.tag == "Player")
        {
            isEnter = true;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if( col.tag == "Player")
        {
            isEnter = false;
        }
    }
}
