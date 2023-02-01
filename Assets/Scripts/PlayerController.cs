using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

    private Rigidbody2D rigid_body;
    //private Animator player_animator;
    private Collider2D coll;

    [SerializeField] LayerMask ground;
    [SerializeField] float move_speed = 5f;
    [SerializeField] float jump_speed = 10f;
    [SerializeField] float hurt_force = 10f;

    Player_AnimtionControl Animation_control;

    public float sprite_size = 0.5f;
    public int max_health = 200;
    int current_health;
    float look_left = -1;
    float look_right = 1;
    //enum animation_states {Player_idle, Player_jump, Player_run, Player_punch, Player_push, Player_hurt};

    public string currentState
    {
        get; set;
    }

    private void Start()
    {
        Animation_control = GetComponent<Player_AnimtionControl>();
        rigid_body = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        current_health = max_health;
        look_left *= sprite_size;
        look_right *= sprite_size;
        currentState = Player_AnimtionControl.animation_states.Player_idle.ToString();
    }

    private void Update()
    {
        //if (anim_state != anim_State.hurt)
        // {
        //    player_control();
        // }
        if (currentState != Player_AnimtionControl.animation_states.Player_hurt.ToString())
            player_control();
        VelocityState();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
     
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
      /* // Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if (collision.gameObject.tag == enemy_tag)
        {
            if (anim_state == anim_State.fall)
            {
                //Destroy(collision.gameObject);
                //enemy.JumpedOn();
                Jump();
            }

            else
            {
                anim_state = anim_State.hurt;
               // foreverUI.forever.health_amount--;
               // foreverUI.forever.health_text.text = foreverUI.forever.health_amount.ToString();
               // if (foreverUI.forever.health_amount <= 0)
               // {
                //    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                //}
                if (collision.gameObject.transform.position.x > transform.position.x)
                {
                    //enemy to my right damage and move left
                    rb.velocity = new Vector2(-hurt_force, rb.velocity.y);
                }
                else
                {
                    //enemy to my left and move right
                    rb.velocity = new Vector2(hurt_force, rb.velocity.y);
                }
            }
        }
        */
    }

    private void player_control()
    {
        float hdirection = Input.GetAxis("Horizontal");
        float vdirection = Input.GetAxis("Vertical");

        if (hdirection < 0)
        {
            rigid_body.velocity = new Vector2(-move_speed, rigid_body.velocity.y);
            transform.localScale = new Vector2(look_left, look_right);
        }
        else if (hdirection > 0)
        {
            rigid_body.velocity = new Vector2(move_speed, rigid_body.velocity.y);
            transform.localScale = new Vector2(look_right, look_right);
        }

        if (Input.GetButtonDown("Jump"))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigid_body.position, Vector2.down,1.4f, ground);
            if (hit.collider != null)
                Jump();
        }
    }

    private void Jump()
    {
        rigid_body.velocity = new Vector2(rigid_body.velocity.x, jump_speed);
        Animation_control.ChangeAnimationState(Player_AnimtionControl.animation_states.Player_jump.ToString());
    }

    private void VelocityState()
    {
        //If player is jumping and then falling
        if (currentState == Player_AnimtionControl.animation_states.Player_jump.ToString())
        {
            if (rigid_body.velocity.y < .1f && coll.IsTouchingLayers(ground))
            {
                Animation_control.ChangeAnimationState(Player_AnimtionControl.animation_states.Player_idle.ToString());
            }

        }
        // Player touches ground, return to idle
        else if (currentState == Player_AnimtionControl.animation_states.Player_jump.ToString())
        {
            if (coll.IsTouchingLayers(ground))
            {
                Animation_control.ChangeAnimationState(Player_AnimtionControl.animation_states.Player_idle.ToString());
            }
        }

        else if (currentState == Player_AnimtionControl.animation_states.Player_hurt.ToString())
        {
            if (Mathf.Abs(rigid_body.velocity.x) < .1f)
            {
                Animation_control.ChangeAnimationState(Player_AnimtionControl.animation_states.Player_idle.ToString());
            }
        }

        else if (Mathf.Abs(rigid_body.velocity.x) > 2f)
        {
            //moving left/right
            Animation_control.ChangeAnimationState(Player_AnimtionControl.animation_states.Player_run.ToString());
        }
        //If not punching or kicking
        else if(!Animation_control.player_animator.GetCurrentAnimatorStateInfo(0).IsName(Player_AnimtionControl.animation_states.Player_punch.ToString()) && !Animation_control.player_animator.GetCurrentAnimatorStateInfo(0).IsName(Player_AnimtionControl.animation_states.Player_push.ToString()))
        {
            //Finished moving back to idle
            Animation_control.ChangeAnimationState(Player_AnimtionControl.animation_states.Player_idle.ToString());
        }
    }

    public void take_damage(int damage)
    {
        current_health -= damage;
        if(current_health <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        Debug.Log("curr: " + current_health);
    }

}
