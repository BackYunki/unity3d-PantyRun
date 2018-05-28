using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class NetworkObjectMessage : MessageBase {

    public GameObject obj;
    public string msg = null;
    public int int_msg;

    public NetworkObjectMessage() {}

    public NetworkObjectMessage(GameObject obj)
    {
        this.obj = obj;
    }

    public NetworkObjectMessage(GameObject obj, string msg)
    {
        this.obj = obj;
        this.msg = msg;
    }

    public NetworkObjectMessage(GameObject obj, int msg)
    {
        this.obj = obj;
        this.int_msg = msg;
    }
}
