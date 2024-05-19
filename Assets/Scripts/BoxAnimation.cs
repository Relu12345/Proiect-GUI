using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxAnimation : MonoBehaviour
{
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayAnim()
    {
        animator.SetBool("Opened", true);
        animator.SetBool("Opened", false);
    }
}
