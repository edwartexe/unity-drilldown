using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class menuStageResult : MonoBehaviour{
    public Grid gridMaster;
    public Text displayResult;
    public Text displayTurns;
    public Text displayRelics;
    public Text displayMoney;

    int gamestate;
    public Button Nextbutton;

    public void initialize() {
        //gridMaster = (Grid)GameObject.Find("GameMaster").GetComponent(typeof(Grid));
    }

    void Update() {
        if (Input.GetKeyDown("z")) {
            if (gamestate == 3) {
                gridMaster.reStart(true);
            } else {
                gridMaster.reStart(false);
            }
        }
        if (Input.GetKeyDown("x")) {
            gridMaster.reStart(false);
        }
    }

    public void setValues(int _gamestate,int _turns, int _relicsaved, int _relicounter, int _money, int _startmoney) {
        int gains = _money - _startmoney;
        this.gamestate = _gamestate;
        if (_gamestate == 2) { 
            displayResult.text = "Defeat...";
            Nextbutton.enabled = false;
        }
        if (_gamestate == 3) { displayResult.text = "VICTORY"; }
        displayTurns.text = "Turns: " + _turns;
        displayRelics.text = "Relics Found: " + _relicsaved + " / "+_relicounter;
        displayMoney.text = "Expedition Balance: $" + gains;
    }

    /*public void closeMenu() {
        this.gameObject.SetActive(false);
    }*/

    public void buttonRetry() {
        gridMaster.reStart(false);
    }

    public void buttonNextStage() {
        gridMaster.reStart(true);
    }
}
