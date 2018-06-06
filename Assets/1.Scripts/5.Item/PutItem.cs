using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class PutItem : NetworkBehaviour
{
    public AudioClip pickupSound;
    string itemName;
    bool enable = false;
    Inventory inventory;
    short putItem;

    private void Start()
    {
       
        itemName = gameObject.name;
        itemName = itemName.Split(' ')[0];
        putItem = NetworksObjectControl.PutItem;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
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
            foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (player.GetComponent<OFBehavior>().isLocalPlayer)
                {
                    inventory = player.GetComponent<Inventory>();
                }
            }
            int i=0;
            while (i<inventory.item.Length)
            {
                if (itemName.Equals(inventory.item[i]))
                {
                    inventory.PutItem(i);
                    break;
                }
                i++;
            }
            GameObject.Find("Canvas").transform.Find("ItemText").gameObject.SetActive(false);
            GetComponent<AudioSource>().PlayOneShot(pickupSound);

            foreach (NetworkClient client in NetworkClient.allClients)
            {
                StringMessage msg = new StringMessage(name);
                client.Send(putItem, msg);
            }
        
        }
    }

    [Server]
    public void DestroyItem()
    {
        Destroy(this.gameObject);
    }
}
