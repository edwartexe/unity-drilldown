using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class unit_drill : unit_parent {

    void Start() {

        gridMaster = (Grid)GameObject.Find("GameMaster").GetComponent(typeof(Grid));
        selector = (selector)GameObject.Find("Selector").GetComponent(typeof(selector));

        //spriteAnimator = this.gameObject.GetComponentsInChildren<Animator>(); //0=drill 1=threads

        attackAnimator = this.transform.Find("body/sprite_drill").gameObject.GetComponent<Animator>();
        movementAnimator = this.transform.Find("sprite_threads").gameObject.GetComponent<Animator>();
        childSprites = GetComponentsInChildren<SpriteRenderer>();

        thisNode = gridMaster.nodeFromWorldPoint(this.transform.position);
        lastNode = thisNode;
        heldRelic = null;

        maxWalkDistance = 3;
        unitClass = globals.drillname;
        health = globals.drillhealth;
        maxHealth= globals.drillhealth;
        attackPower = globals.drillattackstat;
        basecost = globals.action_cost_base_drill;
        attackRange = 1;
    }

    // Update is called once per frame
    void Update(){
        if (isActive) {
            if (alreadyMoved) {
                //this.gameObject.GetComponent<SpriteRenderer>().color = Color.gray;
                foreach (SpriteRenderer csprite in childSprites) {
                    csprite.color = Color.cyan;
                }
            } else {
                //this.gameObject.GetComponent<SpriteRenderer>().color = Color.gray;
                foreach (SpriteRenderer csprite in childSprites) {
                    csprite.color = Color.blue;
                }
            }
        } else {
            foreach (SpriteRenderer csprite in childSprites) {
                csprite.color = Color.gray;
            }

        }

        //animateThreads(selected);
            animateThreads  (selected && ( selector.state_selector == 1 || selector.state_selector == 2) );
            animateDrill    (selected && ( selector.state_selector == 3 || selector.state_selector == 4) );
    }




    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //---------------------------------------------------------------move range ------------------------------------------------------------------------------------------

    /*public override bool checkHoldTargets(Node targetNode, bool compareNode) {
        return false;
    }

    public override bool checkDropTargets(Node targetNode, bool compareNode) {
        return false;
    }*/

    public override bool checkBombTargets(Node targetNode, bool compareNode) {
        return false;
    }
    public override bool checkScanTargets(Node searcher, bool compareNode) {
        return false;
    }

    public override void animateDrill(bool state) {
        if (attackAnimator!=null) {
            attackAnimator.SetBool("drilling", state);
        }
    }

    public override void animateThreads(bool state) {
        if (movementAnimator!=null) {
            movementAnimator.SetBool("moving", state);
        }
    }
    
    private void OnDestroy() {
        gridMaster.updateActors();
    }


}
