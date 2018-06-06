using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class SocketBehavior : NetworkBehaviour {
    public GameObject[] Items;
    int index=0;
    //short changeItem;

    private void Start()
    {
        //changeItem = NetworksObjectControl.ChangeItem;
        GetComponent<Inventory>().SetSocket(this);
    }

    public void ItemView(int index)
    {
        CmdHoldOnItem(index);
        /*
        foreach (NetworkClient client in NetworkClient.allClients)
        {
            NetworkObjectMessage msg = new NetworkObjectMessage(gameObject, index);
            client.Send(changeItem, msg);
        }
        */
    }

    [Command]
    public void CmdItemUse()
    {
        RpcItemUse();
    }

    [ClientRpc]
    public void RpcItemUse()
    {
        Items[index].SetActive(false);
    }

    [Command]
    public void CmdHoldOnItem(int index)
    {
        RpcHoldOnItem(index);
    }

    [ClientRpc]
    public void RpcHoldOnItem(int index)
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
