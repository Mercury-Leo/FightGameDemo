using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Basic : MonoBehaviour
{

    protected Animator enemy_animator;
    protected Rigidbody2D enemy_rigidbody;

    protected int max_health = 100;
    protected int current_health;


    // Start is called before the first frame update
    protected virtual void Start()
    {
        enemy_rigidbody = GetComponent<Rigidbody2D>();
        enemy_animator = GetComponent<Animator>();
        current_health = max_health;
    }

    public virtual void take_damage(int damage)
    {
        current_health -= damage;
    }

    void death()
    {
        Destroy(this.gameObject);
    }

}
