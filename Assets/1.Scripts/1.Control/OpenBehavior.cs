using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class OpenBehavior : NetworkBehaviour
{
    private Animator _animator;
    [SyncVar]
    private bool isOpen = false;
    private bool isEnter = false;
    public AudioClip openSfx;
    public AudioClip closeSfx;
    private new AudioSource audio;
    const short Door = 1001;

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
    

    [ClientCallback]
    private void LateUpdate()
    {
        if (isEnter && Input.GetKeyDown("f"))
        {
            foreach (NetworkClient client in NetworkClient.allClients)
            {
                StringMessage msg = new StringMessage(name);
                client.Send(Door,msg);
            }
        }
    }

    [Server]
    public void DoorOpen()
    {
        isOpen = !isOpen;
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
