using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PutItem : MonoBehaviour {
    public AudioClip pickupSound;
    string itemName;
    bool enable = false;
    Inventory inventory;

    private void Start()
    {
        inventory = GameObject.Find("Canvas").GetComponent<Inventory>();
        itemName = gameObject.name;
        itemName = itemName.Split(' ')[0];
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
            int i=0;
            while (i<20)
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
            Destroy(this.gameObject);
        }
    }
}
