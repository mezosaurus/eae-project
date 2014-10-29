using UnityEngine;
using System.Collections;

public class Lure : AbilityClass {

	public int lurePower;

	void OnTriggerEnter2D(Collider2D collider){

		MessageCenter.Instance.Broadcast(Message)
	}
}
