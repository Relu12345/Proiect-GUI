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
        StartCoroutine(WaitASecMan());
        animator.SetBool("Opened", false);
    }

    private IEnumerator WaitASecMan()
    {
        float time = 0f;

        while (time < 5f)
        {
            time += Time.deltaTime;
            yield return null;
        }
    }
}
