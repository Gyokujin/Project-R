using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    private bool onAttack;
    [SerializeField]
    private GameObject hitBox;

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
        Check();
    }

    // #1. �÷��̾� ����
    void Check()
    {
        if (!onAttack)
        {
            Debug.DrawRay(rigid.position, Vector2.left * 1.4f, Color.red); // ���� �þ�
            Debug.DrawRay(rigid.position, Vector2.right * 1.4f, Color.red); // ���� �þ�
            RaycastHit2D leftCheck = Physics2D.Raycast(rigid.position, Vector2.left, 1.4f, LayerMask.GetMask("Player"));
            RaycastHit2D rightCheck = Physics2D.Raycast(rigid.position, Vector2.right, 1.4f, LayerMask.GetMask("Player"));

            if (leftCheck.collider != null) // ���� ����
            {
                gameObject.transform.localScale = new Vector3(1, 1, 1);
                StartCoroutine("Attack");
            }
            else if (rightCheck.collider != null) // ���� ����
            {
                gameObject.transform.localScale = new Vector3(-1, 1, 1);
                StartCoroutine("Attack");
            }
        }
    }

    // #2. ���� ����
    IEnumerator Attack()
    {
        onAttack = true;

        yield return new WaitForSeconds(0.6f);
        Debug.Log("����");        
        animator.SetTrigger("doAttack");
        hitBox.SetActive(true);
        
        yield return new WaitForSeconds(0.1f);
        hitBox.SetActive(false);

        yield return new WaitForSeconds(1.4f);
        onAttack = false;
    }
}