using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera : MonoBehaviour{
    public float cameraDistOffset;
    private Node lastNode;
    private Grid gridMaster;
    public bool toggleGlobal = true;
    public GameObject focusedObject;

    /**/
    public float mapX = 10.0f;
    public float mapY = 10.0f;
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    public bool follow;
    

    // Use this for initialization
    void Start() {
        if (gridMaster == null) { gridMaster = (Grid)GameObject.Find("GameMaster").GetComponent(typeof(Grid)); }
        this.alignCamera(gridMaster.grid[gridMaster.gridSizeX/2, gridMaster.gridSizeX / 2],false);

        /**/
        float vertExtent = this.gameObject.GetComponent<Camera>().orthographicSize;
        float horzExtent = vertExtent * Screen.width / Screen.height;
        // Calculations assume map is position at the origin
        /*minX = horzExtent - mapX / 2.0f;
        maxX = mapX / 2.0f - horzExtent;
        minY = vertExtent - mapY / 2.0f;
        maxY = mapY / 2.0f - vertExtent;*/
        follow = true;
    }

    public bool IsVisibleToCamera(Transform transform, float radius) {
        Vector3 visTest = this.gameObject.GetComponent<Camera>().WorldToViewportPoint(transform.position);
        bool res = (visTest.x+radius >= 0 && visTest.y+ radius >= 0) && (visTest.x- radius <= 1 && visTest.y- radius <= 1);
        //Debug.Log("Transform="+transform +" test="+visTest + " IsVisibleToCamera= " + res);
        return res;
    }


    // Update is called once per frame
    void Update() {
    }

    /**/
    void LateUpdate() {
        bool thereWasChange = false;
        Vector3 v3 = Vector3.zero;

        if (follow) {
            if (focusedObject != null) {
                v3 = focusedObject.transform.position;
                thereWasChange = true;
            } else {
                if (gridMaster.playersturn) {
                    v3 = gridMaster.selector.transform.position;
                    thereWasChange = true;
                }
            }

            if (thereWasChange) {
                v3.x = Mathf.Clamp(v3.x, minX, maxX);
                v3.y = Mathf.Clamp(v3.y, minY, maxY);
                v3.z = -10.0f;
                this.transform.position = v3;
            }
        }

        if (Input.GetKeyDown("1")) { //DEBUG ONLY
            follow = !follow;
        }
    }

    public void alignCamera(Node focusnode, bool smooth) {
        lastNode = focusnode;
        Vector3 moveTo = new Vector3(focusnode.gridPoint.x, focusnode.gridPoint.y, -10);
        
        if (smooth) {
            this.StartCoroutine(MoveRotateObject(this.transform, this.transform.position, moveTo, 0.8f));
        } else {
            this.transform.position = moveTo;
        }
    }


    //smooth movement
    IEnumerator MoveRotateObject(Transform thisTransform, Vector3 startPos, Vector3 endPos, float time) {
        float i = 0.0f;
        float rate = 1.0f / time;
        while (i < 1.0f) {
            i += Time.deltaTime * rate;
            thisTransform.position = Vector3.Lerp(startPos, endPos, i);
            yield return null;
        }
    }
}
