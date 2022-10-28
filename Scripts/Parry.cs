using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

public class Parry : MonoBehaviour
{
    [HideInInspector] public int health;
    [SerializeField] private int maxHealth;
    [SerializeField] private GameObject healthBar;
    
    [HideInInspector] public bool isBlocking;

    [SerializeField] private CinemachineVirtualCamera cmVcam;
    [SerializeField] private float replaceDuration;
    private float elapsedZoomTime;
    private bool isParrying = false;
    
    [SerializeField] private float perfectParryWindowDuration;
    [SerializeField] private RectTransform burstGauge;
    [SerializeField] private VisualEffect perfectParryVFX;
    [SerializeField] private GameObject perfectParryLight;
    [SerializeField] private float lowARumble,highARumble, rumbleDuration;
    private RumbleGamepad rumbleScript;

    [SerializeField] private VisualEffect burstVFX;
    
    [SerializeField] private List<AudioClip> blockSoundList;
    [SerializeField] private AudioClip perfectParrySound;
    private AudioClip blockSoundToPlay;
    private AudioSource audioSource;

    private float burstStacks;
    private bool burstCast;
    public bool isInBurstMode = false;
    private float burstCastTimer;
    
    private float blockThisFrame;
    private Animator animator;
    private PlayerController playerController;

    [SerializeField] private float burstDurationOneStack;
    private float burstDuration; //Total burst duration which is equal to burstDurationOneStack * burstStacks
    private float elapsedBurstTime;

    private float burstStackAtBurstMode;
    private float burstGaugeXPos;
    void Awake()
    {
        InputManager.instance.block.started += Block; //TODO - Maybe needed to change to performed because of input buffer
        InputManager.instance.block.canceled += CancelBlock;

        InputManager.instance.burst.performed += TryBurstMode;
        InputManager.instance.burst.canceled += CancelCastBurstMode;
        
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
        rumbleScript = GetComponent<RumbleGamepad>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        CastBurstMode();
        CameraZoomParry();
        DecreaseBurstGauge();
    }

    private void Block(InputAction.CallbackContext context)
    {
        if (!InputManager.instance.canBlock)
            return;

        blockThisFrame = Time.time; //Used for perfect parry timing
        animator.SetTrigger("BlockPose");
        //isBlocking = true;
        Debug.Log("BLOCK !");
    }

    public void StopBlock()
    {
        animator.SetTrigger("StopBlocking");
        StartCoroutine(BlockCooldown());
    }
    
    private void CancelBlock(InputAction.CallbackContext context)
    {
        if (!isBlocking)
            return;
        
        animator.SetTrigger("StopBlocking");
        StartCoroutine(BlockCooldown());
    }

    IEnumerator BlockCooldown()
    {
        InputManager.instance.canBlock = false;
        yield return new WaitForSeconds(0.2f);
        InputManager.instance.canBlock = true;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        int damageToTake = 0;
        
        if (collision.gameObject.tag != "EnemyHit")
            return;

        if ((collision.transform.position.x > transform.position.x && isBlocking && playerController.facingDirection == 1) || (collision.transform.position.x < transform.position.x && isBlocking && playerController.facingDirection == -1)) 
        {
            animator.SetTrigger("HitBlocked");
            if (!isInBurstMode)
                AddBurstStacks();

            blockSoundToPlay = blockSoundList[Random.Range(0, blockSoundList.Count)];
            damageToTake = DamageToTakeFromHit(collision) / 2;
            if (Time.time - blockThisFrame < perfectParryWindowDuration)
            {
                Debug.Log("PERFECT PARRY !");
                StartCoroutine(PerfectParry());
                rumbleScript.RumbleConstant(lowARumble,highARumble, rumbleDuration);
                blockSoundToPlay = perfectParrySound;
                damageToTake = 0;
            }

            audioSource.clip = blockSoundToPlay;
            audioSource.Play();
        }
        else
        {
            damageToTake = DamageToTakeFromHit(collision);
            StartCoroutine(ChangeColorAfterHit());
            //TODO - Add Sound
        }

        health -= damageToTake;
        healthBar.GetComponent<RectTransform>().sizeDelta -= new Vector2(damageToTake * 256 / maxHealth, 0);
    }

    IEnumerator ChangeColorAfterHit()
    {
        GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.1f);
        GetComponent<SpriteRenderer>().color = Color.white;
    }
    
    private int DamageToTakeFromHit(Collider2D collision)
    {
        int damageToTake = 0;
        switch (collision.name)
        {
            case "HitBoxAttack1Pattern1":
                damageToTake = 15;
                break;
            case "HitBoxAttack2Pattern1":
                damageToTake = 30;
                break;
            case "HitBoxAttack1Pattern2":
                damageToTake = 20;
                break;
            case "HitBoxAttack2Pattern2":
                damageToTake = 40;
                break;
        }
        return damageToTake;
    }
    
    IEnumerator PerfectParry()
    {
        perfectParryVFX.Play();
        perfectParryLight.SetActive(true);
        SlowMotion();
        cmVcam.m_Lens.OrthographicSize = 4f;

        yield return new WaitForSeconds(0.25f);
        
        isParrying = true;
        //cmVcam.m_Lens.OrthographicSize = 5f;
        perfectParryLight.SetActive(false);
        StopSlowMotion();
    }

    private void CameraZoomParry()
    {
        if (!isParrying)
            return;

        elapsedZoomTime += Time.deltaTime;
        cmVcam.m_Lens.OrthographicSize = Mathf.Lerp(4f, 5f, elapsedZoomTime / replaceDuration);

        if (cmVcam.m_Lens.OrthographicSize == 5f)
        {
            isParrying = false;
            elapsedZoomTime = 0f;
        }
    }

    
    
    private void SlowMotion()
    {
        Time.timeScale = 0.3f;
    }

    private void StopSlowMotion()
    {
        Time.timeScale = 1f;
    }
    
    private void AddBurstStacks()
    {
        if (burstStacks >= 4)
            return;

        burstStacks++;
        //burstGauge.sizeDelta += new Vector2(25, 0);//TODO
        burstGauge.offsetMax += new Vector2(25, 0);
    }

    private void TryBurstMode(InputAction.CallbackContext context)
    {
        if (burstStacks < 1)
            return;

        burstCast = true;
        burstCastTimer = 0;
        Debug.Log("CAST BURST MODE!");
    }

    private void CastBurstMode()
    {
        if(!burstCast)
            return;

        CameraZoomBurst();
        burstCastTimer += Time.deltaTime;
        if (burstCastTimer >= 0.8f)
        {
            burstVFX.Play();
            isInBurstMode = true;
            burstDuration = burstDurationOneStack * burstStacks;
            burstCast = false;
            elapsedBurstTime = 0;
            animator.SetTrigger("StopCastBurst");
            burstGaugeXPos = burstGauge.offsetMax.x;
            burstStackAtBurstMode = burstStacks;
            Debug.Log("ENTER BURST MODE !");
            ReplaceCameraBurst();
        }
    }

    private void CameraZoomBurst()
    {
        cmVcam.m_Follow.transform.position = new Vector3(0,Mathf.Lerp(0f, cmVcam.m_Follow.GetComponent<PositionConstraint>().GetSource(0).sourceTransform.position.y + 1.75f, burstCastTimer / 0.8f),0);
        cmVcam.m_Lens.OrthographicSize = Mathf.Lerp(5f, 3f, burstCastTimer / 0.8f);
    }

    private void ReplaceCameraBurst()
    {
        cmVcam.m_Follow.transform.position = Vector3.zero;
        cmVcam.m_Lens.OrthographicSize = 5f;
        //cmVcam.m_Lens.OrthographicSize = Mathf.Lerp(3f, 5f, burstCastTimer / 0.8f); //TODO - Make it lerp
    }
    
    private void DecreaseBurstGauge()
    {
        if (!isInBurstMode)
            return;

        if (burstStacks <= 0)
        {
            isInBurstMode = false;
            burstVFX.Stop();
        }
        else
        {
            elapsedBurstTime += Time.deltaTime;
            //burstVFX.SetFloat("Rate", Mathf.Pow(4,burstStacks)); // Decrease way faster but this is more visible
            burstVFX.SetFloat("Rate",Mathf.Lerp(32 * burstStackAtBurstMode,0, elapsedBurstTime / burstDuration)); //Decrease slowly but is less visible until the end if start with 256
            burstStacks = Mathf.Lerp(burstStackAtBurstMode, 0, elapsedBurstTime / burstDuration);
            burstGauge.offsetMax = new Vector2(Mathf.Lerp(burstGaugeXPos, -100, elapsedBurstTime / burstDuration),burstGauge.offsetMax.y);
        }
    }
    
    private void CancelCastBurstMode(InputAction.CallbackContext context)
    {
        if (!burstCast)
            return;
        
        burstGauge.offsetMax += new Vector2(-25, 0);
        burstStacks--;
        burstCast = false;
        ReplaceCameraBurst();
    }
    
}