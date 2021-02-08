using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class buygasMenu : MonoBehaviour{

    public Grid gridMaster;
    public creatorMenu menuCreator;

    public Button upbutton;
    public Button downbutton;

    public Button cancelbutton;
    public Button okbutton;

    public InputField gasIn;
    public Text coinOut;


    public int buyAmmount;

    // Start is called before the first frame update
    public void initialize() {

        //if (gridMaster == null) { gridMaster = (Grid)GameObject.Find("GameMaster").GetComponent(typeof(Grid)); }
        menuCreator = gridMaster.selector.menuCreator;
        //menuCreator.gameObject.SetActive(false);
    }

    private void OnEnable() {
        buyAmmount = 1;
        gasIn.text = "" + buyAmmount;
        coinOut.text = "" + buyAmmount * globals.gasCoinCost;
    }

    // Update is called once per frame
    void Update(){
        downbutton.interactable = (buyAmmount > 1);
        upbutton.interactable = (buyAmmount < maxammount() );
        okbutton.interactable = (gridMaster.recursoDinero>=globals.gasCoinCost);
        gasIn.interactable = (gridMaster.recursoDinero >= globals.gasCoinCost);
        
        if ((buyAmmount > 1) && controls.vertical < 0) {
            buttonDOWN();
        }

        if ((buyAmmount < maxammount()) && (controls.vertical > 0)) {
            buttonUP();
        }
        if ((gridMaster.recursoDinero >= globals.gasCoinCost)  && Input.GetButtonDown("Fire1")) {
            doPurchase();
        }
        if (Input.GetButtonDown("Fire2")) {
            cancelPurchase();
        }
    }

    int maxammount() {
        return gridMaster.recursoDinero / globals.gasCoinCost;
    }

    public void buttonUP() {
        buyAmmount += 1;
        if (buyAmmount > maxammount() ) { buyAmmount = maxammount(); }
        gasIn.text = ""+buyAmmount;
        coinOut.text = "" + buyAmmount * globals.gasCoinCost;
    }

    public void buttonDOWN() {
        buyAmmount -= 1;
        if (buyAmmount<=1) { buyAmmount = 1; }
        gasIn.text = "" + buyAmmount;
        coinOut.text = "" + buyAmmount * globals.gasCoinCost;
    }

    public void textChange() {
        if (gasIn.text != "") {
            buyAmmount = int.Parse(gasIn.text);
            buyAmmount = Mathf.Clamp(buyAmmount, 1, maxammount());
            gasIn.text = "" + buyAmmount;
            coinOut.text = "" + buyAmmount * globals.gasCoinCost;
        } else {
            buyAmmount = 1;
            gasIn.text = "" + buyAmmount;
            coinOut.text = "" + buyAmmount * globals.gasCoinCost;
        }
    }

    public void cancelPurchase() {
        //menuCreator.gameObject.SetActive(true);
        //menuCreator.menuactive = true;
        gridMaster.selector.actionLocked = false;
        gridMaster.selector.creationMenuCancelCreation();
        this.gameObject.SetActive(false);
    }

    public void doPurchase() {
        gridMaster.addDinero(-buyAmmount * globals.gasCoinCost);
        gridMaster.addGas(buyAmmount);

        gridMaster.selector.actionLocked = false;
        gridMaster.selector.creationMenuCancelCreation();
        this.gameObject.SetActive(false);
    }



}
