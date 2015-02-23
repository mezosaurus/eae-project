using UnityEngine;
using System.Collections;

public class LTScript : MonoBehaviour
{
    private GameObject target;
    private Vector3 offset;

	
    public void Initialize(GameObject target)
    {
        this.target = target;

		if (target != null && GlobalGameStateManager.NPCData.ContainsKey(target.GetComponent<AIController>().SkinType))
            offset = GlobalGameStateManager.NPCData[target.GetComponent<AIController>().SkinType].LTOffset;
    }

	void Update ()
    {
        if (target != null)
        {
            GetComponent<SpriteRenderer>().enabled = true;
            transform.position = target.transform.position + offset;
        }
        else GetComponent<SpriteRenderer>().enabled = false;
	}
}
