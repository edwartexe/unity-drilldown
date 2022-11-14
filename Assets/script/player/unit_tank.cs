using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class unit_tank : unit_parent{

    
    public Animator bodyAnimator;
    public Animator gunshot;

    // Start is called before the first frame update
    void Start() {

        gridMaster = (Grid)GameObject.Find("GameMaster").GetComponent(typeof(Grid));
        selector = (selector)GameObject.Find("Selector").GetComponent(typeof(selector));

        //spriteAnimator = this.gameObject.GetComponentsInChildren<Animator>(); //0=drill 1=threads

        movementAnimator = this.transform.Find("legs").gameObject.GetComponent<Animator>();
        bodyAnimator = this.transform.Find("body").gameObject.GetComponent<Animator>();

        attackAnimator = this.transform.Find("body/turret").gameObject.GetComponent<Animator>();
        gunshot = this.transform.Find("body/gunshot").gameObject.GetComponent<Animator>();

        childSprites = GetComponentsInChildren<SpriteRenderer>();

        thisNode = gridMaster.nodeFromWorldPoint(this.transform.position);
        lastNode = thisNode;

        maxWalkDistance = 2;
        unitClass = globals.tankname;
        health = globals.tankhealth;
        maxHealth = globals.tankhealth;
        attackPower = globals.tankattackstat;
        basecost = globals.action_cost_base_tank;
        attackRange = 2;
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
            attackAnimator.SetBool("shooting", state);
        }
        if (gunshot != null) {
            gunshot.SetBool("shooting", state);
        }
    }

    public void stopShooting() {
        if (gunshot != null) {
            gunshot.SetBool("shooting", false);
        }
        
    }

    public override void animateThreads(bool state) {
        if (movementAnimator!=null) {
            movementAnimator.SetBool("moving", state);
        }
        if (bodyAnimator != null) {
            bodyAnimator.SetBool("moving", state);
        }
    }

    private void OnDestroy() {
        gridMaster.updateActors();
    }


}
