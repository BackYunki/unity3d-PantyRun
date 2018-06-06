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
    bool boxOpen = false;
    Inventory inventory;
    short openbox;
    short getgun;
    [SyncVar]
    bool isOpen = false;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        openbox = NetworksObjectControl.OpenBox;
        getgun = NetworksObjectControl.GetGun;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && other.GetComponent<OFBehavior>().isLocalPlayer)
        {
            inventory = other.GetComponent<Inventory>();
            GameObject.Find("Canvas").transform.Find("ItemText").gameObject.SetActive(true);
            boxOpen = inventory.HasItem((int)Inventory.Item.key);
            enable = true;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && other.GetComponent<OFBehavior>().isLocalPlayer)
        {
            GameObject.Find("Canvas").transform.Find("ItemText").gameObject.SetActive(false);
            enable = false;
            boxOpen = false;
        }

    }

    private void Update()
    {
        if (Input.GetKeyDown("f") && enable)
        {
            if (isOpen)
            {
                foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
                {
                    if (player.GetComponent<OFBehavior>().isLocalPlayer)
                    {
                        inventory = player.GetComponent<Inventory>();
                    }
                }
                inventory.PutItem((int)Inventory.Item.flaregun);

                GameObject.Find("Canvas").transform.Find("ItemText").gameObject.SetActive(false);
                GetComponent<AudioSource>().PlayOneShot(pickupSound);

                foreach (NetworkClient client in NetworkClient.allClients)
                {
                    StringMessage msg = new StringMessage(name);
                    client.Send(getgun, msg);
                }
            }
            else if(boxOpen)
            {
                inventory.UseItme();
                foreach (NetworkClient client in NetworkClient.allClients)
                {
                    StringMessage msg = new StringMessage(name);
                    client.Send(openbox, msg);
                }
                
            }
            
        }
    }

    public void UseItem()
    {
        if (enable)
            boxOpen = true;
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
