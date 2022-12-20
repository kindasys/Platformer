using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    public Transform target;
    public float speed = 200f;
    public float nextWaypointDistance = 3f;

    public Transform EnemyGFX;

    private bool tookDamage;
    public int health = 2;

    Path path;
    int currentWaypoint = 0;
    bool reachedEndofPath = false;

    Seeker seeker;
    Rigidbody2D rb;

    public int damage = 1;
    [Header("Knockback")]
    [SerializeField] private Transform center;
    [SerializeField] private float knockbackVel = 8f;
    [SerializeField] public bool knockbacked = false;
    [SerializeField] private float knockbackedTime = 1f;
    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath", 0f, .5f);
    }
    void UpdatePath()
    {
        if(target != null)
        {
            if (seeker.IsDone())
                seeker.StartPath(rb.position, target.position, onPathComplete);
        }
    }
    void onPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }
    private void Update()
    {
        tookDamage = false;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (path == null)
            return;
        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndofPath = true;
            return;
        }
        else
        {
            reachedEndofPath = false;
        }
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;

        if (!knockbacked)
        {
            rb.AddForce(force);
        }
        else
        {
            var lerpedXVelocity = Mathf.Lerp(rb.velocity.x, 0f, Time.deltaTime * 3);
            rb.velocity = new Vector2(lerpedXVelocity, rb.velocity.y);
        }

            float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }
        if (rb.velocity.x >= 0.01f)
        {
            EnemyGFX.localScale = new Vector3(-1, 1f, 1f);
        }
        if (rb.velocity.x <= -0.01f)
        {
            EnemyGFX.localScale = new Vector3(1, 1f, 1f);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player_health player = other.GetComponent<player_health>();
            PlayerMovement movement = other.GetComponent<PlayerMovement>();
            if (player != null)
            {
                movement.Knockback(transform);
                player.TakeDamage(damage);
                Physics2D.IgnoreCollision(player.GetComponent<Collider2D>(), GetComponent<Collider2D>());
            }
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
