using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

public class ZombieBehavior : NetworkBehaviour {

    private Transform playerTr;
    private NavMeshAgent agent;

	// Use this for initialization
	void Start () {
        agent = GetComponent<NavMeshAgent>();
    }
    void Update()
    {
        StartCoroutine(GetPlayerPos());
    }
    IEnumerator GetPlayerPos()
    {
        while(playerTr == null)
        {
            yield return new WaitForSeconds(5.0f);
            playerTr = GameObject.FindWithTag("Player").transform;
            if (playerTr != null)
            {
                break;
            }
            continue;
        }

        agent.destination = playerTr.position;
        yield return new WaitForSeconds(0.2f);
    }
}
