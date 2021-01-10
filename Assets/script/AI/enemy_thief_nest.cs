using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy_thief_nest : enemy_parent {

    SpriteRenderer csprite;
    public enemy_thief thiefPrefab;
    public enemy_thief thiefStrongerPrefab;
    public float counter;
    [SerializeField] float increment = 4;
    [SerializeField] float chance =100;

    public bool recovering;
    [SerializeField] int recoverHealth =12;

    void Start() {
        gridMaster = (Grid)GameObject.Find("GameMaster").GetComponent(typeof(Grid));
        selector = (selector)GameObject.Find("Selector").GetComponent(typeof(selector));
        thisNode = gridMaster.nodeFromWorldPoint(this.transform.position);

        maxWalkDistance = 0;
        attackRange = 0;
        enemyClass = globals.nestName;
        maxHealth = globals.nesthealth;
        health = globals.nesthealth;
        UnitPower = globals.nestPower;

        csprite = this.GetComponent<SpriteRenderer>();
        csprite.enabled = !thisNode.nodeOculto;
        counter = Random.Range(0, chance);
        recovering = false;
    }

    public override IEnumerator enemysturn() {
        enemyDone = false;
        if (recovering) {
            health += Random.Range(1, recoverHealth) + (2*gridMaster.relicsaved);
            if (health>=maxHealth) {
                health = maxHealth;
                recovering = false;
                csprite.color = Color.red;
            }
        } else {
            if (counter / chance >= 1) {

                if (!thisNode.isThereAUnitHere()) { //&& (!thisNode.isThereAnEnemyHere() || thisNode.enemyInThisNode.Equals(this)) 
                                                    /*if (posibleEnemies.Length>=1) {
                                                        spawnEnemy(posibleEnemies[0]);
                                                    }*/

                    spawnEnemy(thiefPrefab);
                    counter = Random.Range(0, chance / 2);
                }
            } else {
                counter += Random.Range(increment, increment * 4 ) + (2*gridMaster.relicsaved);
            }
        }
        yield return null;
        enemyDone = true;
    }

    private void Update() {
        csprite.enabled = !thisNode.nodeOculto;
    }

    public void spawnEnemy(enemy_thief enemyPrefab) {
        enemy_thief new_enemy = new enemy_thief(); 
        float rng = Random.RandomRange(0.0f,1.0f);
        float progress = ((float)gridMaster.relicsaved+1) / (float)gridMaster.relicounter;
        Debug.Log("rng "+ rng+" vs "+ progress);
        if ((gridMaster.relicsaved>0) && (rng< progress)) {
            new_enemy = Instantiate(thiefStrongerPrefab, new Vector3(thisNode.gridPoint.x, thisNode.gridPoint.y, 0), Quaternion.identity, gridMaster.groupEnemy);
        } else {
            new_enemy = Instantiate(enemyPrefab, new Vector3(thisNode.gridPoint.x, thisNode.gridPoint.y, 0), Quaternion.identity, gridMaster.groupEnemy);
        }
        new_enemy.alreadyMoved = true;
        new_enemy.thisNode = thisNode;
        new_enemy.originNest = this;
        gridMaster.updateActors();
       
    }

    public override bool okToAttack() {
        return !recovering;
    }

    public override void getRekt() {
        Debug.Log("nest controlled");
        if (heldRelic != null) {
            heldRelic.transform.position = this.transform.position;
            heldRelic.onReveal();
            heldRelic = null;
            Debug.Log("relic dropped");
        }
        if (!recovering) {
            //gridMaster.addGas(globals.nestGasLoot);
            gridMaster.addDinero(globals.nestCoinLoot);
        }
        recovering = true;
        health = 0;
        csprite.color = Color.magenta;
        //Destroy(this.gameObject);
    }

    public override void OnDestroy() {
        gridMaster.updateActors();
    }
}
