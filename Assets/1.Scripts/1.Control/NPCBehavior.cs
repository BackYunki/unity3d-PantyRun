using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehavior : MonoBehaviour {

    public AudioClip message;
    private bool talk = false;

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player" && !talk)
        {
            talk = true;
            GetComponent<AudioSource>().PlayOneShot(message);
        }
    }
}
