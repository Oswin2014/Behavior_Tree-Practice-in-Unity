using UnityEngine;
using System.Collections;

public class AnimationEventUtil : MonoBehaviour {


    void disableAnimator()
    {
        gameObject.GetComponent<Animator>().enabled = false;
    }

}
