using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimController : MonoBehaviour
{
    
    Vector2 moveVector2 => GamePlayerController.GameInputActions.PC.Move.ReadValue<Vector2>();
    //TODO:选择角色的时候调用方法把这个改成当前使用的角色的animator
    public Animator currentPlayerAnimator;
    

    private Animator otherPlayerAnimator;
    
    
    private void Start()
    {
        //*
        
    }
    private void LateUpdate()
    {
        if(currentPlayerAnimator==null)
        {
            return;
        }
        if (moveVector2 != Vector2.zero)
        {
            currentPlayerAnimator.SetBool("isMoving", true);
        }
        else currentPlayerAnimator.SetBool("isMoving", false);
    }
}
