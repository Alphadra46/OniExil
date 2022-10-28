using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

public class BossAI : MonoBehaviour
{

    [HideInInspector] public int health;
    [SerializeField] private GameObject healthBar;
    [SerializeField] private int maxHealth;
    
    [SerializeField] private VisualEffect playerHitVFX;
    private List<VisualEffect> reactorVFXList = new List<VisualEffect>();
    private VisualEffect attackSignVFX;

    
    private Animator animator;

    [SerializeField] private List<AudioClip> bossHitWooshes;
    private AudioSource audioSource;
    
    [SerializeField] private float roarIntensity;
    [SerializeField] private float roarDuration;

    [SerializeField] private float detectionRadius;
    [SerializeField] private LayerMask playerlayer;
    //private Transform playerPos;
    private bool isPlayerInRange;
    private bool stopSearchingPlayer = false;
    [SerializeField] private Transform player;
    [SerializeField] private float bossSpeed;
    [SerializeField] private float chargeSpeed;

    [SerializeField] private float shakeIntensity;
    [SerializeField] private float shakeTime;
    
    private bool isInCooldown;

    private void Awake()
    {
        for (int i = 0; i < transform.GetChild(0).childCount; i++)
        {
            reactorVFXList.Add(transform.GetChild(0).GetChild(i).GetComponent<VisualEffect>());
        }

        health = maxHealth;
        attackSignVFX = transform.GetChild(1).GetComponent<VisualEffect>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }
    
    
    public enum BossState
    {
        Idle,
        Pattern1,
        Pattern2,
        Walking,
        Dead
    }

    public BossState bossState;
    
    
    void Start()
    {
        
    }

    void Update()
    {
        SearchForPlayerInRange();
        switch (bossState)
        {
            case BossState.Idle:
                UpdateIdle();
                break;
            case BossState.Pattern1:
                UpdatePattern1();
                break;
            case BossState.Pattern2:
                UpdatePattern2();
                break;
            case BossState.Walking:
                UpdateWalking();
                break;
            case BossState.Dead:
                UpdateDeath();
                break;
            default:
                break;
        }   
    }

    private void ChangeState(BossState newState)
    {
        ExitState(bossState);
        OpenState(newState);
        bossState = newState;
    }

    private void ExitState(BossState oldState)
    {
        switch (oldState)
        {
            case BossState.Idle:
                ExitIdleState();
                break;
            case BossState.Pattern1:
                ExitPatternOneState();
                break;
            case BossState.Pattern2:
                ExitPatternTwoState();
                break;
            case BossState.Walking:
                ExitWalkingState();
                break;
            default:
                break;
        }
    }
    
    private void OpenState(BossState newState)
    {
        switch (newState)
        {
            case BossState.Idle:
                OpenIdleState();
                break;
            case BossState.Pattern1:
                OpenPatternOneState();
                break;
            case BossState.Pattern2:
                OpenPatternTwoState();
                break;
            case BossState.Walking:
                OpenWalkingState();
                break;
            case BossState.Dead:
                OpenDeadState();
                break;
            default:
                break;
        }
    }

    #region OpenState

    private void OpenIdleState()
    {
        //stopSearchingPlayer = false;
        //Idle only between attack state and at the very start of the fight
    }

    private void OpenPatternOneState()
    {
        //stopSearchingPlayer = true;
        animator.SetTrigger("FirstPatternAttackOne");
        LookTowardPlayer();
        //Stop Player searching
    }

    private void OpenPatternTwoState()
    {
        //stopSearchingPlayer = true;
        animator.SetTrigger("SecondPatternAttackOne");
        LookTowardPlayer();
        //Stop Player searching
    }

    private void OpenWalkingState()
    {
        //stopSearchingPlayer = false;
        LookTowardPlayer();
        animator.SetTrigger("Walk");
        //Walk toward th player if too far away from it
    }
    
    private void OpenDeadState()
    {
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().simulated = false;
        stopSearchingPlayer = true;
        animator.SetTrigger("Death");
        //Death Animation + Sound and FX
    }

    #endregion

    #region ExitState

    private void ExitIdleState()
    {
        
    }

    private void ExitPatternOneState()
    {
        isInCooldown = true;
        StartCoroutine(AttackCooldown());
        //stopSearchingPlayer = false;
        //Re-Enable player searching
    }

    private void ExitPatternTwoState()
    {
        isInCooldown = true;
        StartCoroutine(AttackCooldown());
        //stopSearchingPlayer = false;
        //Re-Enable player searching
    }

    private void ExitWalkingState()
    {
        
    }
    
    #endregion

    #region UpdateState

    private void UpdateIdle()
    {
        //SearchForPlayerInRange();
        if (isInCooldown)
            return;

        if (isPlayerInRange)
        {
            ChangeState(RandomAttackPattern());   
        }
        else
        {
            ChangeState(BossState.Walking);
        }
    }

    private void UpdatePattern1()
    {
        Debug.Log("Pattern1");
    }

    private void UpdatePattern2()
    {
        Debug.Log("Pattern2");
    }

    private void UpdateWalking()
    {
        //SearchForPlayerInRange();
        WalkTowardsPlayer(); //TODO - Move to FixedUpdate()
        Debug.Log("Walking");

    }

    private void UpdateDeath()
    {
        
    }

    #endregion
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag=="PlayerHit")
        {
            playerHitVFX.Play();
            TakeDamage(col);
            StartCoroutine(ChangeColorAfterHit());
        }
    }

    IEnumerator ChangeColorAfterHit()
    {
        GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.1f);
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    public void PlayRandomAttackSound()
    {
        audioSource.clip = bossHitWooshes[Random.Range(0, bossHitWooshes.Count)];
        audioSource.Play();
    }
    
    private void TakeDamage(Collider2D col)
    {
        var damageToTake = 0f;
        switch (col.name)
        {
            case "HitBoxAttack1":
                damageToTake = 4;
                break;
            case "HitBoxAttack2":
                damageToTake = 5;
                break;
            case "HitBoxAttack3":
                damageToTake = 8;
                break;
            default:
                break;
        }

        if (player.GetComponent<Parry>().isInBurstMode)
        {
            damageToTake *= 1.5f;
        }
        
        health -= Mathf.RoundToInt(damageToTake);
        healthBar.GetComponent<RectTransform>().sizeDelta -= new Vector2(damageToTake*256/maxHealth,0);
        if (health <=0)
        {
            ChangeState(BossState.Dead);
        }
    }

    public void LookTowardPlayer()
    {
        if (player.position.x - transform.position.x <=0)
        {
            transform.rotation = new Quaternion(0, 0, 0, 0);
        }
        else
        {
            transform.rotation = new Quaternion(0, 180, 0, 0);
        }
    }
    
    public void EnableCharge()
    {
        transform.GetComponent<Rigidbody2D>().velocity = new Vector2((player.position - transform.position).normalized.x * chargeSpeed,transform.GetComponent<Rigidbody2D>().velocity.y);
    }

    public void DisableCharge()
    {
        transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, transform.GetComponent<Rigidbody2D>().velocity.y);
    }
    
    public void ActivateBossReactors()
    {
        foreach (var reactorVFX in reactorVFXList)
        {
            reactorVFX.Play();
        }
    }
    
    public void DeActivateBossReactors()
    {
        foreach (var reactorVFX in reactorVFXList)
        {
            reactorVFX.Stop();
        }
    }

    public void AttackSign()
    {
        attackSignVFX.Play();
    }

    private void SearchForPlayerInRange()
    {
        if (stopSearchingPlayer)
            return;
        
        isPlayerInRange = Physics2D.OverlapCircle(transform.position, detectionRadius, playerlayer);
        
    }

    private void WalkTowardsPlayer()
    {
        if (isPlayerInRange)
        {
            ChangeState(RandomAttackPattern());
            return;
        }
        
        transform.GetComponent<Rigidbody2D>().velocity = new Vector2((player.position - transform.position).normalized.x * bossSpeed,transform.GetComponent<Rigidbody2D>().velocity.y);
    }


    private BossState RandomAttackPattern()
    {
        BossState nextAttackPattern = BossState.Pattern1;
        switch (Random.Range(0,2))
        {
            case 0:
                nextAttackPattern = BossState.Pattern1;
                break;
            case 1:
                nextAttackPattern = BossState.Pattern2;
                break;
            default:
                break;
        }

        return nextAttackPattern;
    }

    public void GoBackToIdle()
    {
        ChangeState(BossState.Idle);
    }

    public void AttackCameraShake()
    {
        CameraShake.instance.ShakeCamera(shakeTime, shakeIntensity);
    }
    
    IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(2.5f);
        isInCooldown = false;
    }
    
}
