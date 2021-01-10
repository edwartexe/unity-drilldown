using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class unit_parent : MonoBehaviour{

    public bool selected=false;
    public bool alreadyMoved = false;
    public Node lastNode;
    public Node thisNode;
    public Node nextNode;
    public int maxWalkDistance;
    public string unitClass;
    public int maxHealth;
    public int health;
    public int attackPower;
    public int basecost;
    public Grid gridMaster;
    public selector selector;
    public List<Node> movementRange;
    public int temp_gas_usage;
    public int attackRange;
    public relic heldRelic;
    public bool isActive = true;

    //public Animator[] spriteAnimator;
    public Animator attackAnimator;
    public Animator movementAnimator;
    //public Animator elseAnimator;
    public SpriteRenderer[] childSprites;

    public int aggro = 0;//testing this value for enemy ai
    public int scanRange;


    // Start is called before the first frame update
    void Start() {

        gridMaster = (Grid)GameObject.Find("GameMaster").GetComponent(typeof(Grid));
        selector = (selector)GameObject.Find("Selector").GetComponent(typeof(selector));

        //spriteAnimator = this.gameObject.GetComponentsInChildren<Animator>(); //0=drill 1=threads

        attackAnimator = this.transform.Find("sprite_drill").gameObject.GetComponent<Animator>();
        movementAnimator = this.transform.Find("sprite_threads").gameObject.GetComponent<Animator>();
        childSprites = GetComponentsInChildren<SpriteRenderer>();

        thisNode = gridMaster.nodeFromWorldPoint(this.transform.position);
        lastNode = thisNode;
        heldRelic = null;

        maxWalkDistance = 3;
        unitClass = globals.drillname;
        health = globals.drillhealth;
        maxHealth= globals.drillhealth;
        attackPower = globals.drillattackstat;
        basecost = globals.action_cost_base_drill;
        attackRange = 1;
        scanRange = globals.scout_scan_range;
    }


    public void updatePosition() {
        //this.transform.position= new Vector3(thisNode.gridPoint.x, thisNode.gridPoint.y, 0);
        //this.StartCoroutine(MoveAlongPath(this.transform, this.transform.position, selector.pathToCursor, 0.3f));
        //this.StartCoroutine(MoveObject(this.transform, this.transform.position, new Vector3(thisNode.gridPoint.x, thisNode.gridPoint.y, 0), 0.3f));
        object[] parms = new object[] { this.transform, this.transform.position, new Vector3(thisNode.gridPoint.x, thisNode.gridPoint.y, 0), 0.3f };
        this.StartCoroutine("MoveObject", parms);
    }

    public void previewPosition() {
        //this.transform.position = new Vector3(nextNode.gridPoint.x, nextNode.gridPoint.y, 0);
        //this.StartCoroutine(MoveObject(this.transform, this.transform.position, new Vector3(nextNode.gridPoint.x, nextNode.gridPoint.y, 0), 0.3f));
        //this.StartCoroutine(MoveAlongPath(this.transform, this.transform.position, selector.pathToCursor, 0.3f) );
        object[] parms = new object[] { this.transform, this.transform.position, selector.pathToCursor, 0.3f };
        this.StartCoroutine("MoveAlongPath", parms);
    }

    //smooth movement
    public virtual IEnumerator MoveObject(object[] paramets) {
        Transform thisTransform =(Transform)paramets[0];
        Vector3 startPos        = (Vector3)paramets[1];
        Vector3 endPos          = (Vector3)paramets[2];
        float time              = (float)paramets[3];

        float i = 0.0f;
        float rate = 2.0f / time;
        while (i < 2.0f) {
            i += Time.deltaTime * rate;
            thisTransform.position = Vector3.Lerp(startPos, endPos, i);
            if (heldRelic!=null) { 
                heldRelic.transform.position=Vector3.Lerp(startPos, endPos, i);
            }
            yield return null;
        }
    }

    //smooth chained movement
    public virtual IEnumerator MoveAlongPath(object[] paramets) {
        Transform whichTransform    = (Transform)paramets[0];
        Vector3 startPos            = (Vector3)paramets[1];
        List<Node> pathtoalert      = (List<Node>)paramets[2];
        float time                  = (float)paramets[3];

        float rate = 3.0f / time;
        Vector3 currentPos= startPos;
        temp_gas_usage = -1;
        foreach (Node nn in pathtoalert) {
            float i = 0.0f;
            temp_gas_usage++;
            while (i < 2.0f) {
                i += Time.deltaTime * rate;
                whichTransform.position = Vector3.Lerp(currentPos, new Vector3(nn.gridPoint.x, nn.gridPoint.y, 0), i);
                if (heldRelic != null) { 
                    heldRelic.transform.position = Vector3.Lerp(currentPos, new Vector3(nn.gridPoint.x, nn.gridPoint.y, 0), i); 
                }
                yield return null;
            }
            currentPos = new Vector3(nn.gridPoint.x, nn.gridPoint.y, 0);


        }
    }
    /*
    //smooth movement
    public virtual IEnumerator MoveObject(Transform thisTransform, Vector3 startPos, Vector3 endPos, float time) {
        float i = 0.0f;
        float rate = 2.0f / time;
        while (i < 2.0f) {
            i += Time.deltaTime * rate;
            thisTransform.position = Vector3.Lerp(startPos, endPos, i);
            if (heldRelic != null) {
                heldRelic.transform.position = Vector3.Lerp(startPos, endPos, i);
            }
            yield return null;
        }
    }

    //smooth chained movement
    public virtual IEnumerator MoveAlongPath(Transform whichTransform, Vector3 startPos, List<Node> pathtoalert, float time) {
        float rate = 3.0f / time;
        Vector3 currentPos = startPos;
        temp_gas_usage = -1;
        foreach (Node nn in pathtoalert) {
            float i = 0.0f;
            //Debug.Log("cycle at ["+nn.gridPoint.x +" , "+nn.gridPoint.y+"]");
            temp_gas_usage++;
            //this.StartCoroutine(MoveObject(this.transform, this.transform.position, new Vector3(nn.gridPoint.x, nn.gridPoint.y, 0), 0.3f));
            while (i < 2.0f) {
                i += Time.deltaTime * rate;
                whichTransform.position = Vector3.Lerp(currentPos, new Vector3(nn.gridPoint.x, nn.gridPoint.y, 0), i);
                if (heldRelic != null) {
                    heldRelic.transform.position = Vector3.Lerp(currentPos, new Vector3(nn.gridPoint.x, nn.gridPoint.y, 0), i);
                }
                yield return null;
            }
            currentPos = new Vector3(nn.gridPoint.x, nn.gridPoint.y, 0);


        }
    }*/

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
    public virtual bool highLightMoveableNodes(Node searcher) {
        //return checkDistanceWalk(searcher, thisNode, thisNode, 0);
        return movementRange.Contains(searcher);
    }
    
    public virtual bool checkDrillTargets(Node targetNode, bool compareNode) {
        List<Node> vecinos = new List<Node>();
        if (nextNode.upNode != null)    { vecinos.Add(nextNode.upNode); }
        if (nextNode.rightNode != null) { vecinos.Add(nextNode.rightNode); }
        if (nextNode.downNode != null)  { vecinos.Add(nextNode.downNode); }
        if (nextNode.leftNode != null)  { vecinos.Add(nextNode.leftNode); }

        foreach (Node nn in vecinos) {
            if (compareNode) {
                if (targetNode.Equals(nn)) {
                    if (nn.nodeOculto) {
                        return true;
                    }
                }
            } else {
                    if (nn.nodeOculto) {
                        return true;
                    }
            }
        }

        return false;
    }
    

    public virtual bool checkAttackTargets(Node targetNode, bool compareNode) {
        List<Node> vecinos = listAttackNodes();

        foreach (Node nn in vecinos) {

            if (compareNode) {
                if (targetNode.Equals(nn)) {
                    if (nn.isThereAnEnemyHere() && nn.enemyInThisNode.okToAttack() && !nn.nodeOculto) {
                        return true;
                    }
                }
            } else {
                if (nn.isThereAnEnemyHere() && nn.enemyInThisNode.okToAttack() && !nn.nodeOculto) {
                    return true;
                }
            }


        }

        return false;
    }

    public List<Node> listAttackNodes() {

        List<Node> targetsvisited = new List<Node>();
        List<Node> targetsvisiting = new List<Node>();

        //targetsvisiting.AddRange(movementRange);
        targetsvisiting.Add(nextNode);
        for (int i = 0; i <= attackRange; i++) {
            //Debug.Log(this.gameObject.name + " i: " + i + "/" + attackRange);
            List<Node> unvisited = new List<Node>();

            foreach (Node newNode in targetsvisiting) {
                if (newNode.canUnitAttack()) {
                    if (!targetsvisited.Contains(newNode.upNode) && newNode.upNode != null) { unvisited.Add(newNode.upNode); }
                    if (!targetsvisited.Contains(newNode.rightNode) && newNode.rightNode != null) { unvisited.Add(newNode.rightNode); }
                    if (!targetsvisited.Contains(newNode.downNode) && newNode.downNode != null) { unvisited.Add(newNode.downNode); }
                    if (!targetsvisited.Contains(newNode.leftNode) && newNode.leftNode != null) { unvisited.Add(newNode.leftNode); }

                    if (!newNode.isThereAUnitHere() || newNode.unitInThisNode.Equals(this)) {
                        targetsvisited.Add(newNode);
                    }
                }
            }

            targetsvisiting = unvisited;
        }
        //this.attackRangeNodes = targetsvisited;
        return targetsvisited;
    }

    public virtual bool checkHoldTargets(Node targetNode, bool compareNode) {
        if (this.heldRelic != null) { return false; }

        List<Node> vecinos = new List<Node>();
        if (nextNode != null) { vecinos.Add(nextNode); }
        if (nextNode.upNode != null && !nextNode.upNode.nodeOculto) { vecinos.Add(nextNode.upNode); }
        if (nextNode.rightNode != null && !nextNode.rightNode.nodeOculto) { vecinos.Add(nextNode.rightNode); }
        if (nextNode.downNode != null && !nextNode.downNode.nodeOculto) { vecinos.Add(nextNode.downNode); }
        if (nextNode.leftNode != null && !nextNode.leftNode.nodeOculto) { vecinos.Add(nextNode.leftNode); }

        foreach (Node nn in vecinos) {

            if (compareNode) {
                if (targetNode.Equals(nn)) {
                    if (nn.isThereAnItemHere() && nn.itemInThisNode.state == 1 && !nn.tiletype.Contains("locked") ) {
                        return true;
                    }
                }
            } else {
                if (nn.isThereAnItemHere() && nn.itemInThisNode.state == 1 && !nn.tiletype.Contains("locked")) {
                    return true;
                }
            }


        }
        return false;
    }

    public virtual bool checkDropTargets(Node targetNode, bool compareNode) {
        if (this.heldRelic == null) { return false; }

        List<Node> vecinos = new List<Node>();
        if (nextNode != null) { vecinos.Add(nextNode); }
        if (nextNode.upNode != null && !nextNode.upNode.nodeOculto) { vecinos.Add(nextNode.upNode); }
        if (nextNode.rightNode != null && !nextNode.rightNode.nodeOculto) { vecinos.Add(nextNode.rightNode); }
        if (nextNode.downNode != null && !nextNode.downNode.nodeOculto) { vecinos.Add(nextNode.downNode); }
        if (nextNode.leftNode != null && !nextNode.leftNode.nodeOculto) { vecinos.Add(nextNode.leftNode); }

        foreach (Node nn in vecinos) {

            if (compareNode) {
                if (targetNode.Equals(nn)) {
                    //if (nn.tiletype.Equals("HQ")) {
                    if (!nn.isThereAnEnemyHere()) {
                        return true;
                    }
                }
            } else {
                if (!nn.isThereAnEnemyHere()) {
                    return true;
                }
            }


        }
        return false;
    }

    public virtual bool checkBombTargets(Node targetNode, bool compareNode) {
        List<Node> vecinos = new List<Node>();
        if (nextNode.upNode != null) { 
            vecinos.Add(nextNode.upNode);
            if (nextNode.upNode.rightNode != null) { vecinos.Add(nextNode.upNode.rightNode); }
            if (nextNode.upNode.leftNode != null) { vecinos.Add(nextNode.upNode.leftNode); }
        }
        if (nextNode.rightNode != null) { vecinos.Add(nextNode.rightNode); }
        if (nextNode.downNode != null) { 
            vecinos.Add(nextNode.downNode); 
            if (nextNode.downNode.rightNode != null){ vecinos.Add(nextNode.downNode.rightNode);}
            if (nextNode.downNode.leftNode != null) { vecinos.Add(nextNode.downNode.leftNode); }
        }
        if (nextNode.leftNode != null) { vecinos.Add(nextNode.leftNode); }


        foreach (Node nn in vecinos) {
            if (compareNode) {
                if (targetNode.Equals(nn)) {
                    return true;
                }
            } else {
                return true;
            }
        }

        return false;
    }

    public virtual List<Node> listBombNodes() {

        List<Node> vecinos = new List<Node>();
        if (nextNode.upNode != null) {
            vecinos.Add(nextNode.upNode);
            if (nextNode.upNode.rightNode != null) { vecinos.Add(nextNode.upNode.rightNode); }
            if (nextNode.upNode.leftNode != null) { vecinos.Add(nextNode.upNode.leftNode); }
        }
        if (nextNode.rightNode != null) { vecinos.Add(nextNode.rightNode); }
        if (nextNode.downNode != null) {
            vecinos.Add(nextNode.downNode);
            if (nextNode.downNode.rightNode != null) { vecinos.Add(nextNode.downNode.rightNode); }
            if (nextNode.downNode.leftNode != null) { vecinos.Add(nextNode.downNode.leftNode); }
        }
        if (nextNode.leftNode != null) { vecinos.Add(nextNode.leftNode); }
        return vecinos;
    }

    public virtual List<Node> getScanTargets() {
        bool bbb = false;
        int centerX = (int) this.nextNode.gridPoint.x;
        int centerY = (int) this.nextNode.gridPoint.y;
        int startX  = Mathf.Clamp(centerX - globals.scout_scan_range, 0, gridMaster.gridSizeX - 1);
        int startY  = Mathf.Clamp(centerY - globals.scout_scan_range, 0, gridMaster.gridSizeZ - 1);
        int finishX = Mathf.Clamp(centerX + globals.scout_scan_range, 0, gridMaster.gridSizeX - 1);
        int finishY = Mathf.Clamp(centerY + globals.scout_scan_range, 0, gridMaster.gridSizeZ - 1);

        List<Node> scanTargets = new List<Node>();
        for (int i = startX; i<=finishX;i++) {
            for (int j = startY; j <= finishY; j++) {
                if (gridMaster.grid[i, j]!=null) {
                    scanTargets.Add(gridMaster.grid[i, j]);
                }
            }
        }

        return scanTargets;
    }

    public virtual bool checkScanTargets(Node searcher, bool compareNode) {
        if (compareNode) {
            return getScanTargets().Contains(searcher); 
        } else {
            return getScanTargets().Count > 0;
        }
        

    }
    //=======================================================================================
    

    public virtual void animateDrill(bool state) {
        if (attackAnimator!=null) {
            attackAnimator.SetBool("drilling", state);
        }
    }

    public virtual void animateThreads(bool state) {
        if (movementAnimator!=null) {
            movementAnimator.SetBool("moving", state);
        }
    }


    public int currentWalkDistance() {
        if (gridMaster.recursoGas <= 0) {
            return 0;
        } else {
            return maxWalkDistance;
        }
    }

    public virtual List<Node> listMovementRange() {
        List<Node> visited = new List<Node>();
        List<Node> visiting = new List<Node>();

        visiting.Add(thisNode);

        //if turned off skip other nodes
        if (!isActive) {
            this.movementRange = visiting;
            return visiting;
        }

        for (int i = 0; i <= currentWalkDistance(); i++) {
            //Debug.Log("i: " + i + "/" + currentWalkDistance());
            List<Node> unvisited = new List<Node>();

            foreach (Node newNode in visiting) {
                if ( newNode.canUnitCross() ) {
                    if (!visited.Contains(newNode.upNode) && newNode.upNode != null) { unvisited.Add(newNode.upNode); }
                    if (!visited.Contains(newNode.rightNode) && newNode.rightNode != null) { unvisited.Add(newNode.rightNode); }
                    if (!visited.Contains(newNode.downNode) && newNode.downNode != null) { unvisited.Add(newNode.downNode); }
                    if (!visited.Contains(newNode.leftNode) && newNode.leftNode != null) { unvisited.Add(newNode.leftNode); }

                    if (!newNode.isThereAUnitHere() || newNode.unitInThisNode.Equals(this)) {
                        visited.Add(newNode);
                    }
                    
                }
            }
            visiting = unvisited;
        }
        this.movementRange = visited;
        return visited;
    }

    public void getRekt() {
        Debug.Log("desu troyed");

        GameObject explotion_inst = Instantiate((GameObject)Resources.Load("unit_explotion"), this.transform.position, Quaternion.identity, gridMaster.groupUnit);
        Destroy(explotion_inst, 1.0f);

        if (heldRelic!=null) {
            /*relic newrelic = Instantiate(gridMaster.relic_prefab, this.transform.position, Quaternion.identity);
            newrelic.relicID = relicID;
            newrelic.onReveal();
            gridMaster.relics.Add(newrelic);*/


            heldRelic.transform.position = this.transform.position;
            heldRelic.onReveal();
            heldRelic = null;


            Debug.Log("relic dropped");
        }
        Destroy(this.gameObject);
    }

    public void detonate() {
        Debug.Log("XPLOTIOOOOON");
        
        GameObject explotion_inst = Instantiate((GameObject)Resources.Load("unit_explotion"), this.transform.position, Quaternion.identity, gridMaster.groupUnit);
        Destroy(explotion_inst, 1.0f);

        if (heldRelic != null) {
            heldRelic.transform.position = this.transform.position;
            heldRelic.onReveal();
            heldRelic = null;
            Debug.Log("relic dropped");
        }
        Destroy(this.gameObject);
    }

    private void OnDestroy() {
        gridMaster.updateActors();
    }


}
