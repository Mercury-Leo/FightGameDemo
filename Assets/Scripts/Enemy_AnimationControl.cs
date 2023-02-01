using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_AnimationControl : MonoBehaviour
{
    Animator Enemy_animator;
    Enemy_controller enemy_controller;
    public enum animation_states { Firedog_idle, Firedog_walking, Firedog_attack, Firedog_damaged, Firedog_death };

    //public string currentState;

    // Start is called before the first frame update
    void Start()
    {
        enemy_controller = GetComponent<Enemy_controller>();
        Enemy_animator = GetComponent<Animator>();
    }

    public void ChangeAnimationState(string newState)
    {
        if (enemy_controller.currentState == newState) return;

        Enemy_animator.Play(newState);

        enemy_controller.currentState = newState;
    }
}
