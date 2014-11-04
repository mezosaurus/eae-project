using UnityEngine;
using System.Collections;

public class GameBehavior : MonoBehaviour
{
    private void Update()
    {
        // Check to see if paused
        GameUpdate();

		if( Input.GetButtonDown("Start") )
		{
			Application.Quit();
		}
    }

    protected virtual void GameUpdate() { }
}
