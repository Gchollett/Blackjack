using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDeckAnimation : MonoBehaviour
{
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Move(){
        animator.SetBool("Hovering",true);
    }

    public void MoveBack(){
        animator.SetBool("Hovering",false);
    }
}
