using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class flareround : NetworkBehaviour {
	private GameObject flaregun;
	private flaregun flare;
	public AudioClip pickupSound;	
    
	void Start () 
	{
		flaregun = GameObject.Find("Flaregun");
		flare = flaregun.GetComponent<flaregun>();
	}
		
	void OnTriggerEnter(Collider other)
	{
		
		if(other.tag == "Player" && flare.spareRounds < flare.maxSpareRounds)
		{
			GetComponent<AudioSource>().PlayOneShot(pickupSound);			
			flare.spareRounds++;
			Destroy(this.gameObject,pickupSound.length);				
		}
		
	}
}

