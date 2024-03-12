using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearComponent : MonoBehaviour
{
    public AnimationClip clearAnimation;

    public float GetAnimationLength()
    {
        return clearAnimation.length;
    }

    public void Clear()
    {
        StartCoroutine(ClearCoroutine());
    }

    private IEnumerator ClearCoroutine()
    {
        Animator animator = GetComponent<Animator>();

        if (animator != null)
        {
            animator.Play(clearAnimation.name);
            yield return new WaitForSeconds(clearAnimation.length);
            Destroy(gameObject);
        }
    }
}
