using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class unit_bomb : unit_parent{

    // Start is called before the first frame update
    void Start() {

        gridMaster = (Grid)GameObject.Find("GameMaster").GetComponent(typeof(Grid));
        selector = (selector)GameObject.Find("Selector").GetComponent(typeof(selector));

        //spriteAnimator = this.gameObject.GetComponentsInChildren<Animator>(); //0=drill 1=threads

        movementAnimator = this.transform.Find("wheels").gameObject.GetComponent<Animator>();
        attackAnimator = this.transform.Find("body").gameObject.GetComponent<Animator>();

        childSprites = GetComponentsInChildren<SpriteRenderer>();

        thisNode = gridMaster.nodeFromWorldPoint(this.transform.position);
        lastNode = thisNode;

        maxWalkDistance = 4;
        unitClass = globals.bombname;
        health = globals.bombhealth;
        maxHealth = globals.bombhealth;
        basecost = globals.action_cost_base_bomb;
        attackPower = 0;
        attackRange = 0;
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
 
    public override bool checkDrillTargets(Node targetNode, bool compareNode) {
        return false;
    }

    public override bool checkAttackTargets(Node targetNode, bool compareNode) {
        return false;
    }

    public override bool checkHoldTargets(Node targetNode, bool compareNode) {
        return false;
    }

    public override bool checkDropTargets(Node targetNode, bool compareNode) {
        return false;
    }
    public override bool checkScanTargets(Node searcher, bool compareNode) {
        return false;
    }

    public override void animateDrill(bool state) {
        if (attackAnimator!=null) {
            attackAnimator.SetBool("explode", state);
        }
    }


    public override void animateThreads(bool state) {
        if (movementAnimator != null) {
            movementAnimator.SetBool("moving", state);
        }
    }

    private void OnDestroy() {
        gridMaster.updateActors();
    }


}
