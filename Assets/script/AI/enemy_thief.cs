using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class enemy_thief : enemy_parent {

    protected int thisGasLoot;
    protected int thisCoinLoot;

    [SerializeField] protected bool stolenLoot;
    [SerializeField] protected int stolenCoin;
    [SerializeField] protected int currentCoin=> thisCoinLoot + stolenCoin;
    protected int stealPotential = 20;
    protected bool stillVisible = true;
    public enemy_thief_nest originNest;

     
    void Start()
    {
        gridMaster = (Grid)GameObject.Find("GameMaster").GetComponent(typeof(Grid));
        selector = (selector)GameObject.Find("Selector").GetComponent(typeof(selector));

        headAnimator = this.transform.Find("tail").gameObject.GetComponent<Animator>();
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
        enemyClass = globals.thiefname;
        maxHealth = globals.thiefhealth;
        health = globals.thiefhealth;
        UnitPower = globals.thiefPower;
        thisGasLoot = globals.thiefGasloot;
        thisCoinLoot = globals.thiefCoinloot;
    }

    

    // Update is called once per frame
    void Update()
    {
        //this.gameObject.GetComponent<SpriteRenderer>().enabled = !thisNode.nodeOculto;
        foreach (SpriteRenderer csprite in childSprites) {
            csprite.enabled = !thisNode.nodeOculto && stillVisible;
        }
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //---------------------------------------------------------------move range ------------------------------------------------------------------------------------------
    

    public override List<Node> listMovementRange() {
        List<Node> visited = new List<Node>();
        List<Node> visiting = new List<Node>();

        visiting.Add(thisNode);
        for (int i = 0; i <= maxWalkDistance; i++) {
            //Debug.Log(this.gameObject.name + " i: " +i+"/"+maxWalkDistance);
            List<Node> unvisited = new List<Node>();

            foreach (Node newNode in visiting) {
                //Debug.Log(this.gameObject.name + " node: " + newNode.gridPoint );
                /*bool pass = true;
                if (newNode.tiletype.Contains("bloqueado") || newNode.tiletype.Contains("bottomless") || newNode.tiletype.Contains("sphinxstatue")) { pass = false; }
                if (newNode.walkable == false) { pass = false; }
                if (newNode.isThereAUnitHere() == true) { pass = false; }*/
                if (newNode.canThiefCross() && !newNode.isThereAUnitHere()) {
                    if (!visited.Contains(newNode.upNode) && newNode.upNode != null) { unvisited.Add(newNode.upNode); }
                    if (!visited.Contains(newNode.rightNode) && newNode.rightNode != null) { unvisited.Add(newNode.rightNode); }
                    if (!visited.Contains(newNode.downNode) && newNode.downNode != null) { unvisited.Add(newNode.downNode); }
                    if (!visited.Contains(newNode.leftNode) && newNode.leftNode != null) { unvisited.Add(newNode.leftNode); }

                    if ((!newNode.isThereAnEnemyHere() || newNode.enemyInThisNode.Equals(this)) && !newNode.tiletype.Contains("locked")) {
                        visited.Add(newNode);
                    }
                }
            }

            visiting = unvisited;
        }
        this.movementRange = visited;
        listAttackNodes();
        return visited;
    }

    public override List<Node> listAttackNodes() {

        List<Node> targetsvisited = new List<Node>();
        List<Node> targetsvisiting = new List<Node>();

        targetsvisiting.AddRange(movementRange);
        for (int i = 0; i <= attackRange; i++) {
            //Debug.Log(this.gameObject.name + " i: " + i + "/" + attackRange);
            List<Node> unvisited = new List<Node>();

            foreach (Node newNode in targetsvisiting) {
                if (newNode.canThiefCross()) {
                    if (!targetsvisited.Contains(newNode.upNode) && newNode.upNode != null) { unvisited.Add(newNode.upNode); }
                    if (!targetsvisited.Contains(newNode.rightNode) && newNode.rightNode != null) { unvisited.Add(newNode.rightNode); }
                    if (!targetsvisited.Contains(newNode.downNode) && newNode.downNode != null) { unvisited.Add(newNode.downNode); }
                    if (!targetsvisited.Contains(newNode.leftNode) && newNode.leftNode != null) { unvisited.Add(newNode.leftNode); }

                    //if (!newNode.isThereAnEnemyHere() || newNode.enemyInThisNode.Equals(this)) { }
                    if ((!newNode.isThereAnEnemyHere() || newNode.enemyInThisNode.enemyClass.Equals(globals.nestName)) && !newNode.tiletype.Contains("locked")) {
                        targetsvisited.Add(newNode);
                    }

                }
            }

            targetsvisiting = unvisited;
        }
        this.attackRangeNodes = targetsvisited;
        return targetsvisited;
    }

    public override IEnumerator enemysturn() {
        enemyDone = false;
        animateHead(1);

        //take relic if at the spot
        //turns out having the roaming enemy hold a relic wasnt a good idea
        /*if (thisNode.isThereAnItemHere()) {
            relic targetrelic = (relic)thisNode.itemInThisNode.GetComponent<relic>();
            heldRelic = targetrelic;
            targetrelic.onTake(false);
            targetrelic.transform.position = transform.position;
        }*/

        //thief walks trough walls
        if (true) {
            unit_parent prey = null;
            unit_HQ HQ = selector.unitHQ_code;
            listMovementRange();
            Debug.Log(this.gameObject.name + ">>>>>>");

            /*if (!thisNode.nodeOculto) {
                selector.camScript.focusedObject = this.gameObject;
            }*/

            //step 1: get target unit
            unit_parent[] weakUnits = GameObject.FindObjectsOfType<unit_parent>(); 
            foreach (unit_parent unit in weakUnits) {
                Debug.Log(this.gameObject.name + " vs " + unit.name);
                if (highLightAttackableNodes(unit.thisNode) &&(unit.health<= UnitPower || unit.attackPower==0) ) {
                    if (prey == null) {
                        prey = unit;
                        Debug.Log(this.gameObject.name + " first prey");
                    } else {
                        if (prey.heldRelic != null) {
                            if ((unit.heldRelic != null) && unit.health < prey.health) {
                                prey = unit;
                                Debug.Log(this.gameObject.name + " new target, weaker, relic");
                            }
                        } else {
                            if (unit.heldRelic != null) {
                                prey = unit;
                                Debug.Log(this.gameObject.name + " new target, relic");
                            } else {
                                if (unit.health < prey.health) {
                                    prey = unit;
                                    Debug.Log(this.gameObject.name + " new target, weaker");
                                } else {
                                    Debug.Log(this.gameObject.name + " no change");
                                }
                            }
                        }

                    }

                } else {
                    Debug.Log(this.gameObject.name + " out of reach, lets raid hq");
                }

            }


            //step 2: make path to target
            if (prey != null) {

                List<Node> attackPath = null;
                attackPath = pathToUnit(prey.thisNode, this.thisNode, 100,true);
                Debug.Log(this.gameObject.name + " path asigned");

                if (attackPath != null && attackPath.Count() > 0) {
                    int i = 0;
                    //step 3: move there
                    while ((i < attackPath.Count) && (i < maxWalkDistance + 1)) {
                        //Debug.Log(this.gameObject.name + " moving to prey[" + attackPath[i].gridPoint + "]");
                        this.thisNode = attackPath[i];
                        if (!thisNode.nodeOculto) {
                            selector.camScript.focusedObject = this.gameObject;
                        } else {
                            selector.camScript.focusedObject = null;
                        }
                        this.updatePosition();
                        //wait a bit between movements
                        float time = 0.2f;
                        float ix = 0.0f;
                        float rate = 4.0f / time;
                        while (ix < 4.0f) {
                            ix += Time.deltaTime * rate;
                            yield return null;
                        }
                        //yield return new WaitForSeconds(1);
                        i++;
                    }
                } else {
                    if (attackPath == null) { Debug.Log(this.gameObject.name + " attackPath null"); } else { Debug.Log(this.gameObject.name + " attackPath count " + attackPath.Count); }

                }

                //step 4: attack
                if (
                    !thisNode.nodeOculto && (
                        (thisNode.upNode != null && thisNode.upNode.Equals(prey.thisNode)) ||
                        (thisNode.leftNode != null && thisNode.leftNode.Equals(prey.thisNode)) ||
                        (thisNode.rightNode != null && thisNode.rightNode.Equals(prey.thisNode)) ||
                        (thisNode.downNode != null && thisNode.downNode.Equals(prey.thisNode))
                    )
                ) {
                    prey.health -= UnitPower;
                    animateHead(2);
                    prey.gameObject.GetComponent<Animator>().Play("unitHitAnimation");
                    if (prey.health <= 0) {
                        prey.getRekt();
                    }
                    yield return new WaitForSeconds(0.5f);
                }

            } else {
                //Step 2 alternative, attack HQ
                if (!stolenLoot) {
                    List<Node> attackPath = null;
                    attackPath = pathToUnit(HQ.thisNode, this.thisNode, 100,true);
                    Debug.Log(this.gameObject.name + " path asigned to hq");

                    if (attackPath != null && attackPath.Count() > 0) {
                        int i = 0;
                        //step 3: move there
                        while ((i < attackPath.Count) && (i < maxWalkDistance + 1)) {
                            this.thisNode = attackPath[i];
                            if (!thisNode.nodeOculto) {
                                selector.camScript.focusedObject = this.gameObject;
                            } else {
                                selector.camScript.focusedObject = null;
                            }
                            this.updatePosition();
                            float time = 0.2f;
                            float ix = 0.0f;
                            float rate = 4.0f / time;
                            while (ix < 4.0f) {
                                ix += Time.deltaTime * rate;
                                yield return null;
                            }
                            i++;
                        }
                    } else {
                        if (attackPath == null) { Debug.Log(this.gameObject.name + " attackPath null"); } else { Debug.Log(this.gameObject.name + " attackPath count " + attackPath.Count); }

                    }

                    
                    //step 4: STEAL
                    if (
                        !thisNode.nodeOculto && (
                            (thisNode.upNode != null && thisNode.upNode.Equals(HQ.thisNode)) ||
                            (thisNode.leftNode != null && thisNode.leftNode.Equals(HQ.thisNode)) ||
                            (thisNode.rightNode != null && thisNode.rightNode.Equals(HQ.thisNode)) ||
                            (thisNode.downNode != null && thisNode.downNode.Equals(HQ.thisNode))
                        )
                    ) {
                        if (steal(HQ)) {
                            yield return new WaitForSeconds(0.5f);
                        }
                    }

                } else {
                    if (originNest!=null) {
                        List<Node> attackPath = null;
                        attackPath = pathToUnit(originNest.thisNode, this.thisNode, 100,false);
                        Debug.Log(this.gameObject.name + " path asigned to nest");

                        if (attackPath != null && attackPath.Count() > 0) {
                            int i = 0;
                            //step 3: move there
                            while ((i < attackPath.Count) && (i < maxWalkDistance + 1)) {
                                this.thisNode = attackPath[i];
                                if (!thisNode.nodeOculto) {
                                    selector.camScript.focusedObject = this.gameObject;
                                } else {
                                    selector.camScript.focusedObject = null;
                                }
                                this.updatePosition();
                                float time = 0.2f;
                                float ix = 0.0f;
                                float rate = 4.0f / time;
                                while (ix < 4.0f) {
                                    ix += Time.deltaTime * rate;
                                    yield return null;
                                }
                                i++;
                            }
                        } else {
                            if (attackPath == null) { Debug.Log(this.gameObject.name + " escape path null"); } else { Debug.Log(this.gameObject.name + " escape path count " + attackPath.Count); }

                        }

                        //step 4-alt: escape
                        if (thisNode.Equals(originNest.thisNode)) {
                            Debug.Log("enemy escaped");
                            if (heldRelic != null) {
                                heldRelic.transform.position = this.transform.position;
                                heldRelic.onReveal();
                                heldRelic = null;
                                Debug.Log("relic dropped");
                            }
                            yield return new WaitForSeconds(0.2f);
                            stillVisible = false;
                            yield return new WaitForSeconds(0.2f);
                            enemyDone = true;
                            yield return null;
                            Destroy(this.gameObject);
                        }

                            
                        

                    }
                    
                }
            
            
            }

        } /*else {
            Debug.Log(this.gameObject.name + " sleeping");
        }*/

        animateHead(0);
        yield return null;
        enemyDone = true;
        selector.camScript.focusedObject = null;
    }

    public bool steal(unit_HQ HQ) {
        if (!thisNode.nodeOculto &&
            (
                (thisNode.upNode != null && thisNode.upNode.Equals(HQ.thisNode)) ||
                (thisNode.leftNode != null && thisNode.leftNode.Equals(HQ.thisNode)) ||
                (thisNode.rightNode != null && thisNode.rightNode.Equals(HQ.thisNode)) ||
                (thisNode.downNode != null && thisNode.downNode.Equals(HQ.thisNode))
            )
        ) {
            int rng = Random.Range(stealPotential/2, stealPotential);
            selector.gridMaster.addDinero(-rng);
            this.stolenCoin = rng;
            this.stolenLoot = true;
            animateTail(stolenLoot);
            animateBody(stolenLoot);
            return true;
        } else { return false; }
    }

    public override void animateHead(int state) {
        //return null;
    }

    public virtual void animateTail(bool state) {
        if (headAnimator != null) {
            headAnimator.SetBool("excited", state);
        }
    }

    public override void animateBody(bool state) {
        if (bodyAnimator != null) {
            bodyAnimator.SetBool("excited", state);
        }
    }


    //----------------------------------------------------- PATH FINDING -----------------------------------------------------------
    //buscar un camino de nodos desde el nodo actual a donde esta el jugador -------------------------------------------------------

    //este metodo hace djikstra desde el target hasta una zona en el espacio de movimiento de este enemigo
    public override List<Node> pathToUnit(Node targetNode, Node startNode, int limit, bool llegarAlVecino) {
        List<DjikstraNodes> unvisited = new List<DjikstraNodes>();
        Stack<Node> path = new Stack<Node>();
        List<Node> returnlist = new List<Node>();

        foreach (Node n in gridMaster.GetComponent<Grid>().grid) {
            DjikstraNodes dj;
            if (n.Equals(targetNode)) {
                dj = new DjikstraNodes(n, 0);
            } else {
                dj = new DjikstraNodes(n, 9999);
            }
            unvisited.Add(dj);
        }

        DjikstraNodes searcher;
        int countYetToVisit = 1;
        int iteration = 0;


        while (countYetToVisit > 0 && iteration < limit) {

            int startIndex = unvisited.FindIndex(x => x.visited == false);
            searcher = unvisited[startIndex];
            foreach (DjikstraNodes dj in unvisited) {
                if (dj.weight < searcher.weight && !dj.visited) {
                    searcher = dj;
                }
            }
            int searcherIndex = unvisited.FindIndex(x => x.Equals(searcher));

            bool inWalkDistance = searcher.node.walkable;
            if (searcher.node.isHQhere()) { inWalkDistance = true; }
            if (searcher.weight == 9999) { inWalkDistance = false; }

            if (inWalkDistance) {
                if (llegarAlVecino) {
                    if (this.movementRange.Contains(searcher.node) && (!searcher.node.isThereAnEnemyHere() || searcher.node.enemyInThisNode.Equals(this)) && !searcher.node.tiletype.Contains("locked")) {
                        //en este if se debe checkear para nodos en los que el enemigo no  puede quedarse pero puede atravezar
                        //ahora que tenemos el nodo disponible mas cercano al target, buscar un camino hasta aquel
                        return pathToNode(searcher.node, startNode, limit);
                    }
                } else {
                    if (this.attackRangeNodes.Contains(searcher.node)) {
                        //ahora que tenemos el nodo disponible mas cercano al target, buscar un camino hasta aquel
                        return pathToNode(searcher.node, startNode, limit);
                    }
                }

                foreach (Node compareThis in searcher.node.getVecinos()) {
                    if (compareThis != null) {
                        int temp = unvisited.FindIndex(x => x.node.Equals(compareThis));
                        if (unvisited[temp].node.canThiefCross()
                        && (searcher.weight + 1 < unvisited[temp].weight)
                        && !unvisited[temp].node.isThereAUnitHere()) {
                            unvisited[temp].weight = searcher.weight + 1;
                            unvisited[temp].prevNode = searcher;
                        }
                    } 
                    
                }
            }

            unvisited[searcherIndex].visited = true;

            countYetToVisit = 0;
            foreach (DjikstraNodes dj in unvisited) {
                if (dj.visited == false) {
                    countYetToVisit++;
                }
            }
            iteration++;
        }

        returnlist = new List<Node>();
        //returnlist.AddRange(path);
        Debug.Log("didnt find a path to enemy");
        return returnlist;

    }

    //djisktra hasta un nodo y devuelve el camino
    public override List<Node> pathToNode(Node targetNode, Node startNode, int limit) {
        List<DjikstraNodes> unvisited = new List<DjikstraNodes>();
        Stack<Node> path = new Stack<Node>();
        List<Node> returnlist = new List<Node>();

        foreach (Node n in gridMaster.GetComponent<Grid>().grid) {
            DjikstraNodes dj;
            if (n.Equals(startNode)) {
                dj = new DjikstraNodes(n, 0);
            } else {
                dj = new DjikstraNodes(n, 9999);
            }
            unvisited.Add(dj);
        }


        DjikstraNodes searcher;
        int countYetToVisit = 1;
        int iteration = 0;

        while (countYetToVisit > 0 && iteration < limit) {

            int startIndex = unvisited.FindIndex(x => x.visited == false);
            searcher = unvisited[startIndex];
            foreach (DjikstraNodes dj in unvisited) {
                if (dj.weight < searcher.weight && !dj.visited) {
                    searcher = dj;
                }
            }
            int searcherIndex = unvisited.FindIndex(x => x.Equals(searcher));

            bool inWalkDistance = searcher.node.walkable;
            if (searcher.weight == 9999) { inWalkDistance = false; }

            if (inWalkDistance) {
                if (searcher.node.Equals(targetNode)) {
                    path.Push(searcher.node);
                    DjikstraNodes backwards = searcher.prevNode;

                    while (backwards != null) {
                        path.Push(backwards.node);
                        backwards = backwards.prevNode;
                    }

                    returnlist = new List<Node>();
                    returnlist.AddRange(path);
                    return returnlist;
                }

                foreach (Node compareThis in searcher.node.getVecinos()) {
                    if (compareThis != null) {
                        /*int temp = unvisited.FindIndex(x => x.node.Equals(compareThis));
                        if ((!unvisited[temp].node.tiletype.Contains("bloqueado") && !unvisited[temp].node.tiletype.Contains("bottomless") && !unvisited[temp].node.tiletype.Contains("sphinxstatue"))
                        && (unvisited[temp].node.walkable)
                        && (searcher.weight + 1 < unvisited[temp].weight)
                        && !unvisited[temp].node.isThereAUnitHere()) {
                            unvisited[temp].weight = searcher.weight + 1;
                            unvisited[temp].prevNode = searcher;
                        }*/
                        int temp = unvisited.FindIndex(x => x.node.Equals(compareThis));
                        if (unvisited[temp].node.canThiefCross()
                        && (searcher.weight + 1 < unvisited[temp].weight)
                        && !unvisited[temp].node.isThereAUnitHere()) {
                            unvisited[temp].weight = searcher.weight + 1;
                            unvisited[temp].prevNode = searcher;
                        }
                    } 
                }
            }

            unvisited[searcherIndex].visited = true;

            countYetToVisit = 0;
            foreach (DjikstraNodes dj in unvisited) {
                if (dj.visited == false) {
                    countYetToVisit++;
                }
            }
            iteration++;
        }

        returnlist = new List<Node>();
        returnlist.AddRange(path);
        Debug.Log("didnt find a path to target");
        return returnlist;

    }


    public void fleeMap() {
        Debug.Log("enemy escaped");
        enemyDone = true;
        if (heldRelic != null) {
            heldRelic.transform.position = this.transform.position;
            heldRelic.onReveal();
            heldRelic = null;
            Debug.Log("relic dropped");
        }
        Destroy(this.gameObject);
    }

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
        //gridMaster.addGas(thisGasLoot);
        gridMaster.addDinero(currentCoin);
        Destroy(this.gameObject);
    }

    public override void OnDestroy() {
        gridMaster.updateActors();
    }
}
