using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{

    public Animator anim;
    private float nextAttackTime = 0f;
    public float attackRate = 2f;
    public Enemy E;
 
    public Transform attackPos;
    public float attackRange;
    public LayerMask enemies;
    public int damage;
    [SerializeField] private Rigidbody2D rb;
 
 
    void FixedUpdate()
    {
        if (Time.time >= nextAttackTime)
        {
            if (Input.GetButton("Fire1"))
            {
                anim.SetTrigger("attack");
                Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, enemies);

 
                foreach(Collider2D enemy in enemiesToDamage)
                {
                    var enemyScript = enemy.GetComponent<Enemy>();
                    if (enemyScript != null) {
                        enemyScript.Knockback(transform);
                        enemyScript.TakeDamage(damage);
                    }
                    var enemyAIScript = enemy.GetComponent<EnemyAI>();
                    if (enemyAIScript != null)
                    {
                        enemyAIScript.Knockback(transform);
                        enemyAIScript.TakeDamage(damage);
                    }
                }
                nextAttackTime = Time.time + 1f / attackRate;
            }

        }
    }
 
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }
}

