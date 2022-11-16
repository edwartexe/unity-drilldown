using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;   //Allows us to use UI.


public class selector : MonoBehaviour {
    public Grid gridMaster;
    public unit_HQ unitHQ_code => gridMaster.unitHQ_code;
    public Node cursorNode;
    public camera camScript;
    public battleMenu menuUnit;
    public stageMenu menuStage;
    public creatorMenu menuCreator;
    public buygasMenu menuBuyGas;
    public scout_scan_result menuScanResult;

    //public controls p1_controls;

    public bool actionLocked = false;
    public bool movementLocked = false;
    public unit_parent selectedUnit;
    public enemy_parent selectedEnemy;
    public int state_selector = 0; //0 normal, 1 unidad seleccionada
    public int create_selector = 0;


    List<Node> lastPath = null;
    public List<Node> pathToCursor = null;

    // Start is called before the first frame update
    public void initialize() {
        if (gridMaster==null)       { gridMaster = (Grid)GameObject.Find("GameMaster").GetComponent(typeof(Grid)); }
        //unitHQ_code = (unit_HQ)GameObject.Find("unitHQ").GetComponent(typeof(unit_HQ));
        if (menuUnit == null)       { menuUnit = (battleMenu)GameObject.Find("menuUnit").GetComponent(typeof(battleMenu)); }
        if (menuStage == null)      { menuStage = (stageMenu)GameObject.Find("menuStage").GetComponent(typeof(stageMenu)); }
        if (menuCreator == null)    { menuCreator = (creatorMenu)GameObject.Find("menuCreator").GetComponent(typeof(creatorMenu)); }
        if (menuBuyGas == null)     { menuBuyGas = (buygasMenu)GameObject.Find("menuBuyGas").GetComponent(typeof(buygasMenu)); }

        /*if (p1_controls == null) {
            //p1_controls = (controls)GameObject.Find("controls").GetComponent(typeof(controls)); 
            p1_controls = this.gameObject.GetComponent<controls>();
        }*/


        menuUnit.initialize();
        menuStage.initialize();
        menuCreator.initialize();
        menuBuyGas.initialize();
        menuScanResult.initialize();

        menuUnit.gameObject.SetActive(false);
        menuStage.gameObject.SetActive(false);
        menuCreator.gameObject.SetActive(false);
        menuBuyGas.gameObject.SetActive(false);
        menuScanResult.gameObject.SetActive(false);

        if (unitHQ_code != null) {
            unitHQ_code.initialize();
            cursorNode = unitHQ_code.thisNode;
        } else {
            cursorNode = gridMaster.nodeFromWorldPoint(transform.position);
        }
        transform.position = new Vector3(cursorNode.gridPoint.x, cursorNode.gridPoint.y, 0);

        GameObject camera = GameObject.Find("MainCamera");
        camScript = (camera)camera.GetComponent(typeof(camera));
        //camScript.alignCamera(cursorNode, false);
    }

    /*private void OnGUI() {
        GUI.Label(new Rect(10, 310, 300, 20), "Horizontal: " + Input.GetAxisRaw("Horizontal"));
        GUI.Label(new Rect(10, 330, 300, 20), "Vertical: " + Input.GetAxisRaw("Vertical"));
        GUI.Label(new Rect(10, 350, 300, 20), "HorizontalJ: " + Input.GetAxisRaw("HorizontalJ"));
        GUI.Label(new Rect(10, 370, 300, 20), "VerticalJ: " + Input.GetAxisRaw("VerticalJ"));
        GUI.Label(new Rect(10, 390, 300, 20), "HorizontalD: " + Input.GetAxisRaw("HorizontalD"));
        GUI.Label(new Rect(10, 410, 300, 20), "VerticalD: " + Input.GetAxisRaw("VerticalD"));


        GUI.Label(new Rect(410, 310, 300, 20), "Horizontal: " + controls.horizontal);
        GUI.Label(new Rect(410, 330, 300, 20), "Vertical: " + controls.vertical);
        GUI.Label(new Rect(410, 350, 300, 20), "h: " + controls.h_isAxisInUse);
        GUI.Label(new Rect(410, 370, 300, 20), "v: " + controls.v_isAxisInUse);
        GUI.Label(new Rect(410, 390, 300, 20), "SpawnRate: " + controls.SpawnRate);
        GUI.Label(new Rect(410, 410, 300, 20), "timestamp: " + controls.timestamp);
    }*/


    bool nextDirection(int directionModifier) {
        //direccion de movimiento relativa al angulo de camara. basado en un modulo de las 4 direcciones + el indice de cada direccion
        //indices:
        //  3
        //0   2
        //  1
        switch (globals.Mod(directionModifier, 4)) {
            case 0:
                return (cursorNode.leftNode != null);
            case 1:
                return (cursorNode.downNode != null);
            case 2:
                return (cursorNode.rightNode != null);
            case 3:
                return (cursorNode.upNode != null);
            default:
                return false;
        }
    }





    // Update is called once per frame
    void Update() {
        controls.calculate();


        if (actionLocked) { return; }

        
        //----------------------------------------------------------------------------------------------------------------------------------------
        switch (state_selector) {
            case 0: // nada seleccionado
                if (Input.GetButtonDown("Fire1")) {
                    if (!cursorNode.nodeOculto && cursorNode.isThereAUnitHere()) {
                        selectedUnit = cursorNode.unitInThisNode;
                        if (!selectedUnit.alreadyMoved) {
                            selectedUnit.listMovementRange();
                            selectedUnit.selected = true;
                            state_selector = 1;
                            //Debug.Log("found unit");
                        } else {
                            selectedUnit = null;
                            //Debug.Log("unit already moved");
                        }

                    } else {
                        if (!cursorNode.nodeOculto && cursorNode.isThereAnEnemyHere()) {
                            selectedEnemy = cursorNode.enemyInThisNode;
                            selectedEnemy.listMovementRange();
                            selectedEnemy.selected = true;
                            state_selector = 1;
                        } else{
                            if (!cursorNode.nodeOculto && cursorNode.tiletype.Equals("HQ") ) {
                                state_selector = 6;
                                unitHQ_code.selected = true;
                                showCreationMenu();
                            } else {
                                //Debug.Log("no unit");
                            }
                        }

                    }
                }

                //abrir menu de stage
                if (Input.GetButtonDown("Fire2")) {
                    showStageMenu();
                }
                break;
            //--------------------------
            case 1: //unidad seleccionada
                if ((selectedUnit != null) && (selectedUnit.selected)) {
                    //if (selectedUnit.checkDistanceWalk(cursorNode, selectedUnit.thisNode, selectedUnit.thisNode, 0)) {
                    if (selectedUnit.highLightMoveableNodes(cursorNode)) {
                        lastPath = pathToCursor;
                        pathToCursor = pathToAlert(cursorNode, selectedUnit.thisNode, 100);
                        if (lastPath != null) {
                            foreach (Node pathnode in lastPath) {
                                pathnode.pathmember = false;
                            }
                        }
                        foreach (Node pathnode in pathToCursor) {
                            pathnode.pathmember = true;
                        }
                    }

                    if (Input.GetButtonDown("Fire1")) {
                        if (selectedUnit.highLightMoveableNodes(cursorNode)) {
                        /*Debug.Log("move here!");
                        //selectedUnit.lastNode = selectedUnit.thisNode;
                        //selectedUnit.thisNode = this.cursorNode;
                        //selectedUnit.updatePosition();
                        selectedUnit.nextNode = this.cursorNode;
                        selectedUnit.previewPosition();

                        //disengage code
                        selectedUnit.selected = false;
                        selectedUnit.alreadyMoved = true;
                        selectedUnit = null;
                        actionLocked = false;
                        state_selector = 0;
                        if (lastPath != null) {
                            foreach (Node pathnode in lastPath) {
                                pathnode.pathmember = false;
                            }
                        }
                        //open action menu
                        //actionLocked = true;
                        state_selector = 2;*/



                        //Debug.Log("move here!");
                        selectedUnit.nextNode = this.cursorNode;
                            selectedUnit.previewPosition();
                            state_selector = 2;
                            showBattleMenu();
                        }
                    }

                    //cancel movement
                    if (Input.GetButtonDown("Fire2")) {
                        battleMenuCancelHighlightMovement();
                    }

                }

                if ((selectedEnemy != null) && (selectedEnemy.selected)) {

                    //cancel movement
                    if (Input.GetButtonDown("Fire2")) {
                        enemyCancelHighlight();
                    }

                }


                    
                break;
            case 2:
                //durante este estado el menu esta abierto
                //cancel
                /*if (Input.GetButtonDown("Fire2")) {
                    battleMenuCancelMovement();
                }*/
                break;
            case 3:
                //drill
                if (Input.GetButtonDown("Fire1")) {
                    if ((selectedUnit != null) && (selectedUnit.selected)) {
                        if (selectedUnit.checkDrillTargets(this.cursorNode, true)) {
                            //drill action
                            //djkstra between unit and target. range should be a straight line so there's no problem with path
                            List<Node> drillpath = pathToAlert(selectedUnit.nextNode, cursorNode, 99, false);
                            foreach (Node pathmember in drillpath) {
                                performDrilling(pathmember);
                            }

                            //disengage code
                            selectedUnit.lastNode = selectedUnit.thisNode;
                            selectedUnit.thisNode = selectedUnit.nextNode;
                            if (selectedUnit.heldRelic != null) {
                                selectedUnit.heldRelic.thisNode = selectedUnit.thisNode;
                            }
                            ////gridMaster.addGas(-selectedUnit.temp_gas_usage);
                            selectedUnit.updatePosition();
                            selectedUnit.selected = false;
                            selectedUnit.alreadyMoved = true;
                            selectedUnit = null;
                            actionLocked = false;
                            state_selector = 0;
                            //gridMaster.addGas(-globals.action_cost_drill);
                            if (lastPath != null) {
                                foreach (Node pathnode in lastPath) {
                                    pathnode.pathmember = false;
                                }
                            }
                        }
                    }
                }
                //cancel
                if (Input.GetButtonDown("Fire2")) {
                    battleMenuCancelAction();
                }
                break;
            case 4:
                //attack
                if (Input.GetButtonDown("Fire1")) {
                    if ((selectedUnit != null) && (selectedUnit.selected)) {

                        selectedUnit.savedAttackTargets = selectedUnit.listAttackNodes();
                        if (selectedUnit.checkAttackTargets(this.cursorNode, true) && this.cursorNode.isThereAnEnemyHere()) {
                            enemy_parent target = this.cursorNode.enemyInThisNode;
                            target.health -= selectedUnit.attackPower;
                            if (target.health<=0) {
                                //Destroy(target.gameObject);
                                target.getRekt();
                            }
                            //disengage code
                            selectedUnit.lastNode = selectedUnit.thisNode;
                            selectedUnit.thisNode = selectedUnit.nextNode;
                            if (selectedUnit.heldRelic != null) {
                                selectedUnit.heldRelic.thisNode = selectedUnit.thisNode;
                            }
                            ////gridMaster.addGas(-selectedUnit.temp_gas_usage);
                            selectedUnit.updatePosition();
                            selectedUnit.selected = false;
                            selectedUnit.alreadyMoved = true;
                            selectedUnit = null;
                            actionLocked = false;
                            state_selector = 0;
                            //gridMaster.addGas(-globals.action_cost_attack);
                            if (lastPath != null) {
                                foreach (Node pathnode in lastPath) {
                                    pathnode.pathmember = false;
                                }
                            }
                        }
                    }
                }
                //cancel
                if (Input.GetButtonDown("Fire2")) {
                    battleMenuCancelAction();
                }
                break;

            case 5://hold
                if (Input.GetButtonDown("Fire1")) {
                    if ((selectedUnit != null) && (selectedUnit.selected)) {
                        if (selectedUnit.checkHoldTargets(this.cursorNode, true) && this.cursorNode.isThereAnItemHere()) {

                            relic targetrelic = (relic) cursorNode.itemInThisNode;
                            selectedUnit.heldRelic = targetrelic;
                            targetrelic.onTake(true);
                            targetrelic.transform.position = selectedUnit.transform.position;
                            //disengage code
                            selectedUnit.lastNode = selectedUnit.thisNode;
                            selectedUnit.thisNode = selectedUnit.nextNode;
                            if (selectedUnit.heldRelic != null) {
                                selectedUnit.heldRelic.thisNode = selectedUnit.thisNode;
                            }
                            ////gridMaster.addGas(-selectedUnit.temp_gas_usage);
                            selectedUnit.updatePosition();
                            selectedUnit.selected = false;
                            selectedUnit.alreadyMoved = true;
                            selectedUnit = null;
                            actionLocked = false;
                            state_selector = 0;
                            //gridMaster.addGas(-globals.action_cost_hold);
                            if (lastPath != null) {
                                foreach (Node pathnode in lastPath) {
                                    pathnode.pathmember = false;
                                }
                            }
                            gridMaster.wakeUpStatues();//only for wake up boss check

                        }
                    }
                }
                //cancel
                if (Input.GetButtonDown("Fire2")) {
                    battleMenuCancelAction();
                }
                break;

            case 6://create unit
                //buffer state
                //create unit menu active
                break;

            case 7:
                //set created unit in place
                if (Input.GetButtonDown("Fire1")) {
                    if ((unitHQ_code != null) && unitHQ_code.selected && unitHQ_code.checkCreationSpaces(this.cursorNode, true) && (create_selector!=0) ) {

                        int createcost = globals.drill_create_cost;
                        unit_parent selected_creation_unit = gridMaster.unit_drill_prefab;
                        switch (create_selector) {
                            case 1:
                                createcost = globals.drill_create_cost;
                                selected_creation_unit = gridMaster.unit_drill_prefab;
                                break;
                            case 2:
                                createcost = globals.tank_create_cost;
                                selected_creation_unit = gridMaster.unit_tank_prefab;
                                break;
                            case 3:
                                createcost = globals.scout_create_cost;
                                selected_creation_unit = gridMaster.unit_scout_prefab;
                                break;
                            case 4:
                                createcost = globals.bomb_create_cost;
                                selected_creation_unit = gridMaster.unit_bomb_prefab;
                                break;
                            case 5:
                                createcost = globals.tank_create_cost;
                                selected_creation_unit = gridMaster.unit_armoredS_prefab;
                                break;
                            case 6:
                                createcost = globals.superDrill_create_cost;
                                selected_creation_unit = gridMaster.unit_superDrill_prefab;
                                break;
                        }

                        if (gridMaster.recursoDinero >= 0) {
                            //create unit
                            unit_parent new_creation_unit = Instantiate(selected_creation_unit, new Vector3(cursorNode.gridPoint.x, cursorNode.gridPoint.y, 0), Quaternion.identity, gridMaster.groupUnit);
                            new_creation_unit.alreadyMoved = true; 
                            gridMaster.addDinero(-createcost);
                            gridMaster.updateActors();

                            //unset state
                            unitHQ_code.selected = false;
                            unitHQ_code.alreadyMoved = true;
                            actionLocked = false;
                            state_selector = 0;
                        }

                            
                    }
                }

                //cancel
                if (Input.GetButtonDown("Fire2")) {
                    creationMenuCancelCreation();
                }
                break;

            case 8://drop
                if (Input.GetButtonDown("Fire1")) {
                    if ((selectedUnit != null) && (selectedUnit.selected)) {
                        if (selectedUnit.checkDropTargets(cursorNode, true)) {

                            if (cursorNode.tiletype.Equals("HQ")) {
                                selectedUnit.heldRelic.transform.position = new Vector3(cursorNode.gridPoint.x, cursorNode.gridPoint.y, 0);
                                selectedUnit.heldRelic.onSave();
                                selectedUnit.heldRelic = null;
                            } else {
                                selectedUnit.heldRelic.transform.position = new Vector3(cursorNode.gridPoint.x, cursorNode.gridPoint.y, 0);
                                selectedUnit.heldRelic.onReveal();
                                selectedUnit.heldRelic = null;
                            }
                            //disengage code
                            selectedUnit.lastNode = selectedUnit.thisNode;
                            selectedUnit.thisNode = selectedUnit.nextNode;
                            if (selectedUnit.heldRelic != null) {
                                selectedUnit.heldRelic.thisNode = selectedUnit.thisNode;
                            }
                            ////gridMaster.addGas(-selectedUnit.temp_gas_usage);
                            selectedUnit.updatePosition();
                            selectedUnit.selected = false;
                            selectedUnit.alreadyMoved = true;
                            selectedUnit = null;
                            actionLocked = false;
                            state_selector = 0;
                            if (lastPath != null) {
                                foreach (Node pathnode in lastPath) {
                                    pathnode.pathmember = false;
                                }
                            }

                        }
                    }
                }
                //cancel
                if (Input.GetButtonDown("Fire2")) {
                    battleMenuCancelAction();
                }
                break;
            case 9:
                //go bomb
                if (Input.GetButtonDown("Fire1")) {
                    if ((selectedUnit != null) && (selectedUnit.selected)) {

                        //this.cursorNode.revealNode();
                        GameObject explotion_inst = Instantiate((GameObject)Resources.Load("bomb_explotion"), selectedUnit.transform.position, Quaternion.identity, gridMaster.groupUnit);
                        Destroy(explotion_inst, 1.0f);
                        //GameObject dust = Instantiate((GameObject)Resources.Load("dust_ps2"));
                        //dust.transform.position = new Vector3(selectedUnit.nextNode.gridPoint.x, selectedUnit.nextNode.gridPoint.y, -1);
                        //Destroy(dust, 2.0f);

                        foreach (Node nnn in selectedUnit.listBombNodes() ) {
                            if (nnn.nodeOculto) {
                                nnn.revealNode();
                            } else{
                                if (nnn.isThereAnEnemyHere()) {
                                    nnn.enemyInThisNode.health -= globals.bombExplodeStat;
                                    if (nnn.enemyInThisNode.health <= 0) {
                                        nnn.enemyInThisNode.getRekt();
                                    }
                                }
                                //friendly fire off is more fun
                                /*if (nnn.isThereAUnitHere()) {
                                    nnn.unitInThisNode.health -= globals.bombExplodeStat;
                                    if (nnn.unitInThisNode.health <= 0) {
                                        nnn.unitInThisNode.getRekt();
                                    }
                                }*/
                            }
                        }
                        //disengage code
                        selectedUnit.lastNode = selectedUnit.thisNode;
                        selectedUnit.thisNode = selectedUnit.nextNode;
                        if (selectedUnit.heldRelic != null) {
                            selectedUnit.heldRelic.thisNode = selectedUnit.thisNode;
                        }
                        ////gridMaster.addGas(-selectedUnit.temp_gas_usage);
                        selectedUnit.updatePosition();
                        selectedUnit.selected = false;
                        selectedUnit.alreadyMoved = true;
                        unit_parent tempunit = selectedUnit;
                        selectedUnit = null;
                        actionLocked = false;
                        state_selector = 0;
                        //gridMaster.addGas(-globals.action_cost_base_bomb);

                        //destroy bomb
                        tempunit.detonate();

                        if (lastPath != null) {
                            foreach (Node pathnode in lastPath) {
                                pathnode.pathmember = false;
                            }
                        }
                            
                    }
                }
                //cancel
                if (Input.GetButtonDown("Fire2")) {
                    battleMenuCancelAction();
                }
                break;

            case 10:
            //scanplace
                movementLocked = true;
                if ((selectedUnit != null) && (selectedUnit.selected) && (controls.horizontal != 0 || controls.vertical != 0)) {
                    if (controls.horizontal == -1 && controls.vertical == 0) {
                        selectedUnit.scanDirection = 0;
                        selectedUnit.savedScanTargets = selectedUnit.getScanTargets2();
                    }
                    if (controls.horizontal == 0 && controls.vertical == -1) {
                        selectedUnit.scanDirection = 1;
                        selectedUnit.savedScanTargets = selectedUnit.getScanTargets2();
                    }
                    if (controls.horizontal == 1 && controls.vertical == 0) {
                        selectedUnit.scanDirection = 2;
                        selectedUnit.savedScanTargets = selectedUnit.getScanTargets2();
                    }
                    if (controls.horizontal == 0 && controls.vertical == 1) {
                        selectedUnit.scanDirection = 3;
                        selectedUnit.savedScanTargets = selectedUnit.getScanTargets2();
                    }
                }


                if (Input.GetButtonDown("Fire1")) {
                    if ((selectedUnit != null) && (selectedUnit.selected)) {
                        int enemyScan = 0; 
                        int enemyStrongScan = 0;
                        int relicScan = 0;
                        int nestScan = 0;

                        //disengage part 1
                        selectedUnit.lastNode = selectedUnit.thisNode;
                        selectedUnit.thisNode = selectedUnit.nextNode;
                        if (selectedUnit.heldRelic != null) {
                            selectedUnit.heldRelic.thisNode = selectedUnit.nextNode;
                        }
                        ////gridMaster.addGas(-selectedUnit.temp_gas_usage);
                        selectedUnit.updatePosition();

                        selectedUnit.savedScanTargets=selectedUnit.getScanTargets2();
                        foreach (Node nnn in selectedUnit.savedScanTargets) {
                            if (nnn.isThereAnEnemyHere()) {
                                if (nnn.enemyInThisNode.GetComponent<enemy_parent>().enemyClass.Equals(globals.nestName)) {
                                    nestScan++;
                                } else {
                                    if (nnn.enemyInThisNode.GetComponent<enemy_parent>().maxHealth>20) {
                                        enemyStrongScan++;
                                    } else {
                                        enemyScan++;
                                    }
                                }
                            }
                            if (nnn.isThereAnItemHere() && !nnn.isHQhere() && (!nnn.isThereAUnitHere() || (nnn.isThereAUnitHere() && nnn.itemInThisNode.state!=2 )) ) {
                                relicScan++;
                                nnn.markInterest();
                            }
                        }

                            
                        state_selector = 11;
                        //actionLocked = true;
                        movementLocked = false;
                        menuScanResult.gameObject.SetActive(true);
                        menuScanResult.setValues(selectedUnit,enemyScan,enemyStrongScan,nestScan,relicScan);
                        //Debug.Log("Enemy: " + enemyScan + "Big-Enemy: " + enemyStrongScan + " Nest: " + nestScan + " Relic: " + relicScan);

                        //TODO
                        //mostrar enemigos y relics contados


                        //disengage code part 2
                        /*selectedUnit.selected = false;
                        selectedUnit = null;
                        actionLocked = false;
                        //gridMaster.addGas(-globals.action_cost_scan);
                        if (lastPath != null) {
                            foreach (Node pathnode in lastPath) {
                                pathnode.pathmember = false;
                            }
                        }*/

                    }
                }
                //cancel
                if (Input.GetButtonDown("Fire2")) {
                    movementLocked = false;
                    battleMenuCancelAction();
                }
                break;
            case 11:
                /*if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2")) {
                    closeScanMenu();
                }*/
                    break;
        }

        //Check if we have a non-zero value for horizontal or vertical

        int G_horizontal=0;
        int G_vertical=0;
        if (!movementLocked) {
            if (controls.horizontal != 0 || controls.vertical != 0) {
                G_horizontal = controls.horizontal;
                G_vertical = controls.vertical;
                //Debug.Log("horizontal: " + horizontal + " vertical: " + vertical);
            }





            if (G_horizontal == -1 && G_vertical == 0 && nextDirection(0)) {
                //changeIcon();
                cursorNode = cursorNode.leftNode;
                transform.position = new Vector3(cursorNode.gridPoint.x, cursorNode.gridPoint.y, 0);
                //camScript.alignCamera(cursorNode, false);

            }
            if (G_horizontal == 0 && G_vertical == -1 && nextDirection(1)) {
                //changeIcon();
                cursorNode = cursorNode.downNode;
                transform.position = new Vector3(cursorNode.gridPoint.x, cursorNode.gridPoint.y, 0);
                //camScript.alignCamera(cursorNode, false);
            }
            if (G_horizontal == 1 && G_vertical == 0 && nextDirection(2)) {
                //changeIcon();
                cursorNode = cursorNode.rightNode;
                transform.position = new Vector3(cursorNode.gridPoint.x, cursorNode.gridPoint.y, 0);
                //camScript.alignCamera(cursorNode, false);
            }
            if (G_horizontal == 0 && G_vertical == 1 && nextDirection(3)) {
                //changeIcon();
                cursorNode = cursorNode.upNode;
                transform.position = new Vector3(cursorNode.gridPoint.x, cursorNode.gridPoint.y, 0);
                //camScript.alignCamera(cursorNode, false);
            }
        }


        /*if (controls.vertical != 0 ||
                controls.horizontal != 0 ||
                Input.GetButtonDown("Fire1") ||
                Input.GetButtonDown("Fire2")
            ) {
            gridMaster.updateAllNodes();
        }*/

        //updategui();
    }

    /*public void updategui() {
        gridMaster.guimanager.updateNode(cursorNode);
        if (!cursorNode.nodeOculto && cursorNode.isThereAUnitHere()) { 
            gridMaster.guimanager.updateUnit(cursorNode.unitInThisNode); 
        } else {
            if (!cursorNode.nodeOculto && cursorNode.isThereAnEnemyHere()) {
                gridMaster.guimanager.updateEnemy(cursorNode.enemyInThisNode);
            } else {
                gridMaster.guimanager.hideUnitPanel();
            }
        }
    }*/

    public void performDrilling(Node targetNode) {
        //drill action
        if (targetNode.nodeOculto) {
            targetNode.revealNode();
            GameObject dust = Instantiate((GameObject)Resources.Load("dust_ps"));
            dust.transform.position = new Vector3(targetNode.gridPoint.x, targetNode.gridPoint.y, -1);
            Destroy(dust, 1.0f);
        }
    }

    public void showStageMenu() {
        this.actionLocked = true;
        menuStage.gameObject.SetActive(true);
    }

    //----------------start battle menu

    public void showBattleMenu() {
        this.actionLocked = true;
        menuUnit.gameObject.SetActive(true);
    }

    public void battleMenuMove() { }

    public void battleMenuDrill() {
        state_selector = 3;
    }

    public void battleMenuAttack() {
        state_selector = 4;
        ////gridMaster.addGas(-selectedUnit.temp_gas_usage);
    }
    
    public void battleMenuHold() {
        state_selector = 5;
    }
    
    public void battleMenuDrop() {
        state_selector = 8;
    }
    public void battleMenuDetonate() {
        state_selector = 9;
    }

    public void battleMenuScan() {
        state_selector = 10;
    }
    

    public void battleMenuWait() {
        if ((selectedUnit != null) && (selectedUnit.selected)) {
            //disengage code
            selectedUnit.lastNode = selectedUnit.thisNode;
            //selectedUnit.thisNode = this.cursorNode;
            selectedUnit.thisNode = selectedUnit.nextNode;
            ////gridMaster.addGas(-selectedUnit.temp_gas_usage);
            selectedUnit.updatePosition();
            selectedUnit.selected = false;
            selectedUnit.alreadyMoved = true;
            selectedUnit = null;
            actionLocked = false;
            state_selector = 0;
            if (lastPath != null) {
                foreach (Node pathnode in lastPath) {
                    pathnode.pathmember = false;
                }
            }
        }

    }

    public void battleMenuCancelHighlightMovement() {
        if (selectedUnit != null) {
            selectedUnit.selected = false;
            selectedUnit = null;
            //Debug.Log("unit unselected");
            state_selector = 0;
            if (lastPath != null) {
                foreach (Node pathnode in lastPath) {
                    pathnode.pathmember = false;
                }
            }
        } else {
            //Debug.Log("no unit asigned");
        }
    }

    public void battleMenuCancelMovement() {
        if (selectedUnit != null) {
            selectedUnit.StopCoroutine("MoveAlongPath");
            actionLocked = false;
            state_selector = 1;
            selectedUnit.updatePosition();
        } else {
            //Debug.Log("no unit asigned");
        }
    }

    public void battleMenuCancelAction() {
        this.state_selector = 2;
        this.actionLocked = true;
        this.cursorNode = selectedUnit.nextNode;
        transform.position = new Vector3(cursorNode.gridPoint.x, cursorNode.gridPoint.y, 0);
        menuUnit.gameObject.SetActive(true);
    }

    //----------------end battle menu

    public void showCreationMenu() {
        this.actionLocked = true;
        menuCreator.gameObject.SetActive(true);
    }

    public void creationMenuCancelCreation() {
        unitHQ_code.selected = false;
        actionLocked = false;
        state_selector = 0;
    }


    public void enemyCancelHighlight() {
        if (selectedEnemy != null) {
            selectedEnemy.selected = false;
            selectedEnemy = null;
            state_selector = 0;
        } 
    }

    public void closeScanMenu() {
        //state 10 disengage code part 2
        selectedUnit.alreadyMoved = true;
        selectedUnit.selected = false;
        selectedUnit = null;
        actionLocked = false;
        state_selector = 0;
        //gridMaster.addGas(-globals.action_cost_scan);
        if (lastPath != null) {
            foreach (Node pathnode in lastPath) {
                pathnode.pathmember = false;
            }
        }
    }
    public void closeTutorial() {
        actionLocked = false;
        state_selector = 0;
    }

    //----------------

    public void turnOnUnit() {
        if (selectedUnit != null) {
            selectedUnit.alreadyMoved = true;
            selectedUnit.isActive = true;
            selectedUnit.selected = false;
            selectedUnit = null;
            actionLocked = false;
            state_selector = 0;
        }
    }

    public void turnOffUnit() {
        if ((selectedUnit != null) && (selectedUnit.selected)) {
            selectedUnit.lastNode = selectedUnit.thisNode;
            selectedUnit.thisNode = selectedUnit.nextNode;
            ////gridMaster.addGas(-selectedUnit.temp_gas_usage);
            selectedUnit.updatePosition();
            selectedUnit.selected = false;
            selectedUnit.alreadyMoved = true;
            selectedUnit.isActive = false; //main effect.
            selectedUnit = null;
            actionLocked = false;
            state_selector = 0;
            if (lastPath != null) {
                foreach (Node pathnode in lastPath) {
                    pathnode.pathmember = false;
                }
            }
        }
    }




    //----------------------------------------------------- PATH FINDING -----------------------------------------------------------
    //buscar un camino de nodos desde el nodo actual a donde esta el jugador -------------------------------------------------------
    public virtual List<Node> pathToAlert(Node targetNode, Node startNode, int limit, bool friendlyNodes = true) {

        List<DjikstraNodes> unvisited = new List<DjikstraNodes>();
        Stack<Node> path = new Stack<Node>();
        List<Node> returnlist = new List<Node>();

        foreach (Node n in gridMaster.GetComponent<Grid>().grid) {
            DjikstraNodes dj;
            if (n.Equals(startNode)) {
                //Debug.Log ("thats a start");
                dj = new DjikstraNodes(n, 0);
            } else {
                dj = new DjikstraNodes(n, 9999);
            }
            unvisited.Add(dj);
        }


        DjikstraNodes searcher;
        int countYetToVisit = 1;
        int iteration = 0;

        //iniciar circlo de busqueda djikstra
        while (countYetToVisit > 0 && iteration < limit) {

            int startIndex = unvisited.FindIndex(x => x.visited == false);
            searcher = unvisited[startIndex];
            foreach (DjikstraNodes dj in unvisited) {
                if (dj.weight < searcher.weight && !dj.visited) {
                    searcher = dj;
                }
            }
            int searcherIndex = unvisited.FindIndex(x => x.Equals(searcher));
            //Debug.Log ("-- searcher["+searcherIndex+"] is at x: "+searcher.node.totalWorldPoint().x +" z: "+searcher.node.totalWorldPoint().z  );

            bool inWalkDistance = searcher.node.walkable;
            if (searcher.weight == 9999) { inWalkDistance = false; }

            if (inWalkDistance) {
                //desde el nodo objetivo leer en reversa y regresar el camino encontrado
                if (searcher.node.Equals(targetNode)) {
                    //Debug.Log ("found target at "+searcher.totalWorldPoint().x+", "+searcher.totalWorldPoint().y+", "+searcher.totalWorldPoint().z);
                    //Debug.Log ("found it");

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
                        if (friendlyNodes)  {
                            //the valid nodes that are allowed for the path are unit-friendly nodes. default true
                            if ((!unvisited[temp].node.tiletype.Contains("bloqueado") 
                            && !unvisited[temp].node.tiletype.Contains("GuardOnly")
                            && !unvisited[temp].node.tiletype.Contains("locked")
                            && !unvisited[temp].node.tiletype.Contains("sphinxstatue")
                            && !unvisited[temp].node.tiletype.Contains("Guardian")
                            && !unvisited[temp].node.tiletype.Contains("nest") 
                            && !unvisited[temp].node.tiletype.Contains("bottomless"))
                            && (unvisited[temp].node.walkable)
                            && (!unvisited[temp].node.nodeOculto)
                            && (searcher.weight + 1 < unvisited[temp].weight)
                            && !unvisited[temp].node.isThereAnEnemyHere()) {
                                unvisited[temp].weight = searcher.weight + 1;
                                unvisited[temp].prevNode = searcher;
                            }
                        } else {
                            //or if we need something more generic set the parameter to false
                            if ( searcher.weight + 1 < unvisited[temp].weight) {
                                unvisited[temp].weight = searcher.weight + 1;
                                unvisited[temp].prevNode = searcher;
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
            //Debug.Log ("countYetToVisit "+countYetToVisit);
            iteration++;
        }


        returnlist = new List<Node>();
        returnlist.AddRange(path);
        return returnlist;
    }
}
