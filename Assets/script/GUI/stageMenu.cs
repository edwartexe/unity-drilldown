using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class stageMenu : MonoBehaviour {
    public selector cursorParent;
    private Grid gridMaster;
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
        gridMaster = (Grid)GameObject.Find("GameMaster").GetComponent(typeof(Grid));
        cursorParent = (selector)GameObject.Find("Selector").GetComponent(typeof(selector));
        content = this.transform.Find("Viewport/Content").gameObject;
        selectedOption = 0;
        maxOptions = 0;
        buttonArray = new List<GameObject>();

        makeOptions();
    }

    public void makeOptions() {
        string[] allOptions =
        {
            "Return",
            "End Turn",
            "Full Guide",
            "Restart Stage",
            "Quit Game"
        };

        foreach (Transform child in content.transform) {
            Destroy(child.gameObject);
        }

        highlighter = Instantiate(highlighterPrefab, Vector3.zero, Quaternion.identity, content.transform);
        highlighter.name = "highlighter stage";
        highlighter.SetParent(content.transform);
        highlighter.anchorMin = new Vector2(0, 1);
        highlighter.anchorMax = new Vector2(0, 1);
        highlighter.pivot = new Vector2(0, 0);
        highlighter.sizeDelta = new Vector2(400, 30);
        highlighter.localPosition = new Vector3(5, -33, 0);
        highlighter.localEulerAngles = new Vector3(0, 0, 0);
        highlighter.localScale = new Vector3(1, 1, 1);

        int iteration = 0;
        foreach (string option in allOptions) {
            GameObject gogo = Resources.Load("UIbutton_OPs") as GameObject;
            GameObject button = Instantiate(gogo, Vector3.zero, Quaternion.identity) as GameObject;
            button.GetComponent<Button>().onClick.AddListener(delegate { optionSelected(option); });
            button.gameObject.GetComponentInChildren<Text>().text = option;
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
        if (controls.vertical<0) {
            selectedOption = globals.Mod(selectedOption + 1, maxOptions);
            if (buttonArray.Count > 0 && selectedOption >= 0 && selectedOption <= maxOptions) {
                highlighter.GetComponent<RectTransform>().localPosition = buttonArray[selectedOption].GetComponent<RectTransform>().localPosition;
            }
        }
        if (controls.vertical>0) {
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
            cursorParent.actionLocked = false;
            this.gameObject.SetActive(false);
        }
    }

    public void optionSelected(string optionName) {
        switch (optionName) {
            case "Return":
                cursorParent.actionLocked = false;
                this.gameObject.SetActive(false);
                break;
            case "End Turn":
                gridMaster.StartCoroutine("endTurn");
                this.gameObject.SetActive(false);
                break;
            case "Full Guide":
                //cursorParent.actionLocked = false;
                this.gameObject.SetActive(false);
                gridMaster.menuTutorial.gameObject.SetActive(true);
                gridMaster.menuTutorial.setValues(globals.allTutorials);
                break;
            case "Restart Stage":
                this.gameObject.SetActive(false);
                confirmPanel.Show(
                    "Restart this stage?", 
                    () => {  gridMaster.surrender(); },
                    ()=> { cursorParent.actionLocked = false; }
                );
                break;
            case "Quit Game":
                this.gameObject.SetActive(false);
                confirmPanel.Show(
                    "Quit Game?",
                    () => { gridMaster.quitGame(); },
                    () => { cursorParent.actionLocked = false; }
                );
                break;
            default:
                cursorParent.actionLocked = false;
                this.gameObject.SetActive(false);
                break;
        }
    }

}

