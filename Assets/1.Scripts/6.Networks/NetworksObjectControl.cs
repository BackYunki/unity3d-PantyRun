using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class NetworksObjectControl : NetworkBehaviour {

    public const short Door = 1001;
    public const short PutItem = 1002;
    public const short OpenBox = 1003;
    public const short GetGun = 1004;

    // Use this for initialization
    void Start () {
        NetworkServer.RegisterHandler(Door, DoorOpen);
        NetworkServer.RegisterHandler(PutItem, PickUpItem);
        NetworkServer.RegisterHandler(OpenBox, OpentheBox);
        NetworkServer.RegisterHandler(GetGun, GetAGun);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    [Server]
    private void DoorOpen(NetworkMessage msg)
    {
        OpenBehavior ob = GameObject.Find(msg.ReadMessage<StringMessage>().value).GetComponent<OpenBehavior>();

        ob.DoorOpen();
    }

    [Server]
    private void PickUpItem(NetworkMessage msg)
    {
        PutItem put = GameObject.Find(msg.ReadMessage<StringMessage>().value).GetComponent<PutItem>();
        put.DestroyItem();
    }

    [Server]
    private void OpentheBox(NetworkMessage msg)
    {
        OpenBox bx = GameObject.Find(msg.ReadMessage<StringMessage>().value).GetComponent<OpenBox>();
        bx.OpenTheBox();
    }

    [Server]
    private void GetAGun(NetworkMessage msg)
    {
        OpenBox bx = GameObject.Find(msg.ReadMessage<StringMessage>().value).GetComponent<OpenBox>();
        bx.GetGun();
    }
}
