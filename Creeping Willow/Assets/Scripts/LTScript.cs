using UnityEngine;
using System.Collections;

public class LTScript : MonoBehaviour
{
    private GameObject target;
    private float offset;

	
    public void Initialize(GameObject target)
    {
        this.target = target;

        if(target != null)
            offset = target.GetComponent<SpriteRenderer>().bounds.extents.y + GetComponent<SpriteRenderer>().bounds.extents.y;
    }

	void Update ()
    {
        if (target != null)
        {
            GetComponent<SpriteRenderer>().enabled = true;
            transform.position = target.transform.position + new Vector3(0f, -offset);
        }
        else GetComponent<SpriteRenderer>().enabled = false;
	}
}
