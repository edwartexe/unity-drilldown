using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class creatorMenu : MonoBehaviour{ 

    public Grid gridMaster;
    public selector cursorParent;
    public unit_HQ unitHQ_code;
    public buygasMenu menubuygas;

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
        //gridMaster = (Grid)GameObject.Find("GameMaster").GetComponent(typeof(Grid));
        cursorParent = gridMaster.selector;
        unitHQ_code = gridMaster.selector.unitHQ_code;
        menubuygas = gridMaster.selector.menuBuyGas;
        //menubuygas.gameObject.SetActive(false);
    }

    private void OnEnable() {
        selectedOption = 0;
        maxOptions = 0;
        buttonArray = new List<GameObject>();
        content = this.transform.Find("Viewport/Content").gameObject;
        makeOptions();
    }

    public void makeOptions() {
        List<battleOption> allOptions = new List<battleOption>();
        allOptions.Add(new battleOption(0, "New Drill", globals.drill_create_cost));
        allOptions.Add(new battleOption(1, "New Tank", globals.tank_create_cost));
        allOptions.Add(new battleOption(2, "New Scout", globals.scout_create_cost));
        allOptions.Add(new battleOption(4, "New Bomb", globals.bomb_create_cost));
        if (gridMaster.relicsaved>=1) { allOptions.Add(new battleOption(5, "New Armored Scout", globals.tank_create_cost)); }
        if (gridMaster.relicsaved >= 1) { allOptions.Add(new battleOption(6, "New Super Drill", globals.superDrill_create_cost)); }
        //allOptions.Add(new battleOption(3, "Buy Gas", globals.gasCoinCost));
        allOptions.Add(new battleOption(10, "Cancel", 0));

        foreach (Transform child in content.transform) {
            Destroy(child.gameObject);
        }

        highlighter = Instantiate(highlighterPrefab, Vector3.zero, Quaternion.identity, content.transform);
        highlighter.name = "highlighter creator";
        //Image highlighterRenderer = highlighter.AddComponent<Image>();
        //highlighterRenderer.sprite = Resources.Load("OptionHighlight", typeof(Sprite)) as Sprite;
        //RectTransform highlighterTransform = highlighter.GetComponent<RectTransform>();
        //highlighterTransform.SetParent(content.transform);
       /* highlighter.anchorMin = new Vector2(0, 1);
        highlighter.anchorMax = new Vector2(0, 1);
        highlighter.pivot = new Vector2(0, 0);
        highlighter.sizeDelta = new Vector2(400, 30);*/
        highlighter.localPosition = new Vector3(5, -33, 0);
        highlighter.localEulerAngles = new Vector3(0, 0, 0);
        highlighter.localScale = new Vector3(1, 1, 1);


        int iteration = 0;
        foreach (battleOption option in allOptions) {
            GameObject gogo = Resources.Load("UIbutton_OPs") as GameObject;
            //Button bb = (Button) gogo.GetComponent<Button>();

            GameObject button = Instantiate(gogo, Vector3.zero, Quaternion.identity, this.transform) as GameObject;

            //button.GetComponent<Button>().onClick.AddListener(optionSelected );
            button.GetComponent<Button>().onClick.AddListener(delegate { optionSelected(option.code); });
            if (option.cost == 0) {
                button.gameObject.GetComponentInChildren<Text>().text = option.name;
            } else {
                button.gameObject.GetComponentInChildren<Text>().text = option.name + " $ " + option.cost;
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
        content.GetComponent<RectTransform>().sizeDelta = new Vector2(0, maxOptions * 30);

    }



    void Update() {
            if (controls.vertical < 0) {
                selectedOption = globals.Mod(selectedOption + 1, maxOptions);
                if (buttonArray.Count > 0 && selectedOption >= 0 && selectedOption <= maxOptions) {
                    highlighter.GetComponent<RectTransform>().localPosition = buttonArray[selectedOption].GetComponent<RectTransform>().localPosition;
                }
            }
            if (controls.vertical > 0) {
                selectedOption = globals.Mod(selectedOption - 1, maxOptions);
                if (buttonArray.Count > 0 && selectedOption >= 0 && selectedOption <= maxOptions) {
                    highlighter.GetComponent<RectTransform>().localPosition = buttonArray[selectedOption].GetComponent<RectTransform>().localPosition;
                }
            }
            if (Input.GetButtonDown("Fire1")) {
                //selected trigger onclick event
                if (buttonArray.Count > 0 && selectedOption >= 0 && selectedOption <= maxOptions) {
                    buttonArray[selectedOption].GetComponent<Button>().onClick.Invoke();
                }
            }
            if (Input.GetButtonDown("Fire2")) {
                //Destroy(highlighter);
                cursorParent.actionLocked = false;
                cursorParent.creationMenuCancelCreation();
                this.gameObject.SetActive(false);
            }

        
    }

    public void optionSelected(int optionCode) {
        switch (optionCode) {
            case 0: //drill
                if (unitHQ_code.checkCreationSpaces(null,false) && (gridMaster.recursoDinero >= globals.drill_create_cost)  ) {
                    cursorParent.create_selector = 1;
                    cursorParent.actionLocked = false;
                    cursorParent.state_selector = 7;
                    this.gameObject.SetActive(false);
                } else {
                    //play error sound
                }
                break;
            case 1: //tank
                if (unitHQ_code.checkCreationSpaces(null, false) && (gridMaster.recursoDinero >= globals.tank_create_cost)  ) {
                    cursorParent.create_selector = 2;
                    cursorParent.actionLocked = false;
                    cursorParent.state_selector = 7;
                    this.gameObject.SetActive(false);
                } else {
                    //play error sound
                }
                break;
            case 2: //scout
                if (unitHQ_code.checkCreationSpaces(null, false) && (gridMaster.recursoDinero >= globals.scout_create_cost)  ) {
                    cursorParent.create_selector = 3;
                    cursorParent.actionLocked = false;
                    cursorParent.state_selector = 7;
                    this.gameObject.SetActive(false);
                } else {
                    //play error sound
                }
                break;
            case 3: //buyGas
                if (gridMaster.recursoDinero >= globals.gasCoinCost) {
                    menubuygas.gameObject.SetActive(true);
                    this.gameObject.SetActive(false);
                } else { 
                    //play oops
                }
                break;
            case 4: //bomb
                if (unitHQ_code.checkCreationSpaces(null, false) && (gridMaster.recursoDinero >= globals.bomb_create_cost)) {
                    cursorParent.create_selector = 4;
                    cursorParent.actionLocked = false;
                    cursorParent.state_selector = 7;
                    this.gameObject.SetActive(false);
                } else {
                    //play error sound
                }
                break;
            case 5: //armor scout
                if (unitHQ_code.checkCreationSpaces(null, false) && (gridMaster.recursoDinero >= globals.tank_create_cost)) {
                    cursorParent.create_selector = 5;
                    cursorParent.actionLocked = false;
                    cursorParent.state_selector = 7;
                    this.gameObject.SetActive(false);
                } else {
                    //play error sound
                }
                break;
            case 6: //super drill
                if (unitHQ_code.checkCreationSpaces(null, false) && (gridMaster.recursoDinero >= globals.superDrill_create_cost))
                {
                    cursorParent.create_selector = 6;
                    cursorParent.actionLocked = false;
                    cursorParent.state_selector = 7;
                    this.gameObject.SetActive(false);
                }
                else
                {
                    //play error sound
                }
                break;
            case 10: //cancel
                cursorParent.actionLocked = false;
                cursorParent.creationMenuCancelCreation();
                this.gameObject.SetActive(false);
                break;
        }
    }
}
