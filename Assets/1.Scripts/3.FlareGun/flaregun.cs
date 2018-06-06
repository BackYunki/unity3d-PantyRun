using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class flaregun : NetworkBehaviour {

	public Rigidbody flareBullet;
	public Transform barrelEnd;
    public Animation ani;
	public GameObject muzzleParticles;
    public GameObject callAble;
    public GameObject cameraF;
	public AudioClip flareShotSound;
	public AudioClip noAmmoSound;	
	public AudioClip reloadSound;	
	public int bulletSpeed = 2000;
	public int maxSpareRounds = 1;
	public int spareRounds = 0;
	public int currentRound = 0;
    public int heliDelayTime = 2;

    public bool enable = false;

	void Start () {
        callAble = GameObject.Find("HelicopterParent").transform.Find("Helicopter").gameObject;
	}
	
	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.R) && !ani.isPlaying && enable)
		{
			Reload();
		}
	}

    public void UseItem()
    {
        if (!ani.isPlaying && enable)
        {
            if (currentRound > 0)
            {
                Shoot();
                Invoke("Call_Helicopter", heliDelayTime);
            }
            else
            {
                ani.Play("noAmmo");
                GetComponent<AudioSource>().PlayOneShot(noAmmoSound);
            }
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
        ani.CrossFade("Shoot");
        GetComponent<AudioSource>().PlayOneShot(flareShotSound);

        Rigidbody bulletInstance;
        bulletInstance = Instantiate(flareBullet, barrelEnd.position, barrelEnd.rotation) as Rigidbody; //INSTANTIATING THE FLARE PROJECTILE

        bulletInstance.AddForce(cameraF.transform.forward * bulletSpeed); //ADDING FORWARD FORCE TO THE FLARE PROJECTILE

        Instantiate(muzzleParticles, barrelEnd.position, barrelEnd.rotation);   //INSTANTIATING THE GUN'S MUZZLE SPARKS	
    }

    void Reload()
	{
		if(spareRounds >= 1 && currentRound == 0){
			GetComponent<AudioSource>().PlayOneShot(reloadSound);			
			spareRounds--;
			currentRound++;
            ani.CrossFade("Reload");
		}
		
	}


    void Call_Helicopter()
    {
        callAble.SetActive(true);
        callAble.GetComponent<LoapDown>().enabled = true;
    }
}
