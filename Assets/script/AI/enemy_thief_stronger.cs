using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class enemy_thief_stronger : enemy_thief {

    //int stealPotential = 45;
     
    void Start()
    {
        gridMaster = (Grid)GameObject.Find("GameMaster").GetComponent(typeof(Grid));
        selector = (selector)GameObject.Find("Selector").GetComponent(typeof(selector));

        headAnimator = null;
        bodyAnimator = this.transform.Find("body").gameObject.GetComponent<Animator>();
        childSprites = GetComponentsInChildren<SpriteRenderer>(); 
        foreach (SpriteRenderer csprite in childSprites) {
            csprite.enabled = false;
        }

        enemyDone = true;
        thisNode = gridMaster.nodeFromWorldPoint(this.transform.position);
        lastNode = thisNode;

        stolenLoot = false;
        stolenCoin = 0;
        maxWalkDistance = 3;
        attackRange = 1;
        enemyClass = globals.thief2name;
        maxHealth = globals.thief2health;
        health = globals.thief2health;
        UnitPower = globals.thief2Power;
        thisGasLoot = globals.thief2Gasloot;
        thisCoinLoot = globals.thief2Coinloot;
    }

    

    // Update is called once per frame
    void Update()
    {
        foreach (SpriteRenderer csprite in childSprites) {
            csprite.enabled = !thisNode.nodeOculto && stillVisible;
        }
    }

    public override void animateTail(bool state) {
        if (bodyAnimator != null) {
            bodyAnimator.SetBool("excited", state);
        }
    }

    public override void animateBody(bool state) {}

    public override void animateHead(int state) {
        if (bodyAnimator != null) {
            bodyAnimator.SetInteger("state", state);
        }
    }

}
