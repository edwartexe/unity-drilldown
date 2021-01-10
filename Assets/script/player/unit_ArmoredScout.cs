using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class unit_ArmoredScout : unit_parent {

    // Start is called before the first frame update
    void Start() {

        gridMaster = (Grid)GameObject.Find("GameMaster").GetComponent(typeof(Grid));
        selector = (selector)GameObject.Find("Selector").GetComponent(typeof(selector));

        //spriteAnimator = this.gameObject.GetComponentsInChildren<Animator>(); //0=drill 1=threads

        movementAnimator = this.transform.Find("wheels").gameObject.GetComponent<Animator>();
        attackAnimator = this.transform.Find("body/arm").gameObject.GetComponent<Animator>();

        childSprites = GetComponentsInChildren<SpriteRenderer>();

        thisNode = gridMaster.nodeFromWorldPoint(this.transform.position);
        lastNode = thisNode;

        maxWalkDistance = 5;
        unitClass = globals.armoredScoutname;
        health = globals.armoredScouthealth;
        maxHealth = globals.armoredScouthealth;
        basecost = globals.action_cost_base_armoredscout;
        attackPower = globals.armoredScout_attackstat;
        attackRange = 1;
    }



    // Update is called once per frame
    void Update() {
        if (isActive) {
            if (alreadyMoved) {
                foreach (SpriteRenderer csprite in childSprites) {
                    csprite.color = Color.Lerp(Color.cyan,Color.yellow,0.5f);
                }
            } else {
                foreach (SpriteRenderer csprite in childSprites) {
                    csprite.color = Color.yellow;
                }
            }
        } else {
            foreach (SpriteRenderer csprite in childSprites) {
                csprite.color = Color.Lerp(Color.gray, Color.yellow, 0.5f);
            }

        }
        
        animateThreads(selected && (selector.state_selector == 1 || selector.state_selector == 2));
        animateDrill(selected && (selector.state_selector == 3 || selector.state_selector == 4));



    }




    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //---------------------------------------------------------------move range ------------------------------------------------------------------------------------------

    public override bool checkDrillTargets(Node targetNode, bool compareNode) {
        return false;
    }

    public override bool checkBombTargets(Node targetNode, bool compareNode) {
        return false;
    }


    public override void animateDrill(bool state) {
        if (attackAnimator != null) {
            attackAnimator.SetBool("grab", state);
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
