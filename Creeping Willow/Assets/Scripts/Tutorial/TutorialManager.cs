using UnityEngine;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;
    
    public int Phase;
    public PossessableTree TreeA, TreeB;
    public TutorialItem[] TutorialItems;

    private bool sent;
    
    void Awake()
    {
        Instance = this;
        Phase = 0;

        UpdateTutorialItems();

        MessageCenter.Instance.RegisterListener(MessageType.PossessorSpawned, HandlePossessorSpawned);
        MessageCenter.Instance.RegisterListener(MessageType.PossessorDestroyed, HandlePossessorDestroyed);
        MessageCenter.Instance.RegisterListener(MessageType.AbilityPlaced, HandleAbilityPlaced);

        sent = false;
    }

    public void AdvancePhase()
    {
        ++Phase;

        UpdateTutorialItems();
    }

    public void DecrementPhase()
    {
        --Phase;

        UpdateTutorialItems();
    }

    void UpdateTutorialItems()
    {
        foreach(TutorialItem item in TutorialItems)
        {
            // Ugly hack for eaten NPC's
            if (item == null) continue;
            
            if (item.ActivePhases.Contains(Phase)) item.gameObject.SetActive(true);
            else item.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Phase == 18 && GlobalGameStateManager.SoulConsumedTimer <= 0 && !sent
        {
            sent = true;
            MessageCenter.Instance.Broadcast(new LevelFinishedMessage(LevelFinishedType.Win, LevelFinishedReason.NumNPCsEaten));
        }
    }

    private void HandlePossessorSpawned(Message message)
    {
        if(Phase == 4 || Phase == 10)
        {
            DecrementPhase();

            return;
        }
        if(Phase == 5 || Phase == 11)
        {
            AdvancePhase();

            return;
        }
        if(Phase == 16)
        {
            TutorialItems[11].gameObject.SetActive(false);

            return;
        }
    }

    private void HandlePossessorDestroyed(Message message)
    {
        if((Phase == 3 || Phase == 9) && !TreeA.Active)
        {
            AdvancePhase();

            return;
        }
        if(Phase == 6 && !TreeA.Active)
        {
            DecrementPhase();

            // If the NPC is out of range, have the player grab again
            if(Vector3.Distance(TreeA.transform.position, TutorialItems[3].transform.position) > 1.5f)
                DecrementPhase();

            return;
        }
        if(Phase == 12 && !TreeA.Active)
        {
            DecrementPhase();

            // If the NPC is out of range, have the player grab again
            if (Vector3.Distance(TreeA.transform.position, TutorialItems[7].transform.position) > 1.5f)
                DecrementPhase();

            return;
        }
    }

    private void HandleAbilityPlaced(Message message)
    {
        AbilityPlacedMessage aMessage = message as AbilityPlacedMessage;
        
        if(Phase == 4)
        {
            if (aMessage.AType == AbilityType.PossessionLure)
            {
                AdvancePhase();

                return;
            }
        }
        if(Phase == 10)
        {
            if(aMessage.AType == AbilityType.PossessionScare)
            {
                AdvancePhase();

                return;
            }
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(20, 20, 500, 500), "Phase: " + Phase);
    }
}
