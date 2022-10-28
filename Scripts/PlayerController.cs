using System.Collections;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(BetterJumping),typeof(Rigidbody2D),typeof(CapsuleCollider2D))]
public class PlayerController : MonoBehaviour
{
    
    private Vector2 moveDirection;

    private Bounds characterBounds;
    [HideInInspector] public int facingDirection = 1;
    
    [SerializeField] private LayerMask groundLayer;
    private Animator animator;
    
    /*-----Ground Detection-----*/
    [Header("Ground Detection")] 
    [SerializeField] private Transform detectionPoint;
    [SerializeField] private float boxHeight;
    [SerializeField] private float boxWidth;
    
    /*-----Movement-----*/
    [Header("Movement")]
    [SerializeField] private float speed = 10f;
    
    private Vector3 rawMovement;
    private Vector3 movement;
    private Rigidbody2D rb;
    
    /*-----Jump-----*/
    [Header("Jump")]
    [SerializeField] private float jumpHeight = 15f;
    public float jumpFallMultiplier = 4f;
    public float jumpEndEarlyGravityModifier = 7f;

    private bool canJump; //TODO - Make it PUBLIC
    [HideInInspector] public bool endedJumpEarly = true;
    private float lastJumpPressed;
    private bool jumpingThisFrame;
    
    [HideInInspector] public bool grounded;
    [HideInInspector] public bool canMove = true;

    private Parry parryScript;
    private bool canDash => grounded; //TODO - Make it accessible for other scripts and animations
    private bool blockDash;
    
    private float currentHorizontalSpeed, currentVerticalSpeed;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        InputManager.instance.jump.performed += Jump;
        InputManager.instance.jump.canceled += CancelJump;
        InputManager.instance.dash.performed += Dash;
        InputManager.instance.move.performed += MoveAnimation;
        InputManager.instance.move.canceled += StopMoveAnimation;
        animator = GetComponent<Animator>();
        parryScript = GetComponent<Parry>();
    }
    
    void Update()
    {
        //TODO - Move Everything in a StateMachine
        
        
        InputProcess();
        GroundCheck();
        
        WalkProcess();
    }

    private void InputProcess()
    {
        moveDirection = InputManager.instance.move.ReadValue<Vector2>();
        if (moveDirection.x > 0 && canMove)
        {
            facingDirection = 1;
            transform.rotation = new Quaternion(0, 0, 0, 0);
        }
        else if (moveDirection.x < 0 && canMove)
        {
            facingDirection = -1;
            transform.rotation = new Quaternion(0, 180, 0, 0);
        }
    }
    
    void FixedUpdate()
    {
        if (canMove)
            Move();
    }

    private void WalkProcess()
    {
        currentHorizontalSpeed = moveDirection.x;
    }

    private void MoveAnimation(InputAction.CallbackContext context)
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            return;
        
        animator.SetTrigger("Move");
    }

    private void StopMoveAnimation(InputAction.CallbackContext context)
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Run"))
            return;
        
        animator.SetTrigger("StopMove");
    }

    private void GroundCheck()
    {
        grounded = Physics2D.OverlapBox(new Vector2(detectionPoint.position.x, detectionPoint.position.y),new Vector2(boxWidth,boxHeight),0,groundLayer);
    }

    private void Move()
    {
        rb.velocity = new Vector2(currentHorizontalSpeed * speed, rb.velocity.y);
    }

    

    private void Jump(InputAction.CallbackContext context)
    {
        if(!grounded) 
            return; //EXIT : Character is not grounded
        
        endedJumpEarly = false;
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += Vector2.up * jumpHeight;
        
    }

    private void CancelJump(InputAction.CallbackContext context)
    {
        if (!endedJumpEarly && rb.velocity.y > 0)
            endedJumpEarly = true;
    }
    
    
    private void Dash(InputAction.CallbackContext context)
    {
        if (!canDash)
            return;

        if (parryScript.isBlocking)
            return;

        if (blockDash)
            return;

        InputManager.instance.dashBlockAttack = true;
        rb.velocity = Vector2.zero;
        rb.velocity += new Vector2(facingDirection , rb.velocity.y). normalized * 30;
        blockDash = true;
        animator.SetTrigger("Dash");
        StartCoroutine(DashWait());
    }

    public void UnblockAttack()
    {
        InputManager.instance.dashBlockAttack = false;
    }
    
    void RigidbodyDrag(float x)
    {
        rb.drag = x;
    }
    
    IEnumerator DashWait()
    {
        DOVirtual.Float(7, 0, 2f, RigidbodyDrag);
        
        rb.gravityScale = 0;
        GetComponent<Collider2D>().enabled = false;
        GetComponent<BetterJumping>().enabled = false;
        canMove = false;

        yield return new WaitForSeconds(.3f);

        GetComponent<Collider2D>().enabled = true;
        rb.gravityScale = 3;
        GetComponent<BetterJumping>().enabled = true;
        canMove = true;

        yield return new WaitForSeconds(0.8f);
        blockDash = false;
    }
    
    /// <summary>
    /// Disable Player Movements
    /// </summary>
    public void DisableMovement() 
    {
        if (!grounded)
            return;
        
        canMove = false;
        rb.velocity = new Vector2(0, rb.velocity.y);
        
    }

    public void EnableMovement()
    {
        if (canMove)
            return;

        canMove = true;
    }
}