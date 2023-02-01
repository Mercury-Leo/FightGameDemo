using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_AnimtionControl : MonoBehaviour
{
    public Animator player_animator;
    PlayerController player_controller;
    public enum animation_states { Player_idle, Player_jump, Player_run, Player_punch, Player_push, Player_hurt };

    // Start is called before the first frame update
    void Start()
    {
        player_controller = GetComponent<PlayerController>();
        player_animator = GetComponent<Animator>();
    }

    public void ChangeAnimationState(string newState)
    {
        if (player_controller.currentState == newState) return;

        player_animator.Play(newState);

        player_controller.currentState = newState;
    }
}
