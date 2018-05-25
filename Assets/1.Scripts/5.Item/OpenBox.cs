using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class OpenBox : NetworkBehaviour
{
    private Animator _animator;
    public AudioClip pickupSound;
    bool enable = false;
    Inventory inventory;
    short openbox;
    short getgun;
    [SyncVar]
    bool isOpen = false;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        inventory = GameObject.Find("Canvas").GetComponent<Inventory>();
        openbox = NetworksObjectControl.OpenBox;
        getgun = NetworksObjectControl.GetGun;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            GameObject.Find("Canvas").transform.Find("ItemText").gameObject.SetActive(true);
            enable = true;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            GameObject.Find("Canvas").transform.Find("ItemText").gameObject.SetActive(false);
            enable = false;
        }

    }

    private void Update()
    {
        if (Input.GetKeyDown("f") && enable)
        {
            if (isOpen)
            {

                inventory.PutItem((int)Inventory.Item.flaregun);

                GameObject.Find("Canvas").transform.Find("ItemText").gameObject.SetActive(false);
                GetComponent<AudioSource>().PlayOneShot(pickupSound);

                foreach (NetworkClient client in NetworkClient.allClients)
                {
                    StringMessage msg = new StringMessage(name);
                    client.Send(getgun, msg);
                }
            }
            else
            {
                foreach (NetworkClient client in NetworkClient.allClients)
                {
                    StringMessage msg = new StringMessage(name);
                    client.Send(openbox, msg);
                }
            }
            
        }
    }

    [Server]
    public void OpenTheBox()
    {
        _animator.SetBool("isOpen", true);
        isOpen = true;
    }

    [Server]
    public void GetGun()
    {
        GetComponent<BoxCollider>().enabled = false;
        enable = false;
        Destroy(this.transform.Find("Bottom").transform.Find("Gun").gameObject);
        
    }
}
