using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {

    Player self;

	void Start () {
        self = this.gameObject.GetComponentInParent<Player>();
	}
	
    void OnTriggerEnter(Collider other)
    {
        Player target = other.transform.GetComponent<Player>();
        if (target == self)
            return;

        if (null != target)
        {
            target.hited();
            if(null != self)
                self.damage();
        }

    }

}
