using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Combat : MonoBehaviour
{

    Player_AnimtionControl player_animation;
    PlayerController player_controller;

    public Transform PunchZone;
    public Transform PushZone;
    public float PunchRange = 0.5f;
    public float PushRange = 0.5f;
    public int PunchDamage = 20;
    public int PushDamage = 40;
    public float AttackRate = 2f;
    public LayerMask enemylayer;


    float nextAttackTime = 0f;

    void Start()
    {
        player_animation = GetComponent<Player_AnimtionControl>();
        player_controller = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time >= nextAttackTime)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                attack_punch();
                nextAttackTime = Time.time + 1f / AttackRate;
            }

            else if (Input.GetKeyDown(KeyCode.Q))
            {
                attack_push();
                nextAttackTime = Time.time + 1f / AttackRate;
            }
        }
        
    }

    void attack_punch()
    {
        //Play punch animation
        player_animation.ChangeAnimationState(Player_AnimtionControl.animation_states.Player_punch.ToString());
        //Detect nearby enemies
        damage_enemies(PunchDamage);
    }

    void attack_push()
    {
        //Play punch animation
        player_animation.ChangeAnimationState(Player_AnimtionControl.animation_states.Player_push.ToString());
        //Detect nearby enemies
        damage_enemies(PushDamage);
    }

    void damage_enemies(int damage)
    {
        Collider2D[] hitEnemies = enemies_detected();

        //Deal damage to enemy
        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("Hitted " + enemy.name);
            enemy.GetComponent<Enemy_controller>().take_damage(damage);
        }
    }

    Collider2D[] enemies_detected()
    {
        return Physics2D.OverlapCircleAll(PunchZone.position, PunchRange, enemylayer);
    }

    private void OnDrawGizmosSelected()
    {
        if (PunchZone == null)
            return;
        if (PushZone == null)
            return;
        Gizmos.DrawWireSphere(PunchZone.position, PunchRange);
        Gizmos.DrawWireSphere(PushZone.position, PushRange);
    }
}
