using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class enemy_guardian : enemy_parent {

    public Animator mainAnimator;


    // Start is called before the first frame update
    void Start()
    {
        gridMaster = (Grid)GameObject.Find("GameMaster").GetComponent(typeof(Grid));
        selector = (selector)GameObject.Find("Selector").GetComponent(typeof(selector));

        mainAnimator = this.transform.Find("main").gameObject.GetComponent<Animator>();
        headAnimator = this.transform.Find("counter").gameObject.GetComponent<Animator>();
        bodyAnimator = null;
        childSprites = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer csprite in childSprites) {
            csprite.enabled = false;
        }

        enemyDone = true;
        thisNode = gridMaster.nodeFromWorldPoint(this.transform.position);
        lastNode = thisNode;



        maxWalkDistance = 3;
        attackRange = 1;
        enemyClass = globals.guardianName;
        maxHealth = globals.guardianhealth;
        health = globals.guardianhealth;
        UnitPower = globals.guardianPower;
        StartCoroutine("wakeUpAnimation");
    }

    

    // Update is called once per frame
    void Update()
    {
        //this.gameObject.GetComponent<SpriteRenderer>().enabled = !thisNode.nodeOculto;
        foreach (SpriteRenderer csprite in childSprites) {
            csprite.enabled = !thisNode.nodeOculto;
            csprite.color = Color.magenta;
        }
    }


    public override IEnumerator enemysturn() {
        enemyDone = false;
            //normal enemy ai
            //take relic if at the spot
            if (thisNode.isThereAnItemHere()) {
                relic targetrelic = (relic)thisNode.itemInThisNode.GetComponent<relic>();
                heldRelic = targetrelic;
                targetrelic.onTake(false);
                targetrelic.transform.position = transform.position;
            }

            //is this enemy active
            unit_parent prey = null;
            listMovementRange();
            Debug.Log(this.gameObject.name + ">>>>>>");
            selector.camScript.focusedObject = this.gameObject; //NEW

            //step 1: get target unit
            //obtener lista de objetivos
            List<unit_parent> allUnits = new List<unit_parent>();
            foreach (GameObject unit in gridMaster.units) {
                allUnits.Add((unit_parent)unit.GetComponent(typeof(unit_parent)));
            }
            //restablecer aggro
            foreach (unit_parent upa in allUnits) {
                upa.aggro = 0;
            }
            if (allUnits.Count >= 1) {
                //añadir aggro segun criterio
                allUnits.OrderBy(item => item.health).FirstOrDefault().aggro++;
                foreach (unit_parent upa in allUnits) {
                    if (upa.heldRelic != null) {
                        upa.aggro += 10;
                    }

                    if (upa.health < this.UnitPower) {
                        upa.aggro += 2;
                    }
                    if (upa.health == upa.maxHealth) {
                        upa.aggro += 1;
                    }

                    if (highLightAttackableNodes(upa.thisNode)) {
                        upa.aggro += 4;
                    }
                }
                //el de mas aggro es el objetivo
                //prey = allUnits.LastOrDefault();
                prey = allUnits.OrderByDescending(item => item.aggro).FirstOrDefault();
            }


            //step 2: make path to target
            if (prey != null) {

                List<Node> attackPath = null;
                attackPath = pathToUnit(prey.thisNode, this.thisNode, 100, true);
                Debug.Log(this.gameObject.name + " path asigned");

                if (attackPath != null && attackPath.Count() > 0) {
                    int i = 0;
                    //step 3: move there
                    while ((i < attackPath.Count) && (i < maxWalkDistance + 1)) {
                        //Debug.Log(this.gameObject.name + " moving to prey[" + attackPath[i].gridPoint + "]");
                        animateMain("sphinx_walk");
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
                        (thisNode.upNode != null && thisNode.upNode.Equals(prey.thisNode)) ||
                        (thisNode.leftNode != null && thisNode.leftNode.Equals(prey.thisNode)) ||
                        (thisNode.rightNode != null && thisNode.rightNode.Equals(prey.thisNode)) ||
                        (thisNode.downNode != null && thisNode.downNode.Equals(prey.thisNode))
                    
                ) {

                    selector.camScript.focusedObject = this.gameObject;
                    if (thisNode.nodeOculto) { thisNode.revealNode(); }
                    prey.health -= UnitPower;
                    prey.gameObject.GetComponent<Animator>().Play("unitHitAnimation");
                    if (prey.health <= 0) {
                        Node preynode = prey.thisNode;
                        prey.getRekt();
                        preynode.burryNode();
                    }
                    animateMain("sphinx_attack");
                    yield return new WaitForSeconds(0.5f);
                    selector.camScript.focusedObject = null;
                } else {
                    // como la esfinge prioriza al que tenga una reliquia tiende a ignorar enemigos en la via, asi que se le da un target secundario
                    //prey is out of range but attack something else if there is
                    unit_parent secondprey =null;
                    foreach (Node nn in thisNode.getVecinos()) {
                        if (nn.isThereAUnitHere()) {
                            if (secondprey == null) {
                                secondprey = nn.unitInThisNode;
                            } else {
                                if (nn.unitInThisNode.health < secondprey.health) {
                                    secondprey = nn.unitInThisNode;
                                }
                                
                            }
                        }
                    }
                    if (secondprey!=null) {
                        if (thisNode.nodeOculto) { thisNode.revealNode(); }
                        secondprey.health -= UnitPower;
                        secondprey.gameObject.GetComponent<Animator>().Play("unitHitAnimation");
                        if (secondprey.health <= 0) {
                            Node preynode = secondprey.thisNode;
                            secondprey.getRekt();
                            preynode.burryNode();
                        }
                        yield return new WaitForSeconds(0.5f);
                    }
                }
                animateMain("sphinx_ok");

            }
        
        yield return null;
        enemyDone = true;
        selector.camScript.focusedObject = null;
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
                if (newNode.tiletype.Contains("bloqueado") || newNode.tiletype.Contains("bottomless")) { pass = false; }
                if (newNode.walkable == false) { pass = false; }
                if (newNode.isThereAUnitHere() == true) { pass = false; }*/
                if (newNode.canSphinxCross() && !newNode.isThereAUnitHere()) {
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
                if (newNode.canSphinxCross()) {
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

    
    public override void animateHead(int state) {
        if (headAnimator != null) {
            switch (state) {
                case 2: headAnimator.Play("count3to2"); break;
                case 1: headAnimator.Play("count2to1"); break;
                case 0: StartCoroutine("wakeUpAnimation"); break;
            }
            
        }
    }

    public override void animateBody(bool state) {
        if (bodyAnimator != null) {
            bodyAnimator.SetBool("moving", state);
        }
    }

    public void animateMain(string animation) {
        if (mainAnimator != null) {
            mainAnimator.Play(animation);
        }
    }

    public IEnumerator wakeUpAnimation() {
        selector.camScript.focusedObject = this.gameObject;
        selector.actionLocked = true;

        mainAnimator.Play("sphinx_wakeup");
        yield return new WaitForSeconds(1.5f);

        selector.camScript.focusedObject = null;
        selector.actionLocked = false;
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
                        if (unvisited[temp].node.canSphinxCross()
                        && (searcher.weight + 1 < unvisited[temp].weight)){
                            if (!unvisited[temp].node.isThereAUnitHere()) {
                                unvisited[temp].weight = searcher.weight + 1;
                                unvisited[temp].prevNode = searcher;
                            } else {
                                if (unvisited[temp].node.unitInThisNode.health < UnitPower) {
                                    unvisited[temp].weight = searcher.weight + 2;
                                    unvisited[temp].prevNode = searcher;
                                }
                            }
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
                        int temp = unvisited.FindIndex(x => x.node.Equals(compareThis));
                        if (unvisited[temp].node.canSphinxCross()
                        && (searcher.weight + 1 < unvisited[temp].weight)) {
                            if (!unvisited[temp].node.isThereAUnitHere()){ 
                                unvisited[temp].weight = searcher.weight + 1;
                                unvisited[temp].prevNode = searcher;
                            } else {
                                if (unvisited[temp].node.unitInThisNode.health<UnitPower) {
                                    // como la esfinge prioriza al que tenga una reliquia tiende a ignorar caminos mas cortos a travez de unidades
                                    //se le dara la opcion de considerar una unidad debil como un camino aceptable
                                    unvisited[temp].weight = searcher.weight + 2;
                                    unvisited[temp].prevNode = searcher;
                                }
                            }
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

    public override void getRekt()
    {
        Debug.Log("enemy destroyed");
        GameObject explotion_inst = Instantiate((GameObject)Resources.Load("enemy_explotion"), this.transform.position, Quaternion.identity, gridMaster.groupEnemy);
        Destroy(explotion_inst, 1.0f);

        if (heldRelic != null)
        {
            heldRelic.transform.position = this.transform.position;
            heldRelic.onReveal();
            heldRelic = null;
            Debug.Log("relic dropped");

        }
        gridMaster.addDinero(globals.guardianCoinLoot);
        Destroy(this.gameObject);
    }

    public override void OnDestroy() {
        gridMaster.updateActors();
    }
}
