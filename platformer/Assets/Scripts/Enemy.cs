using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    private bool movingRight = true;
    private bool tookDamage;

    public int damage = 1;

    public Transform GroundCheck;
    public int health = 3;
    // Start is called before the first frame update
    [SerializeField] private Rigidbody2D rb;
    [Header("Knockback")]
    [SerializeField] private Transform center;
    [SerializeField] private float knockbackVel = 8f;
    [SerializeField] public bool knockbacked = false;
    [SerializeField] private float knockbackedTime = 1f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*if(!knockbacked)
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        }*/
        RaycastHit2D groundInfo = Physics2D.Raycast(GroundCheck.position, Vector2.down, 2f);
        if(groundInfo.collider == false)
        {
            if(movingRight)
            {
                transform.eulerAngles = new Vector3(0, -180, 0);
                movingRight = false;
            }else{
                transform.eulerAngles = new Vector3(0, 0, 0);
                movingRight = true;
            }
        }
        if(health <= 0)
        {
            Destroy(gameObject);
        }
        tookDamage = false;
    }
    void FixedUpdate()
    {
        if(!knockbacked)
        {       
            //transform.Translate(Vector3.right * speed * Time.deltaTime);   
            if(movingRight)
            {
                rb.velocity = new Vector2(speed, rb.velocity.y);

            }
            if(!movingRight)
            {
                rb.velocity = new Vector2(-speed, rb.velocity.y);
            }
        }
        else
        {
            var lerpedXVelocity = Mathf.Lerp(rb.velocity.x, 0f, Time.deltaTime * 3);
            rb.velocity = new Vector2(lerpedXVelocity, rb.velocity.y);
        } 
    }
    public void TakeDamage(int damage)
    {
        if (!tookDamage)
        {
            health -= damage;
            tookDamage = true;
        }

    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Ground"))
        {
            if(movingRight)
            {
                transform.eulerAngles = new Vector3(0, -180, 0);
                movingRight = false;
            }else{
                transform.eulerAngles = new Vector3(0, 0, 0);
                movingRight = true;
            }
        }

        if(other.CompareTag("Player"))
        {
            player_health player = other.GetComponent<player_health>();
            PlayerMovement movement = other.GetComponent<PlayerMovement>();
            if(player != null)
            {
                movement.Knockback(transform);                
                player.TakeDamage(damage);
                Physics2D.IgnoreCollision(player.GetComponent<Collider2D>(), GetComponent<Collider2D>());
            }
        }
    }
    public void Knockback(Transform t)
    {   
        var dir = transform.position - t.position;
        knockbacked = true;
        rb.velocity = dir.normalized * knockbackVel;
        StartCoroutine(Unknockback());
     
    }
    private IEnumerator Unknockback()
    {
        yield return new WaitForSeconds(knockbackedTime);
        knockbacked = false;
    }
}
