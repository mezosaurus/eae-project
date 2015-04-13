using UnityEngine;
using System.Collections;

public class FlashingIndicator : MonoBehaviour
{
    private float buttonScale, buttonScaleDirection;

	void Start ()
    {
        buttonScale = 1f;
        buttonScaleDirection = 1f;
	}
	
	void Update ()
    {
        buttonScale += (Time.deltaTime * buttonScaleDirection * 1f);

        if (buttonScale > 1.15f)
        {
            buttonScale = 1.15f;
            buttonScaleDirection = -1f;
        }

        if (buttonScale < 0.85f)
        {
            buttonScale = 0.85f;
            buttonScaleDirection = 1f;
        }

        transform.localScale = new Vector3(buttonScale, buttonScale, 1f);
	}
}
