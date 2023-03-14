using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteZombie : Enemy
{
    [Header("Action")]
    private bool onChase;
    [SerializeField]
    private GameObject hitBox;
    [SerializeField]
    private GameObject backSight;

    [Header("Component")]
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
    }

    void Update()
    {
        base.Detect();

        if (!onAttack && onDetect && !onDie)
        {
            DistanceCalculate();
        }
    }

    // #1. ���ݰŸ� Ȯ��
    void DistanceCalculate()
    {
        RaycastHit2D meleeCheck = Physics2D.Raycast(transform.position, Vector2.left * (-sightDir), 0.5f, LayerMask.GetMask("Player")); // ���� Ȯ��
        RaycastHit2D platCheck = Physics2D.Raycast(rigid.position + Vector2.left * (-sightDir) * 0.4f, Vector3.down, 1, LayerMask.GetMask("Platform")); // �ٴ� Ȯ��

        if (meleeCheck || !platCheck)
        {
            StartCoroutine("MeleeAttack");
        }
        else
        {
            StartCoroutine("ChaseAttack");
        }
    }

    // #2. ���� ���� ����
    IEnumerator MeleeAttack()
    {
        onAttack = true;

        yield return new WaitForSeconds(atkTakeTime);
        animator.SetTrigger("doAttack");
        hitBox.SetActive(true);

        yield return new WaitForSeconds(0.1f);
        hitBox.SetActive(false);

        yield return new WaitForSeconds(atkDelay);
        onAttack = false;
    }

    // #3. ���� ���� ����
    IEnumerator ChaseAttack()
    {
        onAttack = true;
        onChase = true;

        yield return new WaitForSeconds(atkTakeTime);
        StartCoroutine("PlatCheck");
        rigid.AddForce(Vector2.left * (-sightDir) * 7.5f, ForceMode2D.Impulse);
        rigid.constraints = RigidbodyConstraints2D.FreezePositionY;
        collider.isTrigger = true;

        yield return new WaitForSeconds(0.03f);
        hitBox.SetActive(true);

        yield return new WaitForSeconds(0.2f);
        animator.SetTrigger("doAttack");

        yield return new WaitForSeconds(0.1f);
        hitBox.SetActive(false);
        collider.isTrigger = false;
        rigid.constraints = RigidbodyConstraints2D.FreezeAll;

        yield return new WaitForSeconds(0.8f);
        onChase = false;

        yield return new WaitForSeconds(atkDelay);
        onAttack = false;
    }


    // #. ������ �ٴ� ����
    IEnumerator PlatCheck()
    {
        while (onChase)
        {
            RaycastHit2D platCheck = Physics2D.Raycast(rigid.position + Vector2.left * (-sightDir) * 0.4f, Vector3.down, 1, LayerMask.GetMask("Platform"));
            
            if (platCheck.collider == null) // ���� ������ �ٴ��� �����Ǹ� ���� ����
            {
                StopCoroutine("ChaseAttack");
                rigid.velocity *= 0;
                hitBox.SetActive(false);
                collider.isTrigger = false;
                rigid.constraints = RigidbodyConstraints2D.FreezeAll;
                onChase = false;
                onAttack = false;
            }

            yield return null;
        }
    }

    // #4. �ǰ� ó��
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (onDie)
            return;

        if (collision.gameObject.CompareTag("PlayerHitBox"))
        {
            float damage = collision.gameObject.GetComponentInParent<Player>().playerATK;
            DamageProcess(damage);
        }
    }

    void DamageProcess(float damage)
    {
        hp -= damage;

        if (hp <= 0)
        {
            onDie = true;
            StartCoroutine("DieProcess");
        }
        else
        {
            BackCheck();
        }
    }

    // #5. ���� ó��
    public IEnumerator DieProcess()
    {
        StopCoroutine("MeleeAttack");
        StopCoroutine("ChaseAttack");
        collider.isTrigger = true;
        spriteRenderer.color = new Color(1, 1, 1, 0.3f);

        yield return new WaitForSeconds(2.0f);
        gameObject.SetActive(false);
    }

    // #6. �Ĺ� Ȯ��
    void BackCheck()
    {
        Debug.DrawRay(backSight.transform.position, Vector2.left * sightDir, Color.blue);
        RaycastHit2D backCheck = Physics2D.Raycast(backSight.transform.position, Vector2.left * sightDir, 1, LayerMask.GetMask("Player"));

        if (backCheck)
        {
            sightDir *= -1;
            transform.localScale = new Vector3(transform.localScale.x * -1, 1, 1);
        }
    }
}