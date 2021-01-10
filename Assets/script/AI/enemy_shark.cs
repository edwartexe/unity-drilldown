using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class enemy_shark : enemy_parent {


    /* public Animator headAnimator;
     public Animator bodyAnimator;
     SpriteRenderer[] childSprites;*/

    public Animator dirtAnimator;
    // Start is called before the first frame update
    void Start()
    {
        gridMaster = (Grid)GameObject.Find("GameMaster").GetComponent(typeof(Grid));
        selector = (selector)GameObject.Find("Selector").GetComponent(typeof(selector));

        headAnimator = this.transform.Find("body").gameObject.GetComponent<Animator>();
        bodyAnimator = this.transform.Find("arm").gameObject.GetComponent<Animator>();
        dirtAnimator = this.transform.Find("dirt").gameObject.GetComponent<Animator>();
        childSprites = GetComponentsInChildren<SpriteRenderer>(); 
        foreach (SpriteRenderer csprite in childSprites) {
            csprite.enabled = false;
        }

        enemyDone = true;
        thisNode = gridMaster.nodeFromWorldPoint(this.transform.position);
        lastNode = thisNode;



        maxWalkDistance = 4;
        attackRange = 1;
        enemyClass = globals.sharkname;
        maxHealth = globals.sharkhealth;
        health = globals.sharkhealth;
        UnitPower = globals.sharkPower;
    }

    

    // Update is called once per frame
    void Update()
    {
        //this.gameObject.GetComponent<SpriteRenderer>().enabled = !thisNode.nodeOculto;
        foreach (SpriteRenderer csprite in childSprites) {
            csprite.enabled = !thisNode.nodeOculto;
        }
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //---------------------------------------------------------------move range ------------------------------------------------------------------------------------------
    
    

    

    public override void animateHead(int state) {
        if (headAnimator != null) {
            headAnimator.SetInteger("bite", state);
        }
    }

    public override void animateBody(bool state) {
        if (bodyAnimator != null) {
            bodyAnimator.SetBool("moving", state);
        }
        if (dirtAnimator != null) {
            dirtAnimator.SetBool("moving", state);
        }
    }


    //----------------------------------------------------- PATH FINDING -----------------------------------------------------------
    //buscar un camino de nodos desde el nodo actual a donde esta el jugador -------------------------------------------------------
    public override void getRekt() {
        Debug.Log("enemy destroyed");
        GameObject explotion_inst = Instantiate((GameObject)Resources.Load("enemy_explotion"), this.transform.position, Quaternion.identity, gridMaster.groupEnemy);
        Destroy(explotion_inst, 1.0f);

        if (heldRelic != null) {
            heldRelic.transform.position = this.transform.position;
            heldRelic.onReveal();
            heldRelic = null;
            Debug.Log("relic dropped");

        }
        //gridMaster.addGas(globals.sharkGasloot);
        gridMaster.addDinero(globals.sharkCoinloot);
        Destroy(this.gameObject);
    }

    public override void OnDestroy() {
        gridMaster.updateActors();
    }
}
