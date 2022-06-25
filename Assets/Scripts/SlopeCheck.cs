using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class SlopeCheck : MonoBehaviour
{
    [SerializeField]
    private LayerMask groundLayerMask;

    [SerializeField]
    private LayerMask slopeLayerMask;

    [HideInInspector]
    public Transform rayCastOrigin;
    
    
    public bool onSlope = false;
    public bool atWall = false;
    public bool onGround = false;

    private void Start()
    {
        rayCastOrigin = transform;
    }

    private void Update()
    {
        CheckSlope();
        CheckWall();   
    }

    private void CheckSlope(){

        if (Physics2D.Raycast(rayCastOrigin.position, Vector2.down, .8f, slopeLayerMask))
            {
            //playerBody2D.gravityScale = 4f;
                //characterController.m_MovementSmoothing = .15f;
                onSlope = true;
            }
            else
            {
                onSlope = false;
                //characterController.m_MovementSmoothing = .3f;
            }
    }

    private void CheckWall()
    {
        if (Physics2D.Raycast(rayCastOrigin.position, Vector2.down, .5f, groundLayerMask))
        {
            onGround = true;
        }
        else
        {
            onGround = false;
        }

        if (Physics2D.Raycast(rayCastOrigin.position, Vector2.left, .5f, groundLayerMask) || Physics2D.Raycast(rayCastOrigin.position, Vector2.right, .5f, groundLayerMask))
        {
            atWall = true;
            
        }
        else
        {
            atWall = false;
        }
    }
}
