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
        animator.SetFloat("velocity", pc.GetCurrentSpeed()); 
        animator.SetBool("jumped", pc.GetJump());
        animator.SetBool("crouched", pc.GetCrouch());
        animator.SetBool("grounded", pc.GetGrounded());
        animator.SetInteger("jumpchain", pc.GetJumpchain());
    }
}
