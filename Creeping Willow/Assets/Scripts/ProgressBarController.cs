using UnityEngine;
using System.Collections;

public class ProgressBarController : MonoBehaviour
{
    public Sprite[] Sprites;
    
    
    // Use this for initialization
    void Start()
    {
        Sprites = new Sprite[101];

        Sprites[0] = Resources.Load<Sprite>("Textures/CircularProgressBar/CircleProgressEmpty");

        for (int i = 1; i <= 101; i++) Sprites[i] = Resources.Load<Sprite>("Textures/CircularProgressBar/CircleProgress" + i);
    }
}
