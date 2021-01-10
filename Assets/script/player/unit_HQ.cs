using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class unit_HQ : MonoBehaviour{

    public bool selected = false;
    public bool alreadyMoved = false;
    public Grid gridMaster;
    public selector selector;
    public Node thisNode;

    public List<Node> creationRange;
    // Start is called before the first frame update
    public void initialize(){

        gridMaster = (Grid)GameObject.Find("GameMaster").GetComponent(typeof(Grid));
        selector = (selector)GameObject.Find("Selector").GetComponent(typeof(selector));


        thisNode = gridMaster.nodeFromWorldPoint(this.transform.position);
        thisNode.walkable = false;
        thisNode.tiletype = "HQ";

        creationRange = new List<Node>();
        if (thisNode.upNode != null) { creationRange.Add(thisNode.upNode); }
        if (thisNode.rightNode != null) { creationRange.Add(thisNode.rightNode); }
        if (thisNode.downNode != null) { creationRange.Add(thisNode.downNode); }
        if (thisNode.leftNode != null) { creationRange.Add(thisNode.leftNode); }
    }

    public virtual bool highLighteNodes(Node searcher) {
        return creationRange.Contains(searcher);
    }

 
    public virtual bool checkCreationSpaces(Node targetNode, bool compareNode) {

        foreach (Node nn in creationRange) {
            if (compareNode) {
                if (targetNode.Equals(nn)) {
                    return (nn.canUnitCross() && !nn.isThereAUnitHere());
                }
            } else {
                if (nn.canUnitCross() && !nn.isThereAUnitHere()) {
                    return true;
                }
            }
        }

        return false;
    }
}
