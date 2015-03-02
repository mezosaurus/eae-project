using UnityEngine;
using System.Collections;

public class TutorialBenchNPCController : AIController {

	new public void Start()
	{
		base.Start ();
		AIGenerator.loadNPCWithSkin (gameObject, "oldman_skin", NPCSkinType.OldMan);
	}

	/*
	protected override void GameUpdate () 
	{
		updateNPC ();
	}
	*/

	protected override void alert ()
	{
		// Do nothing
	}

	protected override void panic()
	{
		// Do nothing
	}
	//*/
}
