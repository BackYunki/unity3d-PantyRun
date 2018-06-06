using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Clocking : NetworkBehaviour {

    public float clokingTime = 5;

    public void UseItem()
    {
        CmdClokingOn();
    }

    [Command]
    public void CmdClokingOn()
    {
        RpcClokingOn();
    }

    [ClientRpc]
    void RpcClokingOn()
    {
        transform.Find("PlayerObject").gameObject.SetActive(false);
        Invoke("ClokingOff", clokingTime);
    }

    void ClokingOff()
    {
        transform.Find("PlayerObject").gameObject.SetActive(true);
    }

}
