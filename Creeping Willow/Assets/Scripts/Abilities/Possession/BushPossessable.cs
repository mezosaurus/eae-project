using UnityEngine;
using System.Collections;

public class BushPossessable : PossessableItem
{
	/******** SOUNDS OF THE BUSH *********/
	public AudioClip[] bushLureSounds;
	public AudioClip[] bushScareSounds;
	// Lure vars
	public float lureCooldownSeconds = 2;
	public float lureTimeleft = 0;
	public bool luring = false;
	// Scare vars
	public float scaredCooldownSeconds = 6;
	public float scareTimeLeft = 0;
	public bool scaring = false;

    protected override void Start()
    {
        base.Start();
    }


    // Update is called once per frame
    protected override void GameUpdate()
    {
        base.GameUpdate();
		if (luring)
		{
			Debug.Log ("lure time left = " + lureTimeleft);
			lureTimeleft -= Time.deltaTime;
			if (lureTimeleft <= 0)
			{
				luring = false;
			}
		}
		if (scaring)
		{
			Debug.Log("scare time left = " + scareTimeLeft);
			scareTimeLeft -= Time.deltaTime;
			if (scareTimeLeft <= 0)
			{
				scaring = false;
			}
		}
    }

    protected override void lure()
    {
        //base.lure();
        //blinking = true;
		float clipLength = 0;
		if (!luring)
		{
			AudioClip lure = (AudioClip)bushLureSounds[Random.Range (0, bushLureSounds.Length)];
			clipLength = lure.length;
			audio.PlayOneShot (lure, 1.0f);
	        Animator anim = gameObject.GetComponent<Animator>();
	        anim.SetTrigger("Lure");
	        AbilityPlacedMessage message = new AbilityPlacedMessage(transform.position.x, transform.position.y, AbilityType.PossessionLure);
	        MessageCenter.Instance.Broadcast(message);
		}
		luring = true;
		if (clipLength > 0)
			lureCooldownSeconds = clipLength;
		lureTimeleft = lureCooldownSeconds;
        //anim.SetBool("Lure", false);
    }

    protected override void scare()
    {
        //base.scare();
        //shaking = true;
		float clipLength = 0;
		if (!scaring)
		{
			AudioClip scare = (AudioClip)bushScareSounds[Random.Range (0, bushScareSounds.Length)];
			clipLength = scare.length;
			audio.PlayOneShot (scare, 1.0f);
	        Animator anim = gameObject.GetComponent<Animator>();
	        anim.SetTrigger("Lure");
	        AbilityPlacedMessage message = new AbilityPlacedMessage(transform.position.x, transform.position.y, AbilityType.PossessionScare);
	        MessageCenter.Instance.Broadcast(message);
		}
		scaring = true;
		if (clipLength > 0)
			scaredCooldownSeconds = clipLength;
		scareTimeLeft = scaredCooldownSeconds;
        //GamePad.SetVibration(PlayerIndex.One, 1f, 1f);
    }

    public override void possess()
    {
        Animator anim = gameObject.GetComponent<Animator>();
        anim.SetBool("Exorcise", false);
        anim.SetBool("Possess", true);
        anim.enabled = true;
        base.possess();
    }

    public override void exorcise()
    {
        Animator anim = gameObject.GetComponent<Animator>();
        anim.SetBool("Possess", false);
        anim.SetBool("Exorcise", true);
        Invoke("baseExorcise", .5f);
        //Invoke(base.possess());
    }

    public void baseExorcise(){
        base.exorcise();
    }
}
