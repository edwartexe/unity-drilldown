using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class confirmPanel : MonoBehaviour{

    public static void Show(string dialogMessage, System.Action actionOnConfirm, System.Action actionOnCancel) {
        instance.storedActionOnConfirm = actionOnConfirm; 
        instance.storedActionOnCancel = actionOnCancel;
        instance.dialogText.text = dialogMessage;
        instance.gameObject.SetActive(true);
    }
    private System.Action storedActionOnConfirm;
    private System.Action storedActionOnCancel;
    private static confirmPanel instance;
    public Text dialogText;

    void Awake() {
        instance = this;
        gameObject.SetActive(false);
    }

    void Update() {
        if (Input.GetButtonDown("Fire1")) {
            OnConfirmButton();
        }
        if (Input.GetButtonDown("Fire2")) {
            OnCancelButton();
        }
    }

    public void OnConfirmButton() {
        if (storedActionOnConfirm != null) {
            storedActionOnConfirm();
            storedActionOnConfirm = null;
            storedActionOnCancel = null;
            gameObject.SetActive(false);
        }
    }
    public void OnCancelButton() {
        if (storedActionOnCancel != null) {
            storedActionOnCancel();
            storedActionOnConfirm = null;
            storedActionOnCancel = null;
            gameObject.SetActive(false);
        }
    }
}
