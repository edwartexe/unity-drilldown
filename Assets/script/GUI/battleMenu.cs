using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class battleMenu : MonoBehaviour {
    public Grid gridMaster;
    public selector cursorParent;
    //Player_TB playerCode;

    protected ScrollRect scrollRect;
    protected RectTransform contentPanel;
    public int selectedOption;
    int maxOptions;
    private Transform[] children;
    GameObject content;
    public RectTransform highlighterPrefab;
    RectTransform highlighter;
    List<GameObject> buttonArray;


    public void initialize() {
        //if (gridMaster == null) { gridMaster = (Grid)GameObject.Find("GameMaster").GetComponent(typeof(Grid)); }
        cursorParent = gridMaster.selector;
    }

    private void OnEnable() {
        selectedOption = 0;
        maxOptions = 0;
        buttonArray = new List<GameObject>();
        content = this.transform.Find("Viewport/Content").gameObject;
        makeOptions();
    }

    public void getOptions() {
        GameObject content = null;
        GameObject[] gos = (GameObject[])FindObjectsOfType(typeof(GameObject));
        for (int i = 0; i < gos.Length; i++) {
            if (gos[i].name.Contains("Content")) {
                content = gos[i];
                //Debug.Log(content.name + " found");
            }
        }
        if (content != null) {
            children = content.GetComponentsInChildren<Transform>();
            int est = 0;
            foreach (Transform child in children) {
                // Do something to child
                //Debug.Log(child.name + " is child #" + est);
                est++;
            }
        }

    }

    public void makeOptions() {
        List<battleOption> allOptions = new List<battleOption>();

        if (gridMaster.selector.selectedUnit != null) {
            unit_parent currentUnit = gridMaster.selector.selectedUnit;
            if (currentUnit.isActive) {
                if (currentUnit.checkAttackTargets(currentUnit.thisNode, false)) { allOptions.Add(new battleOption(0, "Attack", globals.action_cost_attack)); }
                if (currentUnit.checkDrillTargets(currentUnit.thisNode, false)) { allOptions.Add(new battleOption(1, "Drill", globals.action_cost_drill)); }
                if (currentUnit.checkHoldTargets(currentUnit.thisNode, false)) { allOptions.Add(new battleOption(2, "Take", globals.action_cost_hold)); }
                if (currentUnit.checkScanTargets(currentUnit.thisNode, false)) { allOptions.Add(new battleOption(8, "Scan", globals.action_cost_scan)); }
                if (currentUnit.checkDropTargets(currentUnit.thisNode, false)) { allOptions.Add(new battleOption(6, "Drop", 0)); }
                allOptions.Add(new battleOption(3, "Wait", 0));
                if (currentUnit.checkBombTargets(currentUnit.thisNode, false)) { allOptions.Add(new battleOption(7, "Detonate", 0)); }
                allOptions.Add(new battleOption(5, "Turn Off", 0));
            }
           
            if (!currentUnit.isActive) { allOptions.Add(new battleOption(4, "Turn On", 0)); }
        }

        allOptions.Add(new battleOption(10, "Cancel", 0));


        foreach (Transform child in content.transform) {
            Destroy(child.gameObject);
        }

        highlighter = Instantiate( highlighterPrefab, Vector3.zero, Quaternion.identity, content.transform);
        highlighter.name = "highlighter battle";
        highlighter.anchorMin = new Vector2(0, 1);
        highlighter.anchorMax = new Vector2(0, 1);
        highlighter.pivot = new Vector2(0, 0);
        highlighter.sizeDelta = new Vector2(400, 30);
        highlighter.localPosition = new Vector3(5, -33, 0);
        highlighter.localEulerAngles = new Vector3(0, 0, 0);
        highlighter.localScale = new Vector3(1, 1, 1);

        int iteration = 0;
        foreach (battleOption option in allOptions) {
            GameObject gogo = Resources.Load("UIbutton_OPs") as GameObject;
            //Button bb = (Button) gogo.GetComponent<Button>();

            GameObject button = Instantiate(gogo, Vector3.zero, Quaternion.identity) as GameObject;

            //button.GetComponent<Button>().onClick.AddListener(optionSelected );
            button.GetComponent<Button>().onClick.AddListener(delegate { optionSelected(option.code); });
            if (option.cost==0) {
                button.gameObject.GetComponentInChildren<Text>().text = option.name;
            } else {
                button.gameObject.GetComponentInChildren<Text>().text = option.name + " - " + option.cost +" Gas";
            }
            Transform rectTransform = button.GetComponent<RectTransform>();
            rectTransform.SetParent(content.transform);
            rectTransform.localPosition = new Vector3(5, -33 - (iteration * 30), 0);
            rectTransform.localEulerAngles = new Vector3(0, 0, 0);
            rectTransform.localScale = new Vector3(1, 1, 1);
            iteration++;
            buttonArray.Add(button);
            maxOptions = iteration;
        }

    }



    void Update() {
        if (Input.GetKeyDown("left") || Input.GetKeyDown("a")) {

        }
        if (Input.GetKeyDown("down") || Input.GetKeyDown("s")) {
            selectedOption = globals.Mod(selectedOption + 1, maxOptions);
            if (buttonArray.Count > 0 && selectedOption >= 0 && selectedOption <= maxOptions) {
                highlighter.GetComponent<RectTransform>().localPosition = buttonArray[selectedOption].GetComponent<RectTransform>().localPosition;
            }
        }
        if (Input.GetKeyDown("right") || Input.GetKeyDown("d")) {

        }
        if (Input.GetKeyDown("up") || Input.GetKeyDown("w")) {
            selectedOption = globals.Mod(selectedOption-1, maxOptions); 
            if (buttonArray.Count > 0 && selectedOption >= 0 && selectedOption <= maxOptions) {
                highlighter.GetComponent<RectTransform>().localPosition = buttonArray[selectedOption].GetComponent<RectTransform>().localPosition;
            }
        }
        if (Input.GetKeyDown("z")) {
            //selected trigger onclick event
            if (buttonArray.Count > 0 && selectedOption >= 0 && selectedOption <= maxOptions) {
                buttonArray[selectedOption].GetComponent<Button>().onClick.Invoke();
            }
        }
        if (Input.GetKeyDown("x")) {
            //Destroy(highlighter);
            cursorParent.movementLocked = false;
            cursorParent.battleMenuCancelMovement();
            this.gameObject.SetActive(false);
        }
    }

    public void optionSelected(int optionCode) {
        switch (optionCode) {
            case 0: //attack
                if (gridMaster.recursoGas>=globals.action_cost_attack) {
                    cursorParent.movementLocked = false;
                    cursorParent.battleMenuAttack();
                    this.gameObject.SetActive(false);
                    gridMaster.updateActors();
                } else {
                    //play error sound
                }
                break;
            case 1: //drill
                if (gridMaster.recursoGas >= globals.action_cost_drill) {
                    cursorParent.movementLocked = false;
                    cursorParent.battleMenuDrill();
                    this.gameObject.SetActive(false);
                    gridMaster.updateActors();
                } else {
                    //play error sound
                }
                break;
            case 2: //hold
                if (gridMaster.recursoGas >= globals.action_cost_hold) {
                    cursorParent.movementLocked = false;
                    cursorParent.battleMenuHold();
                    this.gameObject.SetActive(false);
                    gridMaster.updateActors();
                    //do the thing
                } else {
                    //play error sound
                }
                break;
            case 3: //wait
                cursorParent.movementLocked = false;
                cursorParent.battleMenuWait();
                this.gameObject.SetActive(false);
                gridMaster.updateActors();
                break;
                
            case 4: //turn on
                cursorParent.movementLocked = false;
                cursorParent.turnOnUnit();
                this.gameObject.SetActive(false);
                gridMaster.updateActors();
                break;
            case 5: //turn off
                cursorParent.movementLocked = false;
                cursorParent.turnOffUnit();
                this.gameObject.SetActive(false);
                gridMaster.updateActors();
                break;
            case 6: //drop
                cursorParent.movementLocked = false;
                cursorParent.battleMenuDrop();
                this.gameObject.SetActive(false);
                gridMaster.updateActors();
                break;
            case 7: //detonate
                cursorParent.movementLocked = false;
                cursorParent.battleMenuDetonate();
                this.gameObject.SetActive(false);
                gridMaster.updateActors();
                break;
            case 8: //scan
                if (gridMaster.recursoGas >= globals.action_cost_scan) {
                    cursorParent.movementLocked = false;
                    cursorParent.battleMenuScan();
                    this.gameObject.SetActive(false);
                    gridMaster.updateActors();
                } else {
                    //play error sound
                }
                break;
            case 10: //cancel
                cursorParent.movementLocked = false;
                cursorParent.battleMenuCancelMovement();
                this.gameObject.SetActive(false);
                break;
        }
    }

}

