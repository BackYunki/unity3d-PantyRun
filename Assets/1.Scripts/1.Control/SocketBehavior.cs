using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SocketBehavior : NetworkBehaviour {
    public GameObject[] Items;
    int index=0;

    private void Start()
    {
        GameObject.Find("Canvas").GetComponent<Inventory>().SetSocket(this);
    }

    public void ItemView(int index)
    {
        Items[this.index].SetActive(false);
        Items[index].SetActive(true);
        this.index = index;
    }
}
