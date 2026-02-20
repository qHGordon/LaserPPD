using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LaserPPD.Core;

public class AnimatorLanguage : MonoBehaviour {

    Animator animator;
    void Awake() {
        animator = GetComponent<Animator>();
    }

    void OnEnable() {
        if (Set.setVal.Language == (int)en_Language.Chinese) {
            animator.SetTrigger("Run_C");
        } else {
            animator.SetTrigger("Run_E");
        }
    }
}
