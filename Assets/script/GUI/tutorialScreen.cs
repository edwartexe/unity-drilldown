using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class tutorialScreen : MonoBehaviour{
    List<tutorial_item> items;
    int i;

    public Button prevB;
    public Image imageBox;
    public Text textBox;
    public Text pageCount;

    public delegate void TestDelegate(); // This defines what type of method you're going to call.
    public TestDelegate methodAtClose; // This is the variable holding the method you're going to call.

    public void initialize() {
        i = 0;
    }

    // This method expects a TestDelegate variable, allowing us to pass a custom void method.
    /*private void SimpleMethod(TestDelegate method) {
        method();
    }*/

    // Update is called once per frame
    void Update(){
        if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2") || Input.GetKeyDown(KeyCode.RightArrow)) {
            nextScreen();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            prevScreen();
        }
        prevB.interactable = (i > 0);
        
    }

    public void setValues(List<tutorial_item> _items, TestDelegate method) {
        //cursorParent.actionLocked = true;

        // Fill Delegate.
        // Call method, pass along delegate. Notice we don't use (); here. (that can be confusing)
        methodAtClose = method;

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
        //SimpleMethod(m_methodToCall);
        methodAtClose();
        this.gameObject.SetActive(false);
    }
}
