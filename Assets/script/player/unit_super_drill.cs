using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class unit_super_drill : unit_parent {

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
        unitClass = globals.superDrillname;
        health = globals.superDrillhealth;
        maxHealth= globals.superDrillhealth;
        attackPower = globals.superDrillattackstat;
        basecost = globals.action_cost_base_superDrill;
        attackRange = 3;
    }

    // Update is called once per frame
    void Update(){
        if (isActive) {
            if (alreadyMoved) {
                //this.gameObject.GetComponent<SpriteRenderer>().color = Color.gray;
                foreach (SpriteRenderer csprite in childSprites)
                {
                    csprite.color = Color.Lerp(Color.cyan, Color.yellow, 0.5f);
                }
            } else {
                //this.gameObject.GetComponent<SpriteRenderer>().color = Color.gray;
                foreach (SpriteRenderer csprite in childSprites)
                {
                    csprite.color = Color.yellow;
                }
            }
        } else {
            foreach (SpriteRenderer csprite in childSprites)
            {
                csprite.color = Color.Lerp(Color.gray, Color.yellow, 0.5f);
            }

        }

        //animateThreads(selected);
            animateThreads  (selected && ( selector.state_selector == 1 || selector.state_selector == 2) );
            animateDrill    (selected && ( selector.state_selector == 3 || selector.state_selector == 4) );
    }


    public override bool checkDrillTargets(Node targetNode, bool compareNode)
    {
        List<Node> vecinos = new List<Node>();

        int i = 1;
        Node tempNode = nextNode;
        while (i <= attackRange && tempNode != null && tempNode.canUnitDig())
        {
            if (tempNode.upNode != null && tempNode.upNode.canUnitDig())
            { vecinos.Add(tempNode.upNode); }
            tempNode = tempNode.upNode;
            i = i + 1;
        }

        i = 1;
        tempNode = nextNode;
        while (i <= attackRange && tempNode != null && tempNode.canUnitDig())
        {
            if (tempNode.rightNode != null && tempNode.rightNode.canUnitDig())
            { vecinos.Add(tempNode.rightNode);  }
            tempNode = tempNode.rightNode;
            i = i + 1;
        }

        i = 1;
        tempNode = nextNode;
        while (i <= attackRange && tempNode != null && tempNode.canUnitDig())
        {
            if (tempNode.downNode != null && tempNode.downNode.canUnitDig())
            { vecinos.Add(tempNode.downNode); }
            tempNode = tempNode.downNode;
            i = i + 1;
        }

        i = 1;
        tempNode = nextNode;
        while (i <= attackRange && tempNode != null && tempNode.canUnitDig())
        {
            if (tempNode.leftNode != null && tempNode.leftNode.canUnitDig() ) 
            { vecinos.Add(tempNode.leftNode); }
            tempNode = tempNode.leftNode;
            i = i + 1;
        }

        if (compareNode) {
            if (vecinos.Contains(targetNode) && targetNode.nodeOculto){
                return true;
            }
        } else{
            foreach (Node nn in vecinos) {
                if (nn.nodeOculto){
                    return true;
                }
            }
        }


        return false;
    }


    public override List<Node> listAttackNodes()
    {
        List<Node> vecinos = new List<Node>();

        int i = 1;
        Node tempNode = nextNode;
        while (i <= attackRange && tempNode != null && tempNode.canUnitAttack())
        {
            if ((tempNode.upNode != null) && tempNode.upNode.canUnitAttack()) 
            { vecinos.Add(tempNode.upNode); }
            tempNode = tempNode.upNode;
            i = i + 1;
        }

        i = 1;
        tempNode = nextNode;
        while (i <= attackRange && tempNode != null && tempNode.canUnitAttack())
        {
            if ((tempNode.rightNode != null) && tempNode.rightNode.canUnitAttack()) 
            { vecinos.Add(tempNode.rightNode); }
            tempNode = tempNode.rightNode;
            i = i + 1;
        }

        i = 1;
        tempNode = nextNode;
        while (i <= attackRange && tempNode != null && tempNode.canUnitAttack())
        {
            if ((tempNode.downNode != null) && tempNode.downNode.canUnitAttack()) 
            { vecinos.Add(tempNode.downNode); }
            tempNode = tempNode.downNode;
            i = i + 1;
        }

        i = 1;
        tempNode = nextNode;
        while (i <= attackRange && tempNode != null && tempNode.canUnitAttack())
        {
            if ((tempNode.leftNode != null) && tempNode.leftNode.canUnitAttack()) 
            { vecinos.Add(tempNode.leftNode); }
            tempNode = tempNode.leftNode;
            i = i + 1;
        }
        return vecinos;
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
