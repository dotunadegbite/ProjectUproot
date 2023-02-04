using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AgentState {
    IDLE,
    ATTACK,
    HURT
}

public class Agent : MonoBehaviour
{
    [SerializeField] public bool isUser;

    // Stats
    [SerializeField] public int health;
    [SerializeField] public int attack;
    [SerializeField] public int speed;

    public bool isDead;

    protected Animator animator;

    public void Start() {
        animator = GetComponent<Animator>();animator = GetComponent<Animator>();
    }

    public void SetAttackTrigger(bool value) {
        animator.SetBool("shouldAttack", value);
    }

    public bool AnimatorIsPlaying(){
     return animator.GetCurrentAnimatorStateInfo(0).length >
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
  }
}
