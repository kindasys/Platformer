using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class player_health : MonoBehaviour
{
    private bool tookDamage;
    public bool isInvincible = false;
    public int health;
    public int numOfhearts = 6;
    public Animator anim;

    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;
    private bool isHealing = false;

    public PlayerMovement pm;

    [SerializeField] private float InvincibilityFramesDurationS;
    [SerializeField] private float invincibilityDeltaTime;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (health > numOfhearts)
        {
            health = numOfhearts;
        }
        for (int i = 0; i < hearts.Length; i++)
        {
            if(i < health)
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }
            if(i < numOfhearts)
            {
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }
        }
        if(health <= 0)
        {
            Destroy(gameObject);
        }
        tookDamage = false;
        isHealing = false;
    }

    public void TakeDamage(int damage)
    {
        if(isInvincible) return;
        if (!tookDamage)
        {
            health -= damage;
            tookDamage = true;
        }

        StartCoroutine(InvincibilityFrames());

    }
    public void heal()
    {
        if(!isHealing && health < numOfhearts)
        {
            health += 1;
            isHealing = true;
        }
    }
    private IEnumerator InvincibilityFrames()
    {
        Debug.Log("Player turned invincible!");
        isInvincible = true;
        pm.isInvurnelable();
        anim.SetBool("Invincibility", true);
    for (float i = 0; i < InvincibilityFramesDurationS; i += invincibilityDeltaTime)
    {
        // TODO: add any logic we want here
        yield return new WaitForSeconds(invincibilityDeltaTime);
    }
        isInvincible = false;
        anim.SetBool("Invincibility", false);
        Debug.Log("Player isnt invincible anymore"); 
        pm.isntInvurnelable();
    }

    

}
