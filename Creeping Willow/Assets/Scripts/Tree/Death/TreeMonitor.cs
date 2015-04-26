using UnityEngine;
using System.Collections;

public class TreeMonitor : MonoBehaviour
{
    public GUISkin Skin;
    public GameObject AxeManKillInactive;


    private Camera cam;

    
    // Use this for initialization
    void Start()
    {
        MessageCenter.Instance.RegisterListener(MessageType.PlayerKilled, HandleTreeDeath);

        cam = GetComponent<Camera>();
    }

    private void HandleTreeDeath(Message message)
    {
        PlayerKilledMessage m = message as PlayerKilledMessage;

        //Destroy(m.Tree);
        m.NPC.SetActive(false);

        if (m.Tree.GetComponent<PossessableTree>().Active)
        {

        }
        else
        {
            // Spawn a killing axe man
            GameObject axeMan = (GameObject)Instantiate(AxeManKillInactive, m.Tree.transform.position + new Vector3(-1.14f, 0.091f), Quaternion.identity);

            axeMan.GetComponent<AxeManKillInactiveTree>().Instantiate(m.Tree, m.NPC, HandleCinematicFinished);
            m.Tree.GetComponent<PossessableTree>().AxeMan = axeMan;

            transform.position = new Vector3(m.Tree.transform.position.x, m.Tree.transform.position.y + 0.7f, -9f);
            cam.enabled = true;
        }
    }

    private void HandleCinematicFinished(GameObject cinematic, GameObject actual)
    {
        actual.transform.position = cinematic.transform.position + new Vector3(0.076f, 0.352f);
        actual.SetActive(true);

        Destroy(cinematic);

        cam.enabled = false;

        if(GameObject.FindGameObjectsWithTag("Player").Length == 0)
            MessageCenter.Instance.Broadcast(new LevelFinishedMessage(LevelFinishedType.Loss, LevelFinishedReason.PlayerDied));
    }

    // Update is called once per frame
    void Update()
    {
        // TEMPORARY
        /*GameObject[] trees = GameObject.FindGameObjectsWithTag("Player");

        foreach(EnemyAIControllerWander enemy in GameObject.FindObjectsOfType<EnemyAIControllerWander>())
        {
            GameObject axeMan = enemy.gameObject;
            
            foreach(GameObject tree in trees)
            {
                if(!tree.GetComponent<PossessableTree>().Dead && Vector3.Distance(tree.transform.position, axeMan.transform.position) <= 1f)
                {
                    MessageCenter.Instance.Broadcast(new PlayerKilledMessage(axeMan, tree));
                }
            }
        }*/
    }

    void OnGUI()
    {
        GUI.skin = Skin;

        if(cam.enabled)
        {
            Rect rect = new Rect(cam.pixelRect.x, Screen.height - cam.pixelRect.yMax, cam.pixelWidth, cam.pixelHeight);

            GUI.Box(rect, "");
        }
    }
}
