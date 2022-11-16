using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node{
	public Vector2 gridPoint;
    public string tiletype;
    public string shownTileType;
    public bool nodeOculto;
    public bool walkable;
    public bool pathmember = false;
    public bool hasSomething = false;

    public Node leftNode;
	public Node rightNode;
	public Node upNode;
	public Node downNode;
    private Sprite defaultSprite= Resources.Load<Sprite>("Square");
    private Sprite focusedSprite= Resources.Load<Sprite>("SquareFocused");


    public GameObject nodeSquare;
    public GameObject nodeTile;

    public Grid gridMaster;
    public unit_HQ unitHQ_code => gridMaster.unitHQ_code;
    public unit_parent unitInThisNode;
    public enemy_parent enemyInThisNode;
    public item_parent itemInThisNode;


    public Node(){
		leftNode=null;
		rightNode=null;
		upNode=null;
		downNode=null;
        walkable = false;

        gridMaster = (Grid) GameObject.Find("Gridmaster").GetComponent(typeof(Grid));
        //unitHQ_code = (unit_HQ) GameObject.Find("unitHQ").GetComponent(typeof(unit_HQ));
    }

	public Node(Vector2 gridPoint2, string tile2, bool _oculto, Grid _gridmaster){
		gridPoint = gridPoint2;
		tiletype = tile2;

		leftNode = null;
		rightNode = null;
		upNode = null;
		downNode = null;
        
        walkable = true;

        nodeOculto = _oculto;
        gridMaster =_gridmaster;
        //unitHQ_code = (unit_HQ)GameObject.Find("unitHQ").GetComponent(typeof(unit_HQ));
    }

    public List<Node> getVecinos() {
        List<Node> temp = new List<Node>();
        if (leftNode!=null)     { temp.Add(leftNode); }
        if (rightNode != null)  { temp.Add(rightNode); }
        if (upNode != null)     { temp.Add(upNode); }
        if (downNode != null)   { temp.Add(downNode); }
        return temp;
    }
    



	public void drawmelikeafrenchgirl(){
		nodeSquare = new GameObject ();
		nodeSquare.name="gridSquare["+gridPoint.x+","+gridPoint.y+"]";
        nodeSquare.transform.parent = gridMaster.groupNode;
        nodeSquare.transform.position = new Vector2(gridPoint.x, gridPoint.y);
		nodeSquare.transform.localEulerAngles = new Vector3 (0,0,0);
		nodeSquare.transform.localScale = new Vector3 (5f,5f,5f);
		SpriteRenderer render = nodeSquare.AddComponent<SpriteRenderer>();
        render.sortingOrder = 1;
        render.sprite = Resources.Load ("Square", typeof(Sprite)) as Sprite;

        nodeTile = new GameObject();
        nodeTile.name = "tile[" + gridPoint.x + "," + gridPoint.y + "]";
        nodeTile.transform.parent = nodeSquare.transform;
        nodeTile.transform.position = new Vector2(gridPoint.x, gridPoint.y);
        nodeTile.transform.localEulerAngles = new Vector3(0, 0, 0);
        nodeTile.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        SpriteRenderer renderTile = nodeTile.AddComponent<SpriteRenderer>();
        renderTile.sortingOrder = 0;
        Sprite[] tyleIconsAtlas = Resources.LoadAll<Sprite>("emerald_tyleset");
        //renderTile.sprite = tyleIconsAtlas.Single(s => s.name == "emerald_tyleset_102");
        
        if (nodeOculto) {
            if (hasSomething) {
                renderTile.sprite = tyleIconsAtlas[142];
                shownTileType = "Unrevealed?";
            } else {
                renderTile.sprite = tyleIconsAtlas[102];
                shownTileType = "Unrevealed";
            }
        } else if (tiletype.Contains("bloqueado")) {
            renderTile.sprite = tyleIconsAtlas[111];
            shownTileType = "Blocked";
        } else if (tiletype.Contains("GuardOnly")) {
            renderTile.sprite = tyleIconsAtlas[6];
            shownTileType = "Enemy Only";
        } else if (tiletype.Contains("locked")) {
            renderTile.sprite = tyleIconsAtlas[116];
            shownTileType = "Enemy Barrier";
        } else if (tiletype.Contains("sphinxstatue")) {
            renderTile.sprite = tyleIconsAtlas[125];
            shownTileType = "Sphinx Statue";
        }  else if (tiletype.Contains("Guardian")) {
            renderTile.sprite = tyleIconsAtlas[125];
            shownTileType = "Guard Statue";
        } else if (tiletype.Contains("nest")) {
            renderTile.sprite = tyleIconsAtlas[1];
            shownTileType = "Enemy Nest";
        } else if (tiletype.Contains("bottomless")) {
            renderTile.sprite = tyleIconsAtlas[105];
            shownTileType = "Bottomless";
        } else if (tiletype.Contains("relic")) {
            renderTile.sprite = tyleIconsAtlas[159];
            shownTileType = "Relic";
        } else if (tiletype.Contains("normal")) {
            renderTile.sprite = tyleIconsAtlas[123];
            shownTileType = "Normal";
        }

    }

    public void updateSprite() {
        if (nodeTile != null && nodeTile.GetComponent<SpriteRenderer>() != null) {
            SpriteRenderer renderTile = nodeTile.GetComponent<SpriteRenderer>();
            Sprite[] tyleIconsAtlas = Resources.LoadAll<Sprite>("emerald_tyleset");
            if (nodeOculto) {
                if (hasSomething) {
                    renderTile.sprite = tyleIconsAtlas[142];
                    shownTileType = "Unrevealed?";
                } else {
                    renderTile.sprite = tyleIconsAtlas[102];
                    shownTileType = "Unrevealed";
                }
            } else if (tiletype.Contains("bloqueado")) {
                renderTile.sprite = tyleIconsAtlas[111];
                shownTileType = "Blocked";
            } else if (tiletype.Contains("GuardOnly")) {
                renderTile.sprite = tyleIconsAtlas[6];
                shownTileType = "Enemy Only";
            } else if (tiletype.Contains("locked")) {
                renderTile.sprite = tyleIconsAtlas[116];
                shownTileType = "Locked";
            } else if (tiletype.Contains("sphinxstatue")) {
                renderTile.sprite = tyleIconsAtlas[125];
                shownTileType = "Sphinx Statue";
            } else if (tiletype.Contains("Guardian"))  {
                renderTile.sprite = tyleIconsAtlas[125];
                shownTileType = "Guard Statue";
            } else if (tiletype.Contains("nest")) {
                renderTile.sprite = tyleIconsAtlas[1];
                shownTileType = "Enemy Nest";
            } else if (tiletype.Contains("bottomless")) {
                renderTile.sprite = tyleIconsAtlas[105];
                shownTileType = "Bottomless";
            } else if (tiletype.Contains("relic")) {
                renderTile.sprite = tyleIconsAtlas[159];
                shownTileType = "Relic";
            } else if (tiletype.Contains("normal")) {
                renderTile.sprite = tyleIconsAtlas[123];
                shownTileType = "Normal";
            }
        }
    }

    public void revealNode(bool skipGain = false) {
        this.nodeOculto = false;
        this.hasSomething = false;
        if (isThereAnItemHere()) {
            if (isThereAnEnemyHere() && enemyInThisNode.heldRelic!=null && enemyInThisNode.heldRelic.Equals(itemInThisNode) ) {
                itemInThisNode.onTake(true);
            } else {
                itemInThisNode.onReveal();
            }
        }
        //gridMaster.addGas(globals.node_reveal_gas);
        if (!skipGain) {
            if (tiletype.Contains("bloqueado") || tiletype.Contains("locked") || tiletype.Contains("GuardOnly") || tiletype.Contains("sphinxstatue") || tiletype.Contains("Guardian")) {
                gridMaster.addDinero(globals.node_blocked_coin);
            } else {
                gridMaster.addDinero(globals.node_reveal_coin);
            }
        }
        updateSprite();
    }

    public void burryNode() {
        this.nodeOculto = true;
        if (isThereAnItemHere()) {
            itemInThisNode.onHide();
        }
        updateSprite();
    }

    public void markInterest() {
        if (hasSomething) {
            return;
        }
        this.hasSomething = true;

        List<Node> visited = new List<Node>();
        List<Node> visiting = new List<Node>();
        visiting.Add(this);
        for (int i = 0; i <= 2; i++) {
            List<Node> unvisited = new List<Node>();

            foreach (Node newNode in visiting) {
                if (!visited.Contains(newNode.upNode) && newNode.upNode != null) { unvisited.Add(newNode.upNode); }
                if (!visited.Contains(newNode.rightNode) && newNode.rightNode != null) { unvisited.Add(newNode.rightNode); }
                if (!visited.Contains(newNode.downNode) && newNode.downNode != null) { unvisited.Add(newNode.downNode); }
                if (!visited.Contains(newNode.leftNode) && newNode.leftNode != null) { unvisited.Add(newNode.leftNode); }

                if (newNode.nodeOculto) {
                    float rng = Random.Range(0f, 10f);
                    if (rng > 5f) {
                        newNode.hasSomething = true;
                        newNode.updateSprite();
                    }
                }

                visited.Add(newNode);
            }
            visiting = unvisited;
        }
        updateSprite();
    }

    public bool isThereAUnitHere(){
		bool result=false;
        GameObject[] units = gridMaster.units;
         foreach (GameObject unit in units) {
             unit_parent unitcode = (unit_parent) unit.GetComponent(typeof(unit_parent));
             if (unitcode.thisNode.Equals (this)){
                 unitInThisNode = unitcode;
                 result = true;
             }
         }
         return result;
     }

    public bool isThereAnEnemyHere() {
        bool result = false;
        GameObject[] enemies = gridMaster.enemies;
        foreach (GameObject enemy in enemies) {
            enemy_parent enemycode = (enemy_parent)enemy.GetComponent(typeof(enemy_parent));
            if (enemycode.thisNode.Equals(this)) {
                enemyInThisNode = enemycode;
                result = true;
            }
        }
        return result;
    }

    public bool isThereAnItemHere() {
        bool result = false;
        List<relic> relics = gridMaster.relics;
        foreach (relic item in relics) {
            if (item.thisNode.Equals(this)) {
                itemInThisNode = item;
                result = true;
            }
        }
        return result;
    }

    public bool isHQhere() {
        return unitHQ_code.thisNode.Equals(this);
    }

    //------------------------------------------------------------------------

    public bool canUnitCross() {
        return (
            !tiletype.Contains("bloqueado") &&
            !tiletype.Contains("GuardOnly") &&
            !tiletype.Contains("locked") &&
            !tiletype.Contains("bottomless") &&
            !tiletype.Contains("sphinxstatue") &&
            !tiletype.Contains("Guardian") &&
            walkable &&
            !nodeOculto &&
            !isThereAnEnemyHere()
        );
    }

    public bool canUnitAttack() {
        return (
            !tiletype.Contains("bloqueado") && 
            !tiletype.Contains("locked") &&
            !tiletype.Contains("bottomless") &&
            !tiletype.Contains("sphinxstatue") &&
            !tiletype.Contains("Guardian") &&
            walkable &&
            !nodeOculto
        );
    }

    public bool canUnitDig()
    {
        return (
            nodeOculto
            ||
            (
            !nodeOculto &&
            !tiletype.Contains("bloqueado") &&
            !tiletype.Contains("locked") &&
            !tiletype.Contains("sphinxstatue") &&
            !tiletype.Contains("Guardian") &&
            !isThereAnEnemyHere()
            )
        );
    }

    public bool canEnemyCross() {
        return (
        !tiletype.Contains("bloqueado") && 
        !tiletype.Contains("bottomless") && 
        !tiletype.Contains("sphinxstatue") &&
        !tiletype.Contains("Guardian") &&
        walkable && 
        !nodeOculto
        );
    }

    public bool canThiefCross() {
        return (
        !tiletype.Contains("bloqueado") 
        && !tiletype.Contains("bottomless")
        && !tiletype.Contains("sphinxstatue")
        && !tiletype.Contains("Guardian")
        && walkable
        );
    }

    public bool canSphinxCross() {
        return (
        !tiletype.Contains("bloqueado")
        && !tiletype.Contains("bottomless")
        && !tiletype.Contains("Guardian")
        && !tiletype.Contains("sphinxstatue")
        && walkable
        );
    }


    //Update
    public void Update() {
        nodeSquare.GetComponent<SpriteRenderer>().sprite = defaultSprite;
        nodeSquare.GetComponent<SpriteRenderer>().color = Color.white;
        /*
		if (gridMaster.cursorCode.cursorNode.Equals (gridMaster.cursorCode.playerCode.playerNode)) {
			if(gridMaster.cursorCode.highLightMoveableNodes(this)){
				nodeSquare.GetComponent<SpriteRenderer> ().color = Color.blue;
			}
		}
		foreach (GameObject enemy in gridMaster.enemies) {
			EnemyParent enemyCode = (EnemyParent) enemy.GetComponent(typeof(EnemyParent));
			if(enemyCode.highLightSeenNodes(this)){
				nodeSquare.GetComponent<SpriteRenderer> ().color = Color.red;
			}
		}
		//cursor potential sound
		if(gridMaster.cursorCode.highLightSoundNodes(this)){
			nodeSquare.GetComponent<SpriteRenderer> ().color = Color.green;
		}
		
		*/

        foreach (GameObject unit in gridMaster.units) {
            unit_parent unit_code = (unit_parent)unit.GetComponent(typeof(unit_parent));
            if (unit_code.selected) {

                switch (gridMaster.selector.state_selector) {
                    case 11:
                        if (unit_code.checkScanTargets(this, true)) {
                            nodeSquare.GetComponent<SpriteRenderer>().sprite = focusedSprite;
                            nodeSquare.GetComponent<SpriteRenderer>().color = Color.green;
                        }
                        break;
                    case 10:
                        if (unit_code.checkScanTargets(this, true)) {
                            nodeSquare.GetComponent<SpriteRenderer>().sprite = focusedSprite;
                            nodeSquare.GetComponent<SpriteRenderer>().color = Color.green;
                        }
                        break;
                    case 9:
                        if (unit_code.checkBombTargets(this, true)) { 
                            nodeSquare.GetComponent<SpriteRenderer>().sprite = focusedSprite;
                            nodeSquare.GetComponent<SpriteRenderer>().color = Color.green;
                        }
                        break;
                    case 8:
                        if (unit_code.checkDropTargets(this, true)) { 
                            nodeSquare.GetComponent<SpriteRenderer>().sprite = focusedSprite;
                            nodeSquare.GetComponent<SpriteRenderer>().color = Color.green;
                        }
                        break;

                    case 5:
                        if (unit_code.checkHoldTargets(this, true)) {
                            nodeSquare.GetComponent<SpriteRenderer>().sprite = focusedSprite;
                            nodeSquare.GetComponent<SpriteRenderer>().color = Color.green;
                        }
                        break;

                    case 4:
                        if (unit_code.checkAttackTargets(this, true)) {
                            nodeSquare.GetComponent<SpriteRenderer>().sprite = focusedSprite;
                            nodeSquare.GetComponent<SpriteRenderer>().color = Color.green;
                        }
                        break;

                    case 3:
                        if (unit_code.checkDrillTargets(this, true)) {
                            nodeSquare.GetComponent<SpriteRenderer>().sprite = focusedSprite;
                            nodeSquare.GetComponent<SpriteRenderer>().color = Color.green;
                        }
                        break;

                    default:
                        if (unit_code.highLightMoveableNodes(this)) {
                            nodeSquare.GetComponent<SpriteRenderer>().sprite = focusedSprite;
                            nodeSquare.GetComponent<SpriteRenderer>().color = Color.green;
                        }
                        if (pathmember) {
                            nodeSquare.GetComponent<SpriteRenderer>().sprite = focusedSprite;
                            nodeSquare.GetComponent<SpriteRenderer>().color = Color.red;
                        }
                        break;
                }
            }
        }

        
        if (unitHQ_code.selected) {
            if (gridMaster.selector.state_selector == 7) {
                if (unitHQ_code.checkCreationSpaces(this, true)) {
                    nodeSquare.GetComponent<SpriteRenderer>().sprite = focusedSprite;
                    nodeSquare.GetComponent<SpriteRenderer>().color = Color.green;
                }
            }
        }

        foreach (GameObject enemy in gridMaster.enemies) {
            enemy_parent enemy_code = (enemy_parent)enemy.GetComponent(typeof(enemy_parent));
            if (enemy_code.selected && !nodeOculto) {
                if (enemy_code.highLightAttackableNodes(this)) {
                    nodeSquare.GetComponent<SpriteRenderer>().sprite = focusedSprite;
                    nodeSquare.GetComponent<SpriteRenderer>().color = Color.magenta;
                }
                if (enemy_code.highLightMoveableNodes(this)) {
                    nodeSquare.GetComponent<SpriteRenderer>().sprite = focusedSprite;
                    nodeSquare.GetComponent<SpriteRenderer>().color = Color.red;
                }
            }
        }

        /*
        if (pathmember) {
            nodeSquare.GetComponent<SpriteRenderer>().sprite = focusedSprite;
            nodeSquare.GetComponent<SpriteRenderer>().color = Color.red;
        }*/
    }
}
