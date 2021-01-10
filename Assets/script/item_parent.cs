using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class item_parent : MonoBehaviour
{

    public bool selected = false;
    public Node lastNode;
    public Node thisNode;
    public Grid gridMaster;
    public selector selector;

    public int state; // 0=hidden, 1=revealed, 2=taken, 3=saved

    // Start is called before the first frame update
    void Start() {

        gridMaster = (Grid)GameObject.Find("GameMaster").GetComponent(typeof(Grid));
        selector = (selector)GameObject.Find("Selector").GetComponent(typeof(selector));
        thisNode = gridMaster.nodeFromWorldPoint(this.transform.position);

    }

    public void updatePosition() {
        this.transform.position = new Vector3(thisNode.gridPoint.x, thisNode.gridPoint.y, 0);
    }

    public virtual void onHide() { }
    public virtual void onReveal() { }
    public virtual void onTake(bool visiblesparks) { }
    public virtual void onSave() { }

    private void OnDestroy() {
        gridMaster.updateActors();
    }

}


