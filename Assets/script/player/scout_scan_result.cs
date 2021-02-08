using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scout_scan_result : MonoBehaviour {

    public Grid gridMaster;
    public selector cursorParent;
    public unit_parent caster;

    int enemyCount, enemyStrongCount, nestCount, relicCount;

    public InputField enemyN;
    public InputField enemyS;
    public InputField Nest;
    public InputField Treasure;

    public void initialize() {
        //gridMaster = (Grid)GameObject.Find("GameMaster").GetComponent(typeof(Grid));
        cursorParent = gridMaster.selector;
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2")) {
            closeMenu();
        }
    }

    public void setValues(unit_parent _caster, int _enemyCount, int _enemyStrongCount, int _nestCount, int _relicCount) {
        caster = _caster;

        enemyCount = _enemyCount;
        enemyStrongCount = _enemyStrongCount;
        nestCount  = _nestCount;
        relicCount = _relicCount;

        enemyN.text = ""+ enemyCount;
        enemyS.text = "" + enemyStrongCount;
        Nest.text = "" + nestCount;
        Treasure.text = "" + relicCount;
    }

    public void closeMenu() {
        cursorParent.closeScanMenu();
        this.gameObject.SetActive(false);
    }
}
