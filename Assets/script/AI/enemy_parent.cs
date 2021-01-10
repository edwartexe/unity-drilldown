using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class enemy_parent : MonoBehaviour{

    public bool selected = false;
    public bool alreadyMoved = false;
    public Node lastNode;
    public Node thisNode;
    public bool enemyDone;

    public int maxWalkDistance = 2;
    public int attackRange = 1;
    public string enemyClass;
    public int maxHealth;
    public int health;
    public int UnitPower;
    public Grid gridMaster;
    public selector selector;
    public List<Node> movementRange;
    public List<Node> attackRangeNodes;
    
    public relic heldRelic;

    public Animator headAnimator;
    public Animator bodyAnimator;
    public SpriteRenderer[] childSprites;

    // Start is called before the first frame update
    void Start()
    {
        gridMaster = (Grid)GameObject.Find("GameMaster").GetComponent(typeof(Grid));
        selector = (selector)GameObject.Find("Selector").GetComponent(typeof(selector));

        headAnimator = this.transform.Find("worm_head").gameObject.GetComponent<Animator>();
        bodyAnimator = this.transform.Find("worm_body").gameObject.GetComponent<Animator>();
        childSprites = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer csprite in childSprites) {
            csprite.enabled = false;
        }

        enemyDone = true;
        thisNode = gridMaster.nodeFromWorldPoint(this.transform.position);
        lastNode = thisNode;
        heldRelic = null;

        enemyClass = globals.wormname;
        maxHealth= globals.wormhealth;
        health = globals.wormhealth;
        UnitPower = globals.wormPower;
    }

    public virtual void updatePosition() {
        //this.transform.position = new Vector3(thisNode.gridPoint.x, thisNode.gridPoint.y, 0);
        this.StartCoroutine(MoveObject(this.transform, this.transform.position, new Vector3(thisNode.gridPoint.x, thisNode.gridPoint.y, 0) , 0.3f));
    }

    //smooth movement
    public virtual IEnumerator MoveObject(Transform thisTransform, Vector3 startPos, Vector3 endPos, float time) {
        float i = 0.0f;
        float rate = 2.0f / time;
        while (i < 2.0f) {
            i += Time.deltaTime * rate;
            thisTransform.position = Vector3.Lerp(startPos, endPos, i);
            if (heldRelic != null) { heldRelic.transform.position = Vector3.Lerp(startPos, endPos, i); }
            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //this.gameObject.GetComponent<SpriteRenderer>().enabled = !thisNode.nodeOculto;
        foreach (SpriteRenderer csprite in childSprites) {
            csprite.enabled = !thisNode.nodeOculto;
        }
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //---------------------------------------------------------------move range ------------------------------------------------------------------------------------------
    public virtual bool highLightMoveableNodes(Node searcher) {
        //return checkDistanceWalk(searcher, thisNode, thisNode, 0);
        return movementRange.Contains(searcher);
    }

    public virtual bool highLightAttackableNodes(Node searcher) {
        //return checkDistanceWalk(searcher, thisNode, thisNode, 0);
        return attackRangeNodes.Contains(searcher);
    }

    public virtual List<Node> getAttackTarget(Node fromnode) {
        List<Node> vecinos = new List<Node>();
        List<Node> targets = new List<Node>();
        if (fromnode.upNode != null) { vecinos.Add(fromnode.upNode); }
        if (fromnode.rightNode != null) { vecinos.Add(fromnode.rightNode); }
        if (fromnode.downNode != null) { vecinos.Add(fromnode.downNode); }
        if (fromnode.leftNode != null) { vecinos.Add(fromnode.leftNode); }

        foreach (Node nn in vecinos) {
                if (nn.isThereAUnitHere()) {
                    targets.Add(nn);
                }
        }

        return targets;
    }

   

    public virtual List<Node> listMovementRange() {
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
                if (newNode.walkable == false) {  pass = false; }
                if (newNode.nodeOculto) {  pass = false;  }
                if (newNode.isThereAUnitHere() == true) { pass = false;  }*/
                if (newNode.canEnemyCross() && !newNode.isThereAUnitHere()) { 
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
        this.movementRange=visited;
        listAttackNodes();
        return visited;
    }

    public virtual List<Node> listAttackNodes() {

        List<Node> targetsvisited = new List<Node>();
        List<Node> targetsvisiting = new List<Node>();

        targetsvisiting.AddRange(movementRange);
        for (int i = 0; i <= attackRange; i++) {
            //Debug.Log(this.gameObject.name + " i: " + i + "/" + attackRange);
            List<Node> unvisited = new List<Node>();

            foreach (Node newNode in targetsvisiting) {
                if (newNode.canEnemyCross()) {
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


    public virtual IEnumerator enemysturn() {
        enemyDone = false;
        animateHead(1);
        animateBody(true);

        //take relic if at the spot
        if (thisNode.isThereAnItemHere()) {
            relic targetrelic = (relic)thisNode.itemInThisNode.GetComponent<relic>();
            heldRelic = targetrelic;
            targetrelic.onTake(false);
            targetrelic.transform.position = transform.position;
        }

        //is this enemy active
        if (!thisNode.nodeOculto) {
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
                        upa.aggro += 3;
                    }

                    if (upa.health < this.UnitPower) {
                        upa.aggro += 2;
                    }

                    if (highLightAttackableNodes(upa.thisNode)) {
                        upa.aggro += 7;
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
                        this.thisNode = attackPath[i];
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
                    animateHead(2);
                    prey.health -= UnitPower;
                    prey.gameObject.GetComponent<Animator>().Play("unitHitAnimation");
                    if (prey.health <= 0) {
                        //Destroy(prey.gameObject);
                        prey.getRekt();
                    }
                    yield return new WaitForSeconds(0.5f);
                }

            } 

        } else {
            Debug.Log(this.gameObject.name + " sleeping");
        }
        yield return null;

        enemyDone = true;
        selector.camScript.focusedObject = null;
        animateHead(0);
        animateBody(false);
    }

    public virtual void animateHead(int state) {
        if (headAnimator != null) {
            headAnimator.SetInteger("state", state);
        }
    }

    public virtual void animateBody(bool state) {
        if (bodyAnimator != null) {
            bodyAnimator.SetBool("moving", state);
        }
    }

    public virtual bool okToAttack() {
        return true;
    }


    //----------------------------------------------------- PATH FINDING -----------------------------------------------------------
    //buscar un camino de nodos desde el nodo actual a donde esta el jugador -------------------------------------------------------
    
    //este metodo hace djikstra desde el target hasta una zona en el espacio de movimiento de este enemigo
    public virtual List<Node> pathToUnit(Node targetNode, Node startNode, int limit, bool llegarAlVecino) {
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
            if (searcher.weight == 9999) { inWalkDistance = false; }

            if (inWalkDistance) {
                if (this.movementRange.Contains(searcher.node) && (!searcher.node.isThereAnEnemyHere() || searcher.node.enemyInThisNode.Equals(this)) && !searcher.node.tiletype.Contains("locked")) {
                    //en este if se debe checkear para nodos en los que el enemigo no  puede quedarse pero puede atravezar
                    //ahora que tenemos el nodo disponible mas cercano al target, buscar un camino hasta aquel
                    return pathToNode(searcher.node,startNode,limit);
                }

                foreach (Node compareThis in searcher.node.getVecinos()) {
                    if (compareThis != null) {
                        int temp = unvisited.FindIndex(x => x.node.Equals(compareThis));
                        if (unvisited[temp].node.canEnemyCross()
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
        return returnlist;

    }

    //djisktra hasta un nodo y devuelve el camino
    public virtual List<Node> pathToNode(Node targetNode, Node startNode, int limit) {
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
                        if (unvisited[temp].node.canEnemyCross()
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
        return returnlist;

    }
   

    public virtual void getRekt() {
        Debug.Log("enemy destroyed");
        GameObject explotion_inst = Instantiate((GameObject)Resources.Load("enemy_explotion"), this.transform.position, Quaternion.identity, gridMaster.groupEnemy);
        Destroy(explotion_inst, 1.0f);

        if (heldRelic != null) {
            heldRelic.transform.position = this.transform.position;
            heldRelic.onReveal();
            heldRelic = null;
            Debug.Log("relic dropped");

        }
        //gridMaster.addGas(globals.wormGasloot);
        gridMaster.addDinero(globals.wormGasloot);
        Destroy(this.gameObject);
    }

    public virtual void OnDestroy() {
        gridMaster.updateActors();
    }
}
