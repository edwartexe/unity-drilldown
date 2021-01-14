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

    private Vector2 touchOrigin = -Vector2.one; //Used to store location of screen touch origin for mobile controls.
    int G_horizontal;
    int G_vertical;
    bool h_isAxisInUse = false;
    bool v_isAxisInUse = false;
    private float SpawnRate = 0.25F;
    private float timestamp = 0F;
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
    

    private void OnGUI() {
        //GUI.Label(new Rect(10, 10, 100, 100), "G_horizontal: " + G_horizontal + " G_vertical: " + G_vertical);
    }

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
        int horizontal = 0;     //Used to store the horizontal move direction.
        int vertical = 0;       //Used to store the vertical move direction.
   
//empanada
#if !UNITY_EDITOR && UNITY_WEBGL
        UnityEngine.WebGLInput.captureAllKeyboardInput = true; // or false
#endif
        //Check if we are running either in the Unity editor or in a standalone build.  
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER


        //Get input from the input manager, round it to an integer and store it
        
            if (Input.GetAxisRaw("Horizontal") != 0) {
                if (h_isAxisInUse == false || Time.time >= timestamp) {
                    horizontal = (int)(Input.GetAxisRaw("Horizontal"));
                    h_isAxisInUse = true;
                    timestamp = Time.time + SpawnRate;
                }
            }
            if (Input.GetAxisRaw("Horizontal") == 0) {
                h_isAxisInUse = false;
            }

            if (Input.GetAxisRaw("Vertical") != 0) {
                if (v_isAxisInUse == false || Time.time >= timestamp) {
                    vertical = (int)(Input.GetAxisRaw("Vertical"));
                    v_isAxisInUse = true;
                    timestamp = Time.time + SpawnRate;
                }
            }
            if (Input.GetAxisRaw("Vertical") == 0) {
                v_isAxisInUse = false;
            }
            //Check if moving horizontally, if so set vertical to zero.
            if (horizontal != 0) {
                vertical = 0;
            }
        
            //----------------------------------------------------------------------------------------------------------------------------------------
            switch (state_selector) {
                //--------------------------
                case 0: // nada seleccionado
                    if (Input.GetKeyDown("z")) {
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
                    if (Input.GetKeyDown("x")) {
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

                        if (Input.GetKeyDown("z")) {
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
                                movementLocked = false;
                                state_selector = 0;
                                if (lastPath != null) {
                                    foreach (Node pathnode in lastPath) {
                                        pathnode.pathmember = false;
                                    }
                                }
                                //open action menu
                                //movementLocked = true;
                                state_selector = 2;*/



                                //Debug.Log("move here!");
                                selectedUnit.nextNode = this.cursorNode;
                                selectedUnit.previewPosition();
                                state_selector = 2;
                                showBattleMenu();
                            }
                        }

                        //cancel movement
                        if (Input.GetKeyDown("x")) {
                            battleMenuCancelHighlightMovement();
                        }

                    }

                    if ((selectedEnemy != null) && (selectedEnemy.selected)) {

                        //cancel movement
                        if (Input.GetKeyDown("x")) {
                            enemyCancelHighlight();
                        }

                    }


                    
                    break;
                case 2:
                    //durante este estado el menu esta abierto
                    //cancel
                    /*if (Input.GetKeyDown("x")) {
                        battleMenuCancelMovement();
                    }*/
                    break;
                case 3:
                    //drill
                    if (Input.GetKeyDown("z")) {
                        if ((selectedUnit != null) && (selectedUnit.selected)) {
                            if (selectedUnit.checkDrillTargets(this.cursorNode, true) && this.cursorNode.nodeOculto) {
                                this.cursorNode.revealNode();
                                GameObject dust = Instantiate((GameObject)Resources.Load("dust_ps"));//, cursorNode.gridPoint, Quaternion.identity, gridMaster.groupUnit
                                dust.transform.position = new Vector3(cursorNode.gridPoint.x, cursorNode.gridPoint.y, -1);
                                Destroy(dust, 1.0f);
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
                                movementLocked = false;
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
                    if (Input.GetKeyDown("x")) {
                        battleMenuCancelAction();
                    }
                    break;
                case 4:
                    //attack
                    if (Input.GetKeyDown("z")) {
                        if ((selectedUnit != null) && (selectedUnit.selected)) {
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
                                movementLocked = false;
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
                    if (Input.GetKeyDown("x")) {
                        battleMenuCancelAction();
                    }
                    break;

                case 5://hold
                    if (Input.GetKeyDown("z")) {
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
                                movementLocked = false;
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
                    if (Input.GetKeyDown("x")) {
                        battleMenuCancelAction();
                    }
                    break;

                case 6://create unit
                    //buffer state
                    //create unit menu active
                    break;

                case 7:
                    //set created unit in place
                    if (Input.GetKeyDown("z")) {
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
                                movementLocked = false;
                                state_selector = 0;
                            }

                            
                        }
                    }

                    //cancel
                    if (Input.GetKeyDown("x")) {
                        creationMenuCancelCreation();
                    }
                    break;

                case 8://drop
                    if (Input.GetKeyDown("z")) {
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
                                movementLocked = false;
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
                    if (Input.GetKeyDown("x")) {
                        battleMenuCancelAction();
                    }
                    break;
                case 9:
                    //go bomb
                    if (Input.GetKeyDown("z")) {
                        if ((selectedUnit != null) && (selectedUnit.selected)) {

                            //this.cursorNode.revealNode();
                            GameObject explotion_inst = Instantiate((GameObject)Resources.Load("bomb_explotion"), this.transform.position, Quaternion.identity, gridMaster.groupUnit);
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
                            movementLocked = false;
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
                    if (Input.GetKeyDown("x")) {
                        battleMenuCancelAction();
                    }
                    break;

                case 10:
                    //scanplace
                    movementLocked = true;
                    if ((selectedUnit != null) && (selectedUnit.selected) && (horizontal != 0 || vertical != 0)) {
                        if (horizontal == -1 && vertical == 0) {
                            selectedUnit.scanDirection = 0;
                            selectedUnit.savedScanTargets = selectedUnit.getScanTargets2();

                    }
                    if (horizontal == 0 && vertical == -1) {
                            selectedUnit.scanDirection = 1;
                            selectedUnit.savedScanTargets = selectedUnit.getScanTargets2();

                    }
                    if (horizontal == 1 && vertical == 0) {
                            selectedUnit.scanDirection = 2;
                            selectedUnit.savedScanTargets = selectedUnit.getScanTargets2();
                    }
                    if (horizontal == 0 && vertical == 1) {
                            selectedUnit.scanDirection = 3;
                            selectedUnit.savedScanTargets = selectedUnit.getScanTargets2();
                    }
                }


                    if (Input.GetKeyDown("z")) {
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
                                }
                            }

                            
                            state_selector = 11;
                            //movementLocked = true;
                            movementLocked = false;
                            menuScanResult.gameObject.SetActive(true);
                            menuScanResult.setValues(selectedUnit,enemyScan,enemyStrongScan,nestScan,relicScan);
                            //Debug.Log("Enemy: " + enemyScan + "Big-Enemy: " + enemyStrongScan + " Nest: " + nestScan + " Relic: " + relicScan);

                            //TODO
                            //mostrar enemigos y relics contados


                            //disengage code part 2
                            /*selectedUnit.selected = false;
                            selectedUnit = null;
                            movementLocked = false;
                            //gridMaster.addGas(-globals.action_cost_scan);
                            if (lastPath != null) {
                                foreach (Node pathnode in lastPath) {
                                    pathnode.pathmember = false;
                                }
                            }*/

                        }
                    }
                    //cancel
                    if (Input.GetKeyDown("x")) {
                        battleMenuCancelAction();
                    }
                    break;
                case 11:
                    /*if (Input.GetKeyDown("z") || Input.GetKeyDown("x")) {
                        closeScanMenu();
                    }*/
                        break;
            }





        //Check if we are running on iOS, Android, Windows Phone 8 or Unity iPhone
#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
        //Check if Input has registered more than zero touches
        if (Input.touchCount > 0){
                //Store the first touch detected.
                Touch myTouch = Input.touches[0];

                //Check if the phase of that touch equals Began
                if (myTouch.phase == TouchPhase.Began){
                    //If so, set touchOrigin to the position of that touch
                    touchOrigin = myTouch.position;
                }

                //If the touch phase is not Began, and instead is equal to Ended and the x of touchOrigin is greater or equal to zero:
                else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0) {
                    //Set touchEnd to equal the position of this touch
                    Vector2 touchEnd = myTouch.position;

                    //Calculate the difference between the beginning and end of the touch on the x axis.
                    float x = touchEnd.x - touchOrigin.x;

                    //Calculate the difference between the beginning and end of the touch on the y axis.
                    float y = touchEnd.y - touchOrigin.y;

                    //Set touchOrigin.x to -1 so that our else if statement will evaluate false and not repeat immediately.
                    touchOrigin.x = -1;

                    //Check if the difference along the x axis is greater than the difference along the y axis.
                    if (Mathf.Abs(x) > Mathf.Abs(y))
                        //If x is greater than zero, set horizontal to 1, otherwise set it to -1
                        horizontal = x > 0 ? 1 : -1;
                    else
                        //If y is greater than zero, set horizontal to 1, otherwise set it to -1
                        vertical = y > 0 ? 1 : -1;
                }
            }

#endif //End of mobile platform dependendent compilation section started above with #elif
        //Check if we have a non-zero value for horizontal or vertical
        if (!movementLocked) {
            if (horizontal != 0 || vertical != 0) {
                //Call AttemptMove passing in the generic parameter Wall, since that is what Player may interact with if they encounter one (by attacking it)
                //Pass in horizontal and vertical as parameters to specify the direction to move Player in.
                //AttemptMove<Wall>(horizontal, vertical);
                G_horizontal = horizontal;
                G_vertical = vertical;
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
        G_horizontal = 0;
        G_vertical = 0; 


    }








    private void LateUpdate() {
        updategui();
    }

    public void updategui() {
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
       
    }

    public void showStageMenu() {
        this.movementLocked = true;
        menuStage.gameObject.SetActive(true);
    }

    //----------------start battle menu

    public void showBattleMenu() {
        this.movementLocked = true;
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
            movementLocked = false;
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
            movementLocked = false;
            state_selector = 1;
            selectedUnit.updatePosition();
        } else {
            //Debug.Log("no unit asigned");
        }
    }

    public void battleMenuCancelAction() {
        this.movementLocked = true;
        this.state_selector = 2;
        this.cursorNode = selectedUnit.nextNode;
        transform.position = new Vector3(cursorNode.gridPoint.x, cursorNode.gridPoint.y, 0);
        menuUnit.gameObject.SetActive(true);
    }

    //----------------end battle menu

    public void showCreationMenu() {
        this.movementLocked = true;
        menuCreator.gameObject.SetActive(true);
    }

    public void creationMenuCancelCreation() {
        unitHQ_code.selected = false;
        movementLocked = false;
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
        movementLocked = false;
        state_selector = 0;
        //gridMaster.addGas(-globals.action_cost_scan);
        if (lastPath != null) {
            foreach (Node pathnode in lastPath) {
                pathnode.pathmember = false;
            }
        }
    }
    public void closeTutorial() {
        movementLocked = false;
        state_selector = 0;
    }

    //----------------

    public void turnOnUnit() {
        if (selectedUnit != null) {
            selectedUnit.alreadyMoved = true;
            selectedUnit.isActive = true;
            selectedUnit.selected = false;
            selectedUnit = null;
            movementLocked = false;
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
            movementLocked = false;
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
    public virtual List<Node> pathToAlert(Node targetNode, Node startNode, int limit) {

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
                        if ((!unvisited[temp].node.tiletype.Contains("bloqueado") 
                        && !unvisited[temp].node.tiletype.Contains("GuardOnly")
                        && !unvisited[temp].node.tiletype.Contains("locked")
                        && !unvisited[temp].node.tiletype.Contains("sphinxstatue")
                        && !unvisited[temp].node.tiletype.Contains("nest") 
                        && !unvisited[temp].node.tiletype.Contains("bottomless"))
                        && (unvisited[temp].node.walkable)
                        && (!unvisited[temp].node.nodeOculto)
                        && (searcher.weight + 1 < unvisited[temp].weight)
                        && !unvisited[temp].node.isThereAnEnemyHere()) {
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
            //Debug.Log ("countYetToVisit "+countYetToVisit);
            iteration++;
        }


        returnlist = new List<Node>();
        returnlist.AddRange(path);
        return returnlist;
    }
}
