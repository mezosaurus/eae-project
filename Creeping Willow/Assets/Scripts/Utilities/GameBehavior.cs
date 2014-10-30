using UnityEngine;
using System.Collections;

public class GameBehavior : MonoBehaviour
{
    private void Update()
    {
        // Check to see if paused
        GameUpdate();
    }

    protected virtual void GameUpdate();
}
