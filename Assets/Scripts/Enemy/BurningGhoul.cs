using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurningGhoul : Enemy
{
    /*
    private Vector2 moveDir;
    private float moveTime;
    private bool onChase;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rigid;
    private BoxCollider2D collider;
    private Animator animator;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();

        Think();
    }

    void Update()
    {
        Check();
    }

    // #1. �̵� ����, �̵� �ð� ����
    void Think()
    {
        int dir = Random.Range(0, 2);
        moveTime = Random.Range(3, 5);

        switch (dir)
        {
            case 0: // ����
                moveDir = Vector3.left;
                spriteRenderer.flipX = false;
                break;

            case 1: // ������
                moveDir = Vector3.right;
                spriteRenderer.flipX = true;
                break;
        }

        StartCoroutine("Move");
    }

    // #2. �̵� ���
    IEnumerator Move()
    {
        while (moveTime > 0)
        {
            rigid.velocity = moveDir;
            moveTime -= Time.deltaTime;
            yield return null;
        }

        Think();
    }

    // #3. �ٴ� �� ������ �÷��̾� ����
    void Check()
    {
        // #. �ٴ� ����
        Debug.DrawRay(rigid.position + moveDir, Vector3.down, Color.green);
        RaycastHit2D platCheck = Physics2D.Raycast(rigid.position + moveDir, Vector3.down, 1, LayerMask.GetMask("Platform"));

        if (platCheck.collider == null)
            Turn();

        // #. �÷��̾� ����
        if (!onChase)
        {
            Debug.DrawRay(rigid.position, moveDir * 4, Color.red);
            RaycastHit2D detectCheck = Physics2D.Raycast(rigid.position, moveDir, 4, LayerMask.GetMask("Player"));

            if (detectCheck.collider != null)
            {
                StopCoroutine("Move");
                StartCoroutine("Chase");
            }
        }
    }

    // #4. �ݴ� �������� �̵�
    void Turn()
    {
        moveDir *= -1;
        spriteRenderer.flipX = !spriteRenderer.flipX;
    }

    // #5. �÷��̾� ����
    IEnumerator Chase()
    {
        onChase = true;
        collider.isTrigger = true;
        animator.SetBool("onChase", true);
        spriteRenderer.color = new Color(0.98f, 0.68f, 0.68f);
        moveTime = 3.5f;

        while (moveTime > 0)
        {
            rigid.velocity = moveDir * 2;
            moveTime -= Time.deltaTime;
            yield return null;
        }

        onChase = false;
        collider.isTrigger = false;
        animator.SetBool("onChase", false);
        spriteRenderer.color = Color.white;
        Think();
    }
    */
}