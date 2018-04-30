using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LanternBehavior : NetworkBehaviour {

    private bool isActive = false;
    public AudioClip toggleSfx;
    private AudioSource audio;
	// Update is called once per frame
    void Start()
    {
        audio = GetComponent<AudioSource>();
    }

	void Update () {
        if (isLocalPlayer && Input.GetKeyDown("e"))
        {
            isActive = !isActive;
            GetComponentInChildren<Light>().enabled = isActive;
            audio.PlayOneShot(toggleSfx);
        }
    }
}
