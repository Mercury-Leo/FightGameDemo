using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_controller : Enemy_Basic
{
    Enemy_AnimationControl enemy_anim_control;

    public Rigidbody2D player;

    public LayerMask ground;
    public LayerMask player_layer;

    float CanBeAttacked = 0f;
    float AttackedRate = 3f;
    float Aggro_zone = 10f;

    public Transform bite_zone;
    public float bite_range = 1.5f;
    public float movement_speed = 5f;
    public float sprite_size = 0.7f;
    public bool facing_right = true;
    public float attack_rate = 3f;
    public float hurt_force = 10f;
    public int stun_time = 1;
    public int bite_damage = 25;

    float nextAttackTime = 0f;

    bool detected_player = false;
    bool enemy_hurt = false;
    
    float look_left = -1;
    float look_right = 1;

    float enemy_patrol_time = 30f;
    float player_outofzone_timer = 0f;
    float max_time = 5000f;

    public string currentState
    {
        get; set;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        enemy_anim_control = GetComponent<Enemy_AnimationControl>();
        player_outofzone_timer = enemy_patrol_time;
        look_left *= sprite_size;
        look_right *= sprite_size;
    }

    // Update is called once per frame
    void Update()
    {
        detect_player();
        //If the player is near the enemy, start moving
        if (detected_player && !enemy_hurt)
        {
            player_outofzone_timer = 0f;
            chase_player();
        }
        else if(!enemy_hurt)
        {
            //counts how long has passed since player was near enemy.
            player_outofzone_timer += Time.deltaTime;
            if (player_outofzone_timer <= enemy_patrol_time) {
                if (currentState != Enemy_AnimationControl.animation_states.Firedog_damaged.ToString() && current_health > 0)
                    move_enemy();
            }
            //switch to idle if 10 seconds passed without player interaction
            else
                enemy_anim_control.ChangeAnimationState(Enemy_AnimationControl.animation_states.Firedog_idle.ToString());
        }

        if (player_outofzone_timer >= max_time)
            player_outofzone_timer = enemy_patrol_time + 1f;
    }

    //Tries to find if the player is close to the enemy
    void detect_player()
    {
        if (Vector2.Distance(this.transform.position, player.transform.position) <= Aggro_zone)
            detected_player = true;
        else
            detected_player = false;
    }

    void chase_player()
    {
        //player is to the right of enemy
        if (player.position.x > enemy_rigidbody.position.x)
        {
            facing_right = true;
            if(!near_player(1))
                move_direction(movement_speed, look_right); 
        }
        //player is to the left of enemy
        else
        {
            facing_right = false;
            if(!near_player(0))
                move_direction(-movement_speed, look_left);
        }

    }

    bool near_player(float dir)
    {
        RaycastHit2D player_found;
        if (dir == 1)
            player_found = Physics2D.Raycast(enemy_rigidbody.position, Vector2.right, 2f, player_layer);
        else
            player_found = Physics2D.Raycast(enemy_rigidbody.position, Vector2.left, 2f, player_layer);
        //If near player can attack
        if(player_found.collider != null)
        {
            if(!enemy_animator.GetCurrentAnimatorStateInfo(0).IsName(Enemy_AnimationControl.animation_states.Firedog_attack.ToString()) && current_health>0)
                attack_player();
            return true;
        }

        return false;
    }

    void run_enemyidle()
    {
        if (!enemy_animator.GetCurrentAnimatorStateInfo(0).IsName(Enemy_AnimationControl.animation_states.Firedog_death.ToString()) && current_health > 0)
            enemy_anim_control.ChangeAnimationState(Enemy_AnimationControl.animation_states.Firedog_idle.ToString());
    }

    void attack_player()
    {
        if (Time.time >= nextAttackTime)
        {
            if (!enemy_animator.GetCurrentAnimatorStateInfo(0).IsName(Enemy_AnimationControl.animation_states.Firedog_death.ToString()) && current_health > 0)
            {
                enemy_anim_control.ChangeAnimationState(Enemy_AnimationControl.animation_states.Firedog_attack.ToString());
                damage_player(bite_damage);
            }
            nextAttackTime = Time.time + (1f / attack_rate);
        }
    }

    void damage_player(int damage)
    {
        Collider2D[] hitEnemies = player_inrange();

        //Deal damage to enemy
        foreach (Collider2D player in hitEnemies)
        {
            Debug.Log("Hitted " + player.name);
            player.GetComponent<PlayerController>().take_damage(damage);
        }
    }

    Collider2D[] player_inrange()
    {
        return Physics2D.OverlapCircleAll(bite_zone.position, bite_range, player_layer);
    }

    //Moves the enemy left or right
    void move_direction(float speed, float direction)
    {
        if (!enemy_animator.GetCurrentAnimatorStateInfo(0).IsName(Enemy_AnimationControl.animation_states.Firedog_death.ToString()) && current_health > 0)
            enemy_anim_control.ChangeAnimationState(Enemy_AnimationControl.animation_states.Firedog_walking.ToString());
        enemy_rigidbody.velocity = new Vector2(speed, enemy_rigidbody.velocity.y);
        transform.localScale = new Vector2(direction, look_right);
    }

    void move_enemy()
    {
        Can_move();
        if (facing_right)
        {
            move_direction(movement_speed, look_right);
        }
        else
        {
            move_direction(-movement_speed, look_left);
        }

    }

    void Can_move()
    {
        //check if theres something infront of enemy
        RaycastHit2D hit;
        Vector2 lookahead = new Vector2(enemy_rigidbody.position.x, enemy_rigidbody.position.y - 1f);
        if (facing_right)
            hit = Physics2D.Raycast(lookahead, Vector2.right, 2f, ground);
        else 
            hit = Physics2D.Raycast(lookahead, Vector2.left, 2f, ground);
        //If found anything, change directions
        if(hit.collider != null)
        {
            facing_right = !facing_right;
        }
    }

    void enemy_pushed()
    {
        //player is right of enemy
        if (player.position.x > enemy_rigidbody.position.x)
        {
            enemy_rigidbody.velocity = new Vector2(-hurt_force, enemy_rigidbody.velocity.y);
        }
        else
        {
            enemy_rigidbody.velocity = new Vector2(hurt_force, enemy_rigidbody.velocity.y);
        }

    }

    IEnumerator enemy_stunned()
    {
        enemy_hurt = true;
        yield return new WaitForSeconds(stun_time);
        enemy_hurt = false;
    }

    public override void take_damage(int damage)
    {
        if (Time.time >= CanBeAttacked) {
            base.take_damage(damage);
            //stun enemy
            StartCoroutine(enemy_stunned());
            //play hurt animation
            if (!enemy_animator.GetCurrentAnimatorStateInfo(0).IsName(Enemy_AnimationControl.animation_states.Firedog_death.ToString()) && current_health > 0)
                enemy_anim_control.ChangeAnimationState(Enemy_AnimationControl.animation_states.Firedog_damaged.ToString());
            enemy_pushed();
            //if health is under 0
            if (current_health <= 0)
            {
                Debug.Log("enemy died");
                //play death animation
                enemy_anim_control.ChangeAnimationState(Enemy_AnimationControl.animation_states.Firedog_death.ToString());
                GetComponent<Collider2D>().enabled = false;
                enemy_rigidbody.bodyType = RigidbodyType2D.Static;
            }
            //The pause between attcks
            CanBeAttacked = Time.time + 1f / AttackedRate;
        }


        
    }
}
