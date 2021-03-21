using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class Grid : MonoBehaviour {
    [SerializeField] protected List<TextAsset> mapFile;
    [SerializeField] protected int mapIndex = 0;
    [SerializeField] protected menuStageResult stageResult;
    public tutorialScreen menuTutorial;
    //public GameObject cursor;
    public Transform player;
	public LayerMask unwalkableMask;
	public Vector2 gridWorldSize;

	public Node[,] grid;
    public selector selector;
    public unit_HQ unitHQ_code;

    float nodeDiameter=1;
	public int gridSizeX, gridSizeZ;

    
	public GameObject[] enemies;
	public GameObject[] units;
    public enemy_thief_nest[] allNests;
    //public GameObject[] items;
    public List<relic> relics;
    public int relicounter;
    public int relicsaved;
    private int gamestate=0; //0=not started, 1=playing, 2=loose, 3=win
    public Sprite[] minesprites2;

    public unit_HQ unit_HQ_prefab;
    public unit_parent unit_drill_prefab; 
    public unit_parent unit_tank_prefab;
    public unit_parent unit_scout_prefab;
    public unit_parent unit_bomb_prefab;
    public unit_parent unit_armoredS_prefab;

    public enemy_parent enemy_worm_prefab;
    public enemy_shark enemy_shark_prefab;
    public enemy_thief enemy_thief_prefab;
    public enemy_thief_nest enemy_nest_prefab;
    public enemy_unlocker enemy_unlocker_prefab;
    public enemy_sphinx enemy_sphinx_prefab;

    public relic relic_prefab;
    public bool playersturn;


    public GUImanager guimanager;
    int turnCount =1;
    public int recursoGas, recursoDinero, recursoDineroInicial;
    int tutorialIndexForLater = 0;

    [SerializeField] protected Transform plane;
    public Transform groupNode;
    public Transform groupEnemy;
    public Transform groupUnit;
    public Transform groupItems;



    // Use this for initialization
    void Awake () {
        mapFile = globals.mapFiles;

        initialize();
    }

    public void initialize() {
        //plane = this.transform.Find("Quad").transform;

        //mc movement
        if (selector == null) { selector = (selector)GameObject.Find("Selector").GetComponent(typeof(selector)); }
        if (guimanager == null) { guimanager = (GUImanager)GameObject.Find("GUImanager").GetComponent(typeof(GUImanager)); }


        if (stageResult == null) { stageResult = (menuStageResult)GameObject.Find("stageResult").GetComponent(typeof(menuStageResult)); }
        stageResult.gridMaster = this;
        stageResult.initialize();
        stageResult.gameObject.SetActive(false);

        if (menuTutorial == null) { menuTutorial = (tutorialScreen)GameObject.Find("tutorialScreen").GetComponent(typeof(tutorialScreen)); }
        menuTutorial.initialize();
        menuTutorial.gameObject.SetActive(false);

        minesprites2 = Resources.LoadAll<Sprite>("mine_sprites2");
        
        readFile();
        updateActors();

        playersturn = true;
        turnCount = 1;
        selector.actionLocked = false;
        selector.initialize();
        showTutorial();
    }


    public void updateActors() {
        //temporal enemy sightline
        enemies = GameObject.FindGameObjectsWithTag("GuardEnemy");
        allNests = GameObject.FindObjectsOfType<enemy_thief_nest>();
        //items = GameObject.FindGameObjectsWithTag("DungeonItems");
        //temporal inkspawn locations
        units = GameObject.FindGameObjectsWithTag("PlayerUnit");


    }

    void readFile(){
        //leer archivo
        //string[] text = System.IO.File.ReadAllLines (@"Assets\MyFolder\MAP_FILES\map_grid_1.txt");
		string textFile = mapFile[mapIndex].text;
		string[] text = textFile.Split ("\n"[0]);

		//leer el tamaño del mapa
		string[] lines = text [0].Split (";"[0]);
		gridSizeX = Mathf.RoundToInt (int.Parse(lines[0]));
		gridSizeZ = Mathf.RoundToInt (int.Parse(lines[1]));
        if (lines.Length>=4) { 
            recursoDinero= Mathf.RoundToInt(int.Parse(lines[2]));
            recursoDineroInicial = recursoDinero;
            recursoGas = Mathf.RoundToInt(int.Parse(lines[3]));
        }
        if (lines.Length >= 5) {
            tutorialIndexForLater = Mathf.RoundToInt(int.Parse(lines[4]));
            Debug.Log(lines[4]);
        } else {
            tutorialIndexForLater = 0;
        }
        
        grid = new Node[gridSizeX, gridSizeZ];
		gridWorldSize = new Vector2 (gridSizeX*nodeDiameter, gridSizeZ*nodeDiameter);

		//ajustar tamaño del plano/piso
		this.transform.position = new Vector3((float.Parse(lines[0]) - 1)/2,  (float.Parse(lines[1]) - 1)/2, -10);
        plane.transform.localScale = new Vector3(gridSizeX*nodeDiameter, gridSizeZ*nodeDiameter, 1);
        relics = new List<relic>();
        relicounter = 0;
        relicsaved = 0;

        //leer cada linea del archivo
        for (int i=1;i<text.Length;i++) {
			//leer linea y transformarla en un nodo segun coordenadas
			lines = text [i].Split (";"[0]);
			Vector2 gridPoint = new Vector2 (int.Parse(lines[0]),int.Parse(lines[1]));

            grid[int.Parse(lines[0]),int.Parse(lines[1])]=new Node(gridPoint, lines[2], bool.Parse(lines[3]), this) ;
            if (lines[2].Contains("relic")) {
                relic newrelic = Instantiate(relic_prefab, new Vector3(gridPoint.x, gridPoint.y, 0), Quaternion.identity, groupItems);
                relicounter++;
                newrelic.relicID = relicounter;
                relics.Add(newrelic);
            }

            if (lines[2].Contains("worm")) {
                enemy_parent newWorm = Instantiate(enemy_worm_prefab, new Vector3(gridPoint.x, gridPoint.y, 0), Quaternion.identity, groupEnemy);//,this.transform
            }
            if (lines[2].Contains("shark")) {
                enemy_shark newshark = Instantiate(enemy_shark_prefab, new Vector3(gridPoint.x, gridPoint.y, 0), Quaternion.identity, groupEnemy);
            }
            if (lines[2].Contains("thief")) {
                enemy_thief newthief = Instantiate(enemy_thief_prefab, new Vector3(gridPoint.x, gridPoint.y, 0), Quaternion.identity, groupEnemy);
            }
            if (lines[2].Contains("nest")) {
                enemy_thief_nest newNest = Instantiate(enemy_nest_prefab, new Vector3(gridPoint.x, gridPoint.y, 0), Quaternion.identity, groupEnemy);
            }
            if (lines[2].Contains("keymaster")) {
                enemy_unlocker newLock = Instantiate(enemy_unlocker_prefab, new Vector3(gridPoint.x, gridPoint.y, 0), Quaternion.identity, groupEnemy);
            }
            if (lines[2].Contains("sphinxawake")) {
                enemy_sphinx newboss = Instantiate(enemy_sphinx_prefab, new Vector3(gridPoint.x, gridPoint.y, 0), Quaternion.identity, groupEnemy);
            }

            if (lines[2].Contains("START")) {
                unit_HQ newHQ = Instantiate(unit_HQ_prefab, new Vector3(gridPoint.x, gridPoint.y, 0), Quaternion.identity, groupUnit);
                newHQ.name = "unitHQ";
                unitHQ_code = newHQ;
            }
            if (lines[2].Contains("DRILL")) {
                unit_parent startUnit = Instantiate(unit_drill_prefab, new Vector3(gridPoint.x, gridPoint.y, 0), Quaternion.identity, groupUnit);
            }
            if (lines[2].Contains("TANK")) {
                unit_parent startUnit = Instantiate(unit_tank_prefab, new Vector3(gridPoint.x, gridPoint.y, 0), Quaternion.identity, groupUnit);
            }
            if (lines[2].Contains("SCOUT")) {
                unit_parent startUnit = Instantiate(unit_scout_prefab, new Vector3(gridPoint.x, gridPoint.y, 0), Quaternion.identity, groupUnit);
            }
            if (lines[2].Contains("BOMB")) {
                unit_parent startUnit = Instantiate(unit_bomb_prefab, new Vector3(gridPoint.x, gridPoint.y, 0), Quaternion.identity, groupUnit);
            }

        }

		//conectar nodos
		for(int i=0;i<gridSizeX;i++) {
			for(int j=0;j<gridSizeZ;j++) {
				try{grid [i, j].leftNode = grid [i-1, j];}
				catch{}
				try{grid [i, j].rightNode = grid [i+1, j];}
				catch{}
				try{grid [i, j].upNode = grid [i, j+1];}
				catch{}
				try{grid [i, j].downNode = grid [i, j-1];}
				catch{}
				grid [i, j].drawmelikeafrenchgirl ();
			}
		}
        
        checkRelics();//in case of no relics end game
        gamestate = 1;
       //world set
    }

    public void showTutorial() {
        //and finally show a tutorial if it has one
        if (tutorialIndexForLater != 0) {
            switch (tutorialIndexForLater) {
                case 1:
                    selector.actionLocked = true;
                    menuTutorial.gameObject.SetActive(true);
                    menuTutorial.setValues(globals.basicTutorials, selector.closeTutorial);
                    break;
                case 2:
                    selector.actionLocked = true;
                    menuTutorial.gameObject.SetActive(true);
                    menuTutorial.setValues(globals.tankTutorials, selector.closeTutorial);
                    break;
                case 3:
                    selector.actionLocked = true;
                    menuTutorial.gameObject.SetActive(true);
                    menuTutorial.setValues(globals.scoutTutorials, selector.closeTutorial);
                    break;
                case 4:
                    selector.actionLocked = true;
                    menuTutorial.gameObject.SetActive(true);
                    menuTutorial.setValues(globals.bombTutorials, selector.closeTutorial);
                    break;
                case 5:
                    selector.actionLocked = true;
                    menuTutorial.gameObject.SetActive(true);
                    menuTutorial.setValues(globals.moreEnemiesTutorial, selector.closeTutorial);
                    break;
            }
        }
    }


    public Node nodeFromWorldPoint(Vector3 worldpos){
        /*float clampx = Mathf.Clamp(worldpos.x,0,gridWorldSize.x);
		float clampz = Mathf.Clamp(worldpos.z,0,gridWorldSize.y);
		
		int x = Mathf.RoundToInt ( clampx / nodeDiameter);
		int z= Mathf.RoundToInt (clampz / nodeDiameter);
		//Debug.Log ("x:"+x+" z:"+z);
		return  grid[x, z];*/

        int x = Mathf.FloorToInt(worldpos.x);
        int z = Mathf.FloorToInt(worldpos.y);
        //Debug.Log ("x:"+x+" z:"+z);
        return grid[x, z]; 

    }

    public void updateAllNodes() {
        foreach (Node n in grid) {
            //only update the visible nodes
            if (selector.camScript.IsVisibleToCamera(n.nodeSquare.transform,0.1f)) {
                n.Update();
            }
        }
    }

    // Update is called once per frame
    void LateUpdate () {
        if (gamestate == 1) {
            updateAllNodes();
            guimanager.updateBase(turnCount, recursoDinero, recursoGas, relicsaved, relicounter);
            guimanager.updateNode(selector.cursorNode);
            if (!selector.cursorNode.nodeOculto && selector.cursorNode.isThereAUnitHere()) {
                guimanager.updateUnit(selector.cursorNode.unitInThisNode);
            } else {
                if (!selector.cursorNode.nodeOculto && selector.cursorNode.isThereAnEnemyHere()) {
                    guimanager.updateEnemy(selector.cursorNode.enemyInThisNode);
                } else {
                    guimanager.hideUnitPanel();
                }
            }
        }

        if (Input.GetKey("escape")) {
            Application.Quit();
        }

        //DEBUG ONLY
        if (Input.GetKeyDown("2")) {
            foreach (Node n in grid) {
                n.revealNode();
            }
        }
        if (Input.GetKeyDown("3")) {
            gamestate = 3;
            StartCoroutine("endTurn");
        }
        

    }
    

    public void reStart(bool nextStage) {
        foreach (Transform child in groupNode) {
            Destroy(child.gameObject);
        }
        foreach (Transform child in groupEnemy) {
            Destroy(child.gameObject);
        }
        foreach (Transform child in groupUnit) {
            Destroy(child.gameObject);
        }
        foreach (Transform child in groupItems) {
            Destroy(child.gameObject);
        }
        updateActors();
        gamestate = 0;
        if (nextStage) { mapIndex++; }
        if (mapIndex < mapFile.Count) {
            Debug.Log("file "+ mapIndex + "/"+ mapFile.Count);
            initialize(); 
        } else {
            //quitGame();
            SceneManager.LoadScene("titleScreen");
        }
        
    }

    public void surrender() {
        gamestate = 2;
        StartCoroutine("endTurn");
    }

    public void quitGame() {
        Application.Quit();
    }

    public void addDinero(int val) {
        recursoDinero += val;
        /*if (recursoDinero < 0) {
            recursoDinero = 0;
        }*/
    }

    public void addGas(int val) {
        recursoGas += val;
        if (recursoGas < 0) {
            recursoGas = 0;
        }

    }

    public void checkRelics() {
        int relCount = 0;
        foreach (relic rel in relics) {
            if (rel.state == 3) {
                relCount++;
            }
        }

        //if (relCount == relics.Count()) {
        if (relCount == relicounter) {
            //end game
            Debug.Log("you won!");
            gamestate = 3;
            StartCoroutine("endTurn");
        }
    }

    public void wakeUpStatues() {
        int relCount = 0;
        foreach (relic rel in relics) {
            if (rel.state == 3) {
                relCount++;
            }
        }

        if (gamestate == 1 && relCount == relicounter - 1) {
            //wake up boss
            Debug.Log("you gonna need a bigger drill");
            foreach (Node n in grid) {
                if (n.tiletype.Contains("sphinxstatue")) {
                    n.tiletype = n.tiletype.Replace("sphinxstatue", "normal");
                    enemy_sphinx newboss = Instantiate(enemy_sphinx_prefab, new Vector3(n.gridPoint.x, n.gridPoint.y, 0), Quaternion.identity, groupEnemy);
                    n.revealNode();
                }
            }
            updateActors();
        }

    }


    IEnumerator endTurn() {

        if (gamestate == 1) {
            playersturn = false;
            guimanager.enemyTurnText.gameObject.SetActive(true);
            selector.actionLocked = true;
            foreach (GameObject unit in units) {
                unit_parent unitscript = unit.GetComponent<unit_parent>();
                unitscript.alreadyMoved = false;
                if (unitscript.isActive) {
                    //this.addGas(-unitscript.basecost);
                    this.addDinero(-unitscript.basecost);
                }
            }
            if (recursoDinero<=globals.gasCoinCost) {
                gamestate = 2;
                Debug.Log("game over");
                yield return null;
            }

            foreach (GameObject enemy in enemies) {
                enemy_parent enemyscript = enemy.GetComponent<enemy_parent>();
                enemyscript.StartCoroutine("enemysturn");

                while (!enemyscript.enemyDone) {
                    //wait for enemy to finish his turn
                    yield return null;
                }
            }

            /*//allNests
            foreach (GameObject oneNest in allNests) {
                enemy_thief_nest nestScript = oneNest.GetComponent<enemy_thief_nest>();
                nestScript.StartCoroutine("nestTurn");

                while (!nestScript.enemyDone) {
                    //wait for enemy to finish his turn
                    yield return null;
                }
            }*/

            selector.actionLocked = false;
            yield return null;
            playersturn = true;
            guimanager.enemyTurnText.gameObject.SetActive(false);
            turnCount++;
        }

        if (gamestate == 2 || gamestate == 3) {
            yield return null;
            selector.actionLocked = true;
            stageResult.gameObject.SetActive(true);
            stageResult.setValues(gamestate,turnCount,relicsaved,relicounter, recursoDinero, recursoDineroInicial);
        }
    }
}
