using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetterJumping : MonoBehaviour
{

    private Rigidbody2D rb;
    private PlayerController playerController;
    private PlayerInputActions playerInputActions;
    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerController = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Jump();
    }
    private void Jump()
    {
        //TODO - Add buffered jump
        if (rb.velocity.y<0 && !playerController.grounded)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (playerController.jumpFallMultiplier - 1) * Time.deltaTime; 
        }
        else if(!playerController.grounded && playerController.endedJumpEarly)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (playerController.jumpEndEarlyGravityModifier - 1) * Time.deltaTime; 
        }

    }
    
}
