using UnityEngine;
using System.Collections;
//using XInputDotNetPure;

public class BenchPossessable : Possessable {

	bool shaking = false;
	float shakeAmount = 1.0f;
    bool needToSend = false;

	// Update is called once per frame
	protected override void GameUpdate () {
		if(shaking){
			float newX = Random.Range(baseX-.05f, baseX+.05f);
			float newY = Random.Range(baseY-.05f, baseY+.05f);
			shakeAmount -= .1f;
			if(shakeAmount <= 0f){
				shaking = false;
				newX = baseX;
				newY = baseY;
			}
			transform.position = new Vector3(newX, newY);
		}
        else
        {
            //GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
            
            if(needToSend)
            {
                MessageCenter.Instance.Broadcast(new CameraChangeFollowedMessage(GameObject.FindGameObjectWithTag("Player").transform, Vector3.zero));

                needToSend = false;
            }
        }
	}

	protected override void act ()
	{
		shaking = true;
		shakeAmount = 1.0f;
        needToSend = true;
		AbilityPlacedMessage message = new AbilityPlacedMessage (transform.position.x,transform.position.y, AbilityType.PossessionLure);
		MessageCenter.Instance.Broadcast (message);
        //GamePad.SetVibration(PlayerIndex.One, 1f, 1f);
	}

	public override void possess ()
	{
		base.possess ();
	}
}
