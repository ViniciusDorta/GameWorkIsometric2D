using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float speed;
    private Rigidbody2D rig;
    private Animator anim;
    private SpriteRenderer sprite;
    public Image LifeBar;
    public Image RedBar;

    [Header("Attack Variables")]
    public Transform attackCheck;
    public float radiusAttack;
    public LayerMask layerEnemy;
    float timeNextAttack;
    public int maxHealth = 5;
    int currentHealth;
    public float invincibleTime = 2;
    bool isInvincible;
    float invincibleTimer;
    public int invincibleLayer;
    int defaultLayer;

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
        defaultLayer = gameObject.layer;
    }

    void Update()
    {
        Move();
        Attack();
        Invincible();
    }

    void Move()
    {
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f);

        anim.SetFloat("Horizontal", movement.x);
        anim.SetFloat("Vertical", movement.y);
        anim.SetFloat("Speed", movement.magnitude);

        transform.position = transform.position + movement * speed * Time.deltaTime;

        if (Input.GetAxis("Horizontal") < 0f)
        {
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
        }

        if (Input.GetAxis("Horizontal") > 0f)
        {
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }
    }

    void Attack()
    {
        if (Input.GetButtonDown("Fire3"))
        {
            PlayerAttack();
            anim.SetTrigger("Attack");
        }
    }

    void PlayerAttack()
    {
        Collider2D[] enemiesAttack = Physics2D.OverlapCircleAll(attackCheck.position, radiusAttack, layerEnemy);
        for (int i = 0; i < enemiesAttack.Length; i++)
        {
            enemiesAttack[i].SendMessage("EnemyHit");
            Debug.Log(enemiesAttack[i].name);
        }
    }

    void Flip()
    {
        //sprite.flipX = !sprite.flipX;
        attackCheck.localPosition = new Vector3(-attackCheck.localPosition.x, attackCheck.localPosition.y, 0f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackCheck.position, radiusAttack);
    }

    void Invincible()
    {
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
            {
                isInvincible = false;
            }
        }
    }

    IEnumerator TakingDamage()
    {
        while (isInvincible)
        {
            sprite.enabled = false;
            yield return new WaitForSeconds(0.1f);
            sprite.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }

        gameObject.layer = defaultLayer;
    }

    public void SetHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
                return;
            isInvincible = true;
            invincibleTimer = invincibleTime;
            StartCoroutine(TakingDamage());
            gameObject.layer = invincibleLayer;
        }
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);

        Vector3 LifeBarScale = LifeBar.rectTransform.localScale;
        LifeBarScale.x = (float)currentHealth / maxHealth;
        LifeBar.rectTransform.localScale = LifeBarScale;
        StartCoroutine(DecreasingRedBar(LifeBarScale));
    }

    IEnumerator DecreasingRedBar(Vector3 newScale)
    {
        yield return new WaitForSeconds(0.5f);
        Vector3 RedBarScale = RedBar.transform.localScale;
        while (RedBar.transform.localScale.x > newScale.x)
        {
            RedBarScale.x -= Time.deltaTime * 0.25f;
            RedBar.transform.localScale = RedBarScale;

            yield return null;
        }

        RedBar.transform.localScale = newScale;
    }


}

