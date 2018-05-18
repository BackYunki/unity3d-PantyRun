using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class flaregun : MonoBehaviour {
	
	public Rigidbody flareBullet;
	public Transform barrelEnd;
	public GameObject muzzleParticles;
    public GameObject callAble;
	public AudioClip flareShotSound;
	public AudioClip noAmmoSound;	
	public AudioClip reloadSound;	
	public int bulletSpeed = 2000;
	public int maxSpareRounds = 1;
	public int spareRounds = 0;
	public int currentRound = 0;
    public int heliDelayTime = 2;
	

    
	void Start () {
	
	}
	
	void Update () 
	{
		
		if(Input.GetButtonDown("Fire1") && !GetComponent<Animation>().isPlaying)
		{
			if(currentRound > 0){
				Shoot();
                Invoke("call_Helicopter", heliDelayTime);
			}else{
				GetComponent<Animation>().Play("noAmmo");
				GetComponent<AudioSource>().PlayOneShot(noAmmoSound);
			}
		}
		if(Input.GetKeyDown(KeyCode.R) && !GetComponent<Animation>().isPlaying)
		{
			Reload();
		}
	
	}
	
	void Shoot()
	{
		currentRound--;
		if(currentRound <= 0){
			currentRound = 0;
		}
        
        CmdShoot();

    }

    [Command]
    void CmdShoot() {
        RpcShoot();
    }

    [ClientRpc]
    void RpcShoot()
    {
        GetComponent<Animation>().CrossFade("Shoot");
        GetComponent<AudioSource>().PlayOneShot(flareShotSound);

        Rigidbody bulletInstance;
        bulletInstance = Instantiate(flareBullet, barrelEnd.position, barrelEnd.rotation) as Rigidbody; //INSTANTIATING THE FLARE PROJECTILE

        bulletInstance.AddForce(barrelEnd.forward * bulletSpeed); //ADDING FORWARD FORCE TO THE FLARE PROJECTILE

        Instantiate(muzzleParticles, barrelEnd.position, barrelEnd.rotation);   //INSTANTIATING THE GUN'S MUZZLE SPARKS	
    }

    void Reload()
	{
		if(spareRounds >= 1 && currentRound == 0){
			GetComponent<AudioSource>().PlayOneShot(reloadSound);			
			spareRounds--;
			currentRound++;
			GetComponent<Animation>().CrossFade("Reload");
		}
		
	}


    void call_Helicopter()
    {
        callAble.SetActive(true);
        callAble.GetComponent<loapDown>().enabled = true;
    }
}
