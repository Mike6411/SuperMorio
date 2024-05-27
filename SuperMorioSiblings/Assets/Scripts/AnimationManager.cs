using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    private PlayerController pc;
    private Animator animator;
    private void Awake() 
    { 
        pc = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
    }

    private void Update() 
    { 
        //sends to the animator all the data gathered from the pc constantly (ideally bools would get set separately solely when they change status)
        animator.SetFloat("velocity", pc.GetCurrentSpeed());
        animator.SetFloat("vertvelocity", pc.GetCurrentYSpeed());
        animator.SetBool("jumped", pc.GetJump());
        animator.SetBool("crouched", pc.GetCrouch());
        animator.SetBool("grounded", pc.GetGrounded());
        animator.SetInteger("jumpchain", pc.GetJumpchain());
    }
}
