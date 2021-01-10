using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class tutorialScreen : MonoBehaviour{

    public Grid gridMaster;
    public selector cursorParent;

    List<tutorial_item> items;
    int i;

    public Button prevB;
    public Image imageBox;
    public Text textBox;
    public Text pageCount;

    public void initialize() {
        //gridMaster = (Grid)GameObject.Find("GameMaster").GetComponent(typeof(Grid));
        cursorParent = gridMaster.selector;
        //items = globals.tutorialItems;
        i = 0;
    }

    // Update is called once per frame
    void Update(){
        if (Input.GetKeyDown("z") || Input.GetKeyDown("x") || Input.GetKeyDown(KeyCode.RightArrow)) {
            nextScreen();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            prevScreen();
        }
        prevB.interactable = (i > 0);
        
    }

    public void setValues(List<tutorial_item> _items) {
        cursorParent.movementLocked = true;
        items = _items;
        i = 0;
        setScreen();
    }

    public void prevScreen() {
        if (0 < i) {
            i--;
            setScreen();
        }
    }

    public void nextScreen() {
        if (i < items.Count-1) {
            i++;
            setScreen();
        } else {
            closeMenu();
        }
    }

    public void setScreen() {
        imageBox.sprite = items[i].Image;
        textBox.text = items[i].Text;
        pageCount.text = i+1 + "/" + items.Count;
    }

    public void closeMenu() {
        cursorParent.closeTutorial();
        this.gameObject.SetActive(false);
    }
}
