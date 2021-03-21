using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class mainScreen : MonoBehaviour {
    public tutorialScreen menuTutorial;
    public GameObject stageSelect;
    public RectTransform scrollContent;
    public RectTransform highlighterPrefab;
    RectTransform highlighter;
    List<GameObject> buttonArray;
    public int selectedOption;
    int maxOptions;

    

    // Start is called before the first frame update
    void Start() {


        if (menuTutorial == null) { menuTutorial = (tutorialScreen)GameObject.Find("tutorialScreen").GetComponent(typeof(tutorialScreen)); }
        menuTutorial.initialize();
        menuTutorial.gameObject.SetActive(false);


        if (stageSelect == null) { stageSelect = GameObject.Find("stageSelect"); }
        stageSelect.gameObject.SetActive(false);


        buttonArray = new List<GameObject>();

        highlighter = Instantiate(highlighterPrefab, Vector3.zero, Quaternion.identity, scrollContent.transform);
        highlighter.name = "highlighter stage";
        highlighter.SetParent(scrollContent.transform);
        /*highlighter.anchorMin = new Vector2(0, 1);
        highlighter.anchorMax = new Vector2(0, 1);
        highlighter.pivot = new Vector2(0, 0);
        highlighter.sizeDelta = new Vector2(400, 30);*/
        //highlighter.anchoredPosition = new Vector3(5, -33);
        //highlighter.localEulerAngles = Vector3.zero;
        //highlighter.localScale = Vector3.one;
        int iteration = 0;

        //DirectoryInfo info = new DirectoryInfo(Application.streamingAssetsPath + "/mapfiles");
        //FileInfo[] mapList = info.GetFiles();

        List<TextAsset> mapList = globals.aviableMaps;
        foreach (TextAsset file in mapList) {
            //Debug.Log(file.Name);

            GameObject gogo = Resources.Load("UIbutton_OPs") as GameObject;
            GameObject button = Instantiate(gogo, Vector3.zero, Quaternion.identity) as GameObject;
            button.GetComponent<Button>().onClick.AddListener(delegate { stageOptionClick(file); });
            //button.gameObject.GetComponentInChildren<Text>().text = file.Name;
            button.gameObject.GetComponentInChildren<Text>().text = file.name;
            RectTransform rectTransform = button.GetComponent<RectTransform>();
            rectTransform.SetParent(scrollContent.transform);
            rectTransform.localPosition = new Vector3(5, -33 - (iteration * 30), 0);
            //rectTransform.anchoredPosition = new Vector2(5, -33 - (iteration * 30));
            rectTransform.sizeDelta = new Vector2(-10,30);
            rectTransform.localScale = new Vector3(1, 1, 1);

            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.pivot = new Vector2(0, 0);

            iteration++;
            buttonArray.Add(button);
            maxOptions = iteration;
        }
        scrollContent.sizeDelta = new Vector2(0, maxOptions*30);
    }


    void stageOptionClick(TextAsset optionSelected) {
        globals.mapFiles = new List<TextAsset> { optionSelected };
        startGame();
    }



    // Update is called once per frame
    void Update() {
        controls.calculate();

        if (controls.vertical < 0) {
            Debug.Log("step 1");
            selectedOption = globals.Mod(selectedOption + 1, maxOptions);
            if (buttonArray.Count > 0 && selectedOption >= 0 && selectedOption <= maxOptions) {
                Debug.Log("step 2");
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
            //deactivate stage select panel
        }
    }

    public void openTutorial() {
        menuTutorial.gameObject.SetActive(true);
        menuTutorial.setValues(globals.allTutorials, closeTutorial);
    }

    public void closeTutorial() {
        menuTutorial.gameObject.SetActive(false);
    }

    public void openStageSelect() {
        stageSelect.gameObject.SetActive(true);
        //stageSelect.setValues(globals.allTutorials, closeTutorial);
    }

    public void closeStageSelect() {
        stageSelect.gameObject.SetActive(false);
    }

    public void startGame() {
        SceneManager.LoadScene("mainScene");
    }
}
