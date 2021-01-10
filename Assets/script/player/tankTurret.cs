using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tankTurret : MonoBehaviour {
    // Start is called before the first frame update

    public void stopShooting() {
        unit_tank parent = this.GetComponentInParent<unit_tank>();
        parent.stopShooting();

    }
}
