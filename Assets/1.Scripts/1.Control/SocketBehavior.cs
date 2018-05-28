using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class SocketBehavior : NetworkBehaviour {
    public GameObject[] Items;
    int index=0;
    short changeItem;

    private void Start()
    {
        Debug.Log("start");
        changeItem = NetworksObjectControl.ChangeItem;
        Debug.Log("here");

        
        GameObject.Find("Canvas").GetComponent<Inventory>().SetSocket(this);
    }

    public void ItemView(int index)
    {
        ChangeItem(index);

        /*
        foreach (NetworkClient client in NetworkClient.allClients)
        {
            NetworkObjectMessage msg = new NetworkObjectMessage(gameObject, index);
            client.Send(changeItem, msg);
        }
        */
    }

    [Command]
    public void ChangeItem(int index)
    {
        HoldOnItem(index);
    }

    [ClientRpc]
    public void HoldOnItem(int index)
    {
        Items[this.index].SetActive(false);
        Items[index].SetActive(true);
        this.index = index;
    }
    /*
    [Server]
    public void HoldOnItem(int index)
    {
        Items[this.index].SetActive(false);
        Items[index].SetActive(true);
        this.index = index;
    }
    */
}
