using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }
    public float MoveSpeed
    {
        set => animator.SetFloat("movementsSpeed", value);
        get => animator.GetFloat("movementsSpeed");
    }
    public void OnReload()
    {
        animator.SetTrigger("onReload");
       
    }
    public bool AimModeIs
    {
        set => animator.SetBool("isAimMode", value);
        get => animator.GetBool("isAimMode");
    }
    public void Play(string stateName, int layer, float normalizedTime)
    {
        animator.Play(stateName, layer, normalizedTime);
    }
    public bool CurrentAnimationIs(string name)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(name);
    }
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
