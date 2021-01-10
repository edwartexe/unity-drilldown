using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class relic : item_parent {
    public int relicID;
    public float speedup =10;
    private bool droprewards = false;

    private SpriteRenderer spriteR;
    private ParticleSystem particleS;

    // Start is called before the first frame update
    void Start()
    {

        gridMaster = (Grid)GameObject.Find("GameMaster").GetComponent(typeof(Grid));
        selector = (selector)GameObject.Find("Selector").GetComponent(typeof(selector));

        spriteR= this.GetComponent<SpriteRenderer>();
        setsprite(relicID);
        particleS = this.GetComponentInChildren<ParticleSystem>();
       

        thisNode = gridMaster.nodeFromWorldPoint(this.transform.position);
        lastNode = thisNode;

        if (!thisNode.nodeOculto) {
            state = 1;
            spriteR.enabled = true;
            particleS.Play();
        } else {
            state = 0;
            spriteR.enabled = false;
        }
    }

    public void setsprite(int spriteID) {
        if (spriteID<=5 && 1<=spriteID) {
            switch (spriteID) {
                case 1: this.spriteR.sprite = gridMaster.minesprites2[3];  break;
                case 2: this.spriteR.sprite = gridMaster.minesprites2[4];  break;
                case 3: this.spriteR.sprite = gridMaster.minesprites2[5];  break;
                case 4: this.spriteR.sprite = gridMaster.minesprites2[6];  break;
                case 5: this.spriteR.sprite = gridMaster.minesprites2[7];  break;
            }
        } else {
            if (globals.Mod(spriteID,2)==0) {
                this.spriteR.sprite = gridMaster.minesprites2[1];
            } else {
                this.spriteR.sprite = gridMaster.minesprites2[2];
            }
        }

    }

    public override void onHide() {
        spriteR.enabled = false;
        particleS.Stop();
        state = 0;
        lastNode = thisNode;
        thisNode = gridMaster.nodeFromWorldPoint(this.transform.position);
    }

    public override void onReveal() {
        spriteR.enabled = true;
        particleS.Play();
        state = 1;
        if (!droprewards) {
            droprewards = true;
            //gridMaster.addGas(globals.relicGasloot);
            gridMaster.addDinero(globals.relicCoinloot);
        }
        lastNode = thisNode;
        thisNode = gridMaster.nodeFromWorldPoint(this.transform.position);
    }

    public override void onTake(bool visiblesparks) {
        spriteR.enabled = false;
        if (visiblesparks) {
            particleS.Play();
        }
        state = 2;
        lastNode = thisNode;
        thisNode = gridMaster.nodeFromWorldPoint(this.transform.position);
    }



    public override void onSave() {
        //save the relic
        spriteR.enabled = false;
        particleS.Stop();
        state = 3;
        lastNode = thisNode;
        thisNode = gridMaster.nodeFromWorldPoint(this.transform.position);
        gridMaster.relicsaved++;
        //gridMaster.addGas(globals.relicSaveGasloot);
        gridMaster.addDinero(globals.relicSaveCoinloot);
        gridMaster.checkRelics();

        //speed up nests
        //allNests
        foreach (enemy_thief_nest oneNest in gridMaster.allNests) {
            oneNest.counter += speedup * gridMaster.relicsaved;
        }
    }

}
