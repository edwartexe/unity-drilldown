using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUImanager : MonoBehaviour{

    public Image imageNode;
    public Text displayPos;
    public Text displayType;

    public Text displayTurn;
    public Text displayCoin;
    public Text displayGas;
    public Text displayRelic;

    public GameObject unitpanel;
    public Image imageUnit;
    public Text displayClass;
    public Text displayHealth;
    public Text displayRange;
    public Text displayExtra;

    private Sprite[] allsprites;


    public Text enemyTurnText;

    private void Start() {
        allsprites = Resources.LoadAll<Sprite>("mine_sprites_all");
        enemyTurnText.gameObject.SetActive(false);
    }

    public void updateNode(Node _node) {
        imageNode.sprite = _node.nodeTile.GetComponent<SpriteRenderer>().sprite;
        displayPos.text = "Pos: ["+_node.gridPoint.x +" , "+_node.gridPoint.y+"]";
        displayType.text = _node.shownTileType;

    }

    public void updateBase(int _turn, int _coin, int _gas, int _relic, int _maxrelic) {
        displayTurn.text = "Turn "+ _turn;
        if (displayGas!=null) { displayGas.text = "Gas " + _gas; }
        displayRelic.text = "Relics:"+ _relic +" / "+_maxrelic;
        if (_coin <= globals.coinRedAlert) {
            displayCoin.color = Color.red;
            displayCoin.fontStyle = FontStyle.Bold;
            displayCoin.fontSize = 16;
            displayCoin.text = "$ " + _coin+ " !!";
        } else {
            displayCoin.color = Color.black;
            displayCoin.fontStyle = FontStyle.Normal;
            displayCoin.fontSize = 14;
            displayCoin.text = "$ " + _coin;
        }
    }

    public void updateUnit(unit_parent _unit) {
        unitpanel.SetActive(true);
        displayClass.text = "" + _unit.unitClass;
        displayHealth.text = _unit.health +" / "+ _unit.maxHealth;
        displayRange.text = "Range: "+ _unit.maxWalkDistance;
        displayExtra.text = "Cost: " + _unit.basecost +" Atk: "+_unit.attackPower;
        switch (_unit.unitClass) {
            case globals.drillname:
                imageUnit.sprite = allsprites[8];
                break;
            case globals.tankname:
                imageUnit.sprite = allsprites[9];
                break;
            case globals.scoutname:
                imageUnit.sprite = allsprites[10];
                break;
            case globals.bombname:
                imageUnit.sprite = allsprites[3];
                break;
        }
    }

    public void updateEnemy(enemy_parent enemy) {
        unitpanel.SetActive(true);
        displayClass.text = "" + enemy.enemyClass;
        displayHealth.text = enemy.health + " / " + enemy.maxHealth;
        displayRange.text = "Range: " + enemy.maxWalkDistance;
        displayExtra.text = "Atk: " + enemy.UnitPower;
        switch (enemy.enemyClass) {
            case globals.wormname :
                imageUnit.sprite = allsprites[6];
                break;
            case globals.sharkname:
                imageUnit.sprite = allsprites[4];
                break;
            case globals.thiefname:
                imageUnit.sprite = allsprites[0];
                break;
            case globals.thief2name:
                imageUnit.sprite = allsprites[2];
                break;
            case globals.bossname:
                imageUnit.sprite = allsprites[11];
                break;
            case globals.nestName:
                imageUnit.sprite = allsprites[1];
                break;
            case globals.unlockerName:
                imageUnit.sprite = allsprites[5];
                break;
        }
    }

    public void hideUnitPanel() {
        unitpanel.SetActive(false);
    }
}
