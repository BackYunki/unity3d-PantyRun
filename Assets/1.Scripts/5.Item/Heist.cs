using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Heist : NetworkBehaviour
{
    public float heistTime = 5;

    public void UseItem()
    {
        CmdHeist();
    }

    [Command]
    void CmdHeist()
    {
        RpcHeist();
    }

    [ClientRpc]
    void RpcHeist()
    {
        MoveBehaviour move = GetComponent<MoveBehaviour>();
        move.walkSpeed = 1.0f;
        move.runSpeed = 2.0f;
        move.sprintSpeed = 4.0f;
        Invoke("DisableHeist", heistTime);
    }

    void DisableHeist()
    {
        MoveBehaviour move = GetComponent<MoveBehaviour>();
        move.walkSpeed = 0.2f;
        move.runSpeed = 1.0f;
        move.sprintSpeed = 2.0f;
    }
}
