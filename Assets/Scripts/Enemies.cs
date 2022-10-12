using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemies : MonoBehaviour
{
    private Transform alvo;
    [SerializeField]
    private float velocMov;
    [SerializeField]
    private float distMin;
    [SerializeField]
    private Rigidbody2D rig;
    [SerializeField]
    private SpriteRenderer sr;
    [SerializeField]
    private Animator anim;
    [SerializeField]
    private float raioVisao;
    [SerializeField]
    private LayerMask layerAreaVisao;

    void Update()
    {
        SearchPlayer();
        if (this.alvo != null)
        {
            Move();
        }
        else
        {
            StopMove();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(this.transform.position, this.raioVisao);
        if (this.alvo != null)
        {
            Gizmos.DrawLine(this.transform.position, this.alvo.position);
        }
    }

    private void SearchPlayer()
    {
        Collider2D colisor = Physics2D.OverlapCircle(this.transform.position, this.raioVisao, this.layerAreaVisao);
        if (colisor != null)
        {
            Vector2 posicaoAtual = this.transform.position;
            Vector2 posicaoAlvo = colisor.transform.position;
            Vector2 direcao = posicaoAlvo - posicaoAtual;
            direcao = direcao.normalized;

            RaycastHit2D hit = Physics2D.Raycast(posicaoAtual, direcao);
            if (hit.transform != null)
            {
                if (hit.transform.CompareTag("Player"))
                {
                    this.alvo = hit.transform;
                }
                else
                {
                    this.alvo = null;
                }
            }
            else
            {
                this.alvo = null;
            }
        }
        else
        {
            this.alvo = null;
        }
    }

    private void Move()
    {
        Vector2 posicaoAlvo = this.alvo.position;
        Vector2 posicaoAtual = this.transform.position;

        float distancia = Vector2.Distance(posicaoAtual, posicaoAlvo);
        if (distancia >= this.distMin)
        {
            Vector2 direcao = posicaoAlvo - posicaoAtual;
            direcao = direcao.normalized;

            this.rig.velocity = (this.velocMov * direcao);

            if (this.rig.velocity.x > 0)
            {
                this.sr.flipX = true;
                this.anim.SetBool("Move", true);
                this.anim.SetBool("topMove", false);
            }
            else if (this.rig.velocity.x < 0)
            {
                this.sr.flipX = false;
                this.anim.SetBool("Move", true);
                this.anim.SetBool("topMove", false);
            }

            if (this.rig.velocity.y < 0)
            {
                this.sr.flipX = false;
                this.anim.SetBool("topMove", true);
                this.anim.SetBool("Move", false);
            }
            else if (this.rig.velocity.y < 0)
            {
                this.sr.flipX = true;
                this.anim.SetBool("topMove", true);
                this.anim.SetBool("Move", false);
            }
        }
        else
        {
            StopMove();
        }
    }

    private void StopMove()
    {
        this.rig.velocity = Vector2.zero;
        this.anim.SetBool("Move", false);

    }
}
