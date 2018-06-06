using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LanternBehavior : NetworkBehaviour
{

    private bool isActive = false;
    public AudioClip toggleSfx;
    private new AudioSource audio;
    // Update is called once per frame
    void Start()
    {
        audio = this.gameObject.GetComponent<AudioSource>();
    }

    void Update()
    {
        if (isLocalPlayer && Input.GetKeyDown("e"))
        {

            audio.PlayOneShot(toggleSfx);
            CmdControlLantern();

        }
    }

    [Command]
    public void CmdControlLantern()
    {
        RpcControlLantern();
    }

    [ClientRpc]
    public void RpcControlLantern()
    {
        SetLanter();
    }

    void SetLanter()
    {
        isActive = !isActive;
        GetComponentsInChildren<Light>()[0].enabled = isActive;
        GetComponentsInChildren<Light>()[1].enabled = isActive;
    }
}