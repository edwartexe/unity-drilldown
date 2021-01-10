using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class enemy_unlocker : enemy_parent {

    void Start() {
        gridMaster = (Grid)GameObject.Find("GameMaster").GetComponent(typeof(Grid));
        selector = (selector)GameObject.Find("Selector").GetComponent(typeof(selector));

        headAnimator = null;
        bodyAnimator = null;
        childSprites = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer csprite in childSprites) {
            csprite.enabled = false;
        }

        enemyDone = true;
        thisNode = gridMaster.nodeFromWorldPoint(this.transform.position);
        lastNode = thisNode;



        maxWalkDistance = 0;
        attackRange = 0;
        enemyClass = globals.unlockerName;
        maxHealth = globals.unlockerhealth;
        health = globals.unlockerhealth;
        UnitPower = globals.unlockerpower;
    }



    // Update is called once per frame
    void Update() {
        foreach (SpriteRenderer csprite in childSprites) {
            csprite.enabled = !thisNode.nodeOculto;
        }
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public override IEnumerator enemysturn() {
        enemyDone = false;
        animateBody(true);

        //take relic if at the spot
        if (thisNode.isThereAnItemHere()) {
            relic targetrelic = (relic)thisNode.itemInThisNode.GetComponent<relic>();
            heldRelic = targetrelic;
            targetrelic.onTake(false);
            targetrelic.transform.position = transform.position;
        }

        //is this enemy active
        Debug.Log(this.gameObject.name + " is just a brick");
        yield return null;

        enemyDone = true;
        animateBody(false);
    }
    public override void animateHead(int state) { }
    public override void animateBody(bool state) { }

    public virtual List<Node> listUnlockTargets() {

        List<Node> vecinos = new List<Node>();
        if (thisNode.upNode != null) {
            vecinos.Add(thisNode.upNode);
            if (thisNode.upNode.rightNode != null) { vecinos.Add(thisNode.upNode.rightNode); }
            if (thisNode.upNode.leftNode != null) { vecinos.Add(thisNode.upNode.leftNode); }
        }
        if (thisNode.rightNode != null) { vecinos.Add(thisNode.rightNode); }
        if (thisNode.downNode != null) {
            vecinos.Add(thisNode.downNode);
            if (thisNode.downNode.rightNode != null) { vecinos.Add(thisNode.downNode.rightNode); }
            if (thisNode.downNode.leftNode != null) { vecinos.Add(thisNode.downNode.leftNode); }
        }
        if (thisNode.leftNode != null) { vecinos.Add(thisNode.leftNode); }
        return vecinos;
    }

    public override void getRekt() {
        Debug.Log("lock destroyed");

        GameObject explotion_inst = Instantiate((GameObject)Resources.Load("enemy_explotion"), this.transform.position, Quaternion.identity, gridMaster.groupEnemy);
        Destroy(explotion_inst, 1.0f);

        if (heldRelic != null) {
            heldRelic.transform.position = this.transform.position;
            heldRelic.onReveal();
            heldRelic = null;
            Debug.Log("relic dropped");
        }

        foreach (Node n in listUnlockTargets() ) {
            if (n.tiletype.Contains("locked")) {
                n.tiletype=n.tiletype.Replace("locked", "normal");
                n.updateSprite();
            }
        }

        gridMaster.addDinero(globals.unlockerCoinLoot);
        Destroy(this.gameObject);
    }

    public override void OnDestroy() {
        gridMaster.updateActors();
    }
}
