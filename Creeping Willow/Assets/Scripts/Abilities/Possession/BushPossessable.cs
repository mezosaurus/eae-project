using UnityEngine;
using System.Collections;

public class BushPossessable : PossessableItem
{

    protected override void Start()
    {
        base.Start();
        hintOffset = new Vector2(.25f, .85f);
    }


    // Update is called once per frame
    protected override void GameUpdate()
    {
        base.GameUpdate();
    }

    protected override void lure()
    {
        //base.lure();
        //blinking = true;
        Animator anim = gameObject.GetComponent<Animator>();
        anim.SetTrigger("Lure");
        AbilityPlacedMessage message = new AbilityPlacedMessage(transform.position.x, transform.position.y, AbilityType.PossessionLure);
        MessageCenter.Instance.Broadcast(message);
        //anim.SetBool("Lure", false);
    }

    protected override void scare()
    {
        //base.scare();
        //shaking = true;
        Animator anim = gameObject.GetComponent<Animator>();
        anim.SetTrigger("Lure");
        AbilityPlacedMessage message = new AbilityPlacedMessage(transform.position.x, transform.position.y, AbilityType.PossessionScare);
        MessageCenter.Instance.Broadcast(message);
        //GamePad.SetVibration(PlayerIndex.One, 1f, 1f);
    }

    public override void possess()
    {
        Animator anim = gameObject.GetComponent<Animator>();
        anim.SetBool("Exorcise", false);
        anim.SetBool("Possess", true);
        anim.enabled = true;
        base.possess();
    }

    public override void exorcise()
    {
        Animator anim = gameObject.GetComponent<Animator>();
        anim.SetBool("Possess", false);
        anim.SetBool("Exorcise", true);
        Invoke("baseExorcise", .5f);
        //Invoke(base.possess());
    }

    public void baseExorcise(){
        base.exorcise();
    }
}
