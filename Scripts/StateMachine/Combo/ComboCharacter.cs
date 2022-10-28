using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class ComboCharacter : MonoBehaviour
{
    [SerializeField] private List<AudioClip> swordWooshesList;
    [SerializeField] private List<AudioClip> swordHitFlesh;
    private AudioSource audioSource;

    [SerializeField] private float amplitude;
    [SerializeField] private float delay;

    private StateMachine meleeStateMachine;
    private bool isAttacking;
    private void Awake()
    {
        InputManager.instance.attack.started += Attack; // TODO - Change to performed and spam attack 1 if maintained with started
        InputManager.instance.attack.canceled += CancelAttack;

        audioSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        meleeStateMachine = GetComponent<StateMachine>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isAttacking && meleeStateMachine.currentState.GetType() == typeof(IdleCombatState))
        {
            meleeStateMachine.SetNextState(new GroundEntryState());
        }
    }

    public void PlaySwordWoosh()
    {
        audioSource.clip = swordWooshesList[Random.Range(0, swordWooshesList.Count)]; //Switch between two sounds
        audioSource.Play();
        //TODO - Move next line to another function or to animator (if instance can be called in animator
        CameraShake.instance.ShakeCamera(delay,amplitude);
    }
    
    IEnumerator AttackDelay()
    {
        isAttacking = true;
        yield return new WaitForSeconds(0.1f);
        isAttacking = false;
    }
    
    private void Attack(InputAction.CallbackContext context)
    {
        if (InputManager.instance.dashBlockAttack)
            return;

        
        StartCoroutine(AttackDelay());
        InputManager.instance.canBlock = false;
    }

    private void CancelAttack(InputAction.CallbackContext context)
    {
        isAttacking = false;
    }
}
