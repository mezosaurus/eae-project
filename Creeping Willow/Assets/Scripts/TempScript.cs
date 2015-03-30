using UnityEngine;
using System.Collections;

public class TempScript : MonoBehaviour
{
    public GameObject Tree, AxeMan;

	// Use this for initialization
	void Start ()
    {
        AxeMan.GetComponent<AxeManKillActiveTree>().Instantiate(Tree, null);
        Tree.GetComponent<PossessableTree>().ChangeState("AxeManMinigameWaitForChop", null);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
