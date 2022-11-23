using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance = null;

    [Header("Status")]
    [Range(0, 100)]
    public float playerHP;
    [Range(0, 10)]
    public float playerATK;
    [SerializeField][Range(0, 10)]
    private float runSpeed = 3;
    [SerializeField][Range(0, 12)]
    private float jumpForce;

    [Header("Move")]
    [SerializeField]
    private int jumpCount;
    private int jumpCnt;
    private bool onMove;
    private bool onGround;
    [SerializeField]
    private float checkDistance;
    private float inputX;
    private float isRight = 1; // �ٶ󺸴� ���� 1 = ������, -1 = ����
    [SerializeField]
    private float slidingSpeed;
    [SerializeField]
    private float wallJumpPower;
    private bool isWallJump;

    [Header("Action")]
    [SerializeField]
    private int atkCount;
    private int atkCnt;
    [SerializeField]
    private GameObject attackBox;
    private bool onAttack;
    private bool onDamaged;
    private bool onDie;

    [Header("Physics")]
    [SerializeField]
    private Transform groundCheckFront; // �ٴ� üũ position
    [SerializeField]
    private Transform groundCheckBack; // �ٴ� üũ position
    [SerializeField]
    private bool isWall;
    [SerializeField]
    private Transform wallCheck;
    [SerializeField]
    private float wallCheckDistance;
    [SerializeField]
    private LayerMask groundLayer;
    [SerializeField]
    private LayerMask wallLayer;

    [Header("Component")]
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rigid;
    private Animator animator;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (instance != this)
                Destroy(this.gameObject);
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        jumpCnt = jumpCount;
        atkCnt = atkCount;
    }

    void Update()
    {
        GetInput();
        Jump();
        Sliding();
    }

    void FixedUpdate()
    {
        Move();
    }

    void GetInput()
    {
        inputX = Input.GetAxisRaw("Horizontal");

        if (Input.GetMouseButtonDown(0) && atkCount > 0 && !onMove && onGround && !onDamaged && !onDie)
        {
            Attack();
        }
    }

    void Move()
    {
        if (onAttack || onDamaged || onDie || isWallJump)
            return;

        // #. ĳ���� �̵�
        rigid.velocity = new Vector2((inputX) * runSpeed, rigid.velocity.y);
        onMove = rigid.velocity.x != 0 ? true : false;

        // #. ĳ������ ���ʰ� ������ �ٴ� üũ�� ����
        bool groundFront = Physics2D.Raycast(groundCheckFront.position, Vector2.down, checkDistance, groundLayer);
        bool groundBack = Physics2D.Raycast(groundCheckBack.position, Vector2.down, checkDistance, groundLayer);

        // #. ���� ���¿��� �� �Ǵ� ���ʿ� �ٴ��� �����Ǹ� �ٴڿ� �پ �̵��ϵ��� ����
        if (!onGround && (groundFront || groundBack))
            rigid.velocity = new Vector2(rigid.velocity.x, 0);

        // #. �� �Ǵ� ������ �ٴ��� �����Ǹ� isGround�� ������
        if (groundFront || groundBack)
        {
            onGround = true;
            jumpCnt = jumpCount;
        }
        else
            onGround = false;

        animator.SetBool("onGround", onGround);

        if (inputX != 0 && !isWallJump)
        {
            // #. ����Ű�� ������ ����� ĳ���Ͱ� �ٶ󺸴� ������ �ٸ��� ĳ������ ������ ��ȯ
            if ((inputX > 0 && isRight < 0) || (inputX < 0 && isRight > 0))
            {
                FlipPlayer();
            }

            animator.SetBool("onMove", true);
        }
        else
        {
            animator.SetBool("onMove", false);
        }
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCnt > 0 && !onAttack && !onDamaged && !onDie)
        {
            // #. ĳ���� ����
            rigid.velocity = Vector2.up * jumpForce;
            animator.SetTrigger("doJump");
        }
        if (Input.GetButtonUp("Jump"))
        {
            jumpCnt--;
        }
    }

    void Sliding()
    {
        if (!onGround)
        {
            isWall = Physics2D.Raycast(wallCheck.position, Vector2.right * isRight, wallCheckDistance, wallLayer);
            animator.SetBool("onSliding", isWall);

            if (isWall)
            {
                rigid.velocity = new Vector2(rigid.velocity.x, rigid.velocity.y * slidingSpeed);
                isWallJump = false;

                if (Input.GetButtonDown("Jump"))
                {
                    isWallJump = true;
                    animator.SetTrigger("doJump");
                    Invoke("FreezeX", 0.3f);
                    rigid.velocity = new Vector2(-isRight * wallJumpPower, 0.9f * wallJumpPower);
                    FlipPlayer();
                }
            }
        }
        else
        {
            animator.SetBool("onSliding", false);
        }
    }

    void Attack()
    {
        onAttack = true;
        atkCnt--;
        animator.SetTrigger("doAttack");
    }

    void OffAttack()
    {
        onAttack = false;
        atkCnt = atkCount;
    }

    IEnumerator OnHitBox()
    {
        attackBox.SetActive(true);

        yield return new WaitForSeconds(0.1f);
        attackBox.SetActive(false);
    }

    // #. �ǰ� ó��
    public void OnDamaged(Vector2 targetPos, float damage)
    {
        if (onDamaged)
            return;

        playerHP -= damage;

        if (playerHP <= 0)
            Die();
        else
        {
            onDamaged = true;
            GameManager.instance.HPSetting("Damage");

            // #. ���̾� ���� (Invincibility)
            gameObject.layer = 9;

            // #. �÷� ����
            spriteRenderer.color = new Color(1, 1, 1, 0.6f);

            // #. �˹�
            int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
            rigid.AddForce(new Vector2(dirc, 0) * 2, ForceMode2D.Impulse);
            animator.SetTrigger("doDamaged");

            // #. �ǰ� ���� ����
            StartCoroutine("OffDamaged");
        }
    }

    IEnumerator OffDamaged()
    {
        yield return new WaitForSeconds(0.5f);
        onDamaged = false;

        yield return new WaitForSeconds(1f);
        gameObject.layer = 3;
        spriteRenderer.color = new Color(1, 1, 1, 1);

        // ������ �ǰ� ���׿� ���� ���� ����
        if (onAttack)
            OffAttack();
    }

    void Die()
    {
        onDie = true;
        gameObject.layer = 9;
        animator.SetTrigger("doDie");

        GameManager.instance.GameOver();
    }

    void FlipPlayer()
    {
        // #. ������ ��ȯ
        transform.eulerAngles = new Vector3(0, Mathf.Abs(transform.eulerAngles.y - 180), 0);
        isRight *= -1;
    }

    void FreezeX()
    {
        isWallJump = false;
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("HitBox"))
        {
            OnDamaged(collision.transform.position, collision.gameObject.GetComponentInParent<Enemy>().atk);
        }
    }

    // #. �ٴ� üũ Ray�� ��ȭ�鿡 ǥ��
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(groundCheckFront.position, Vector2.down * checkDistance);
        Gizmos.DrawRay(groundCheckBack.position, Vector2.down * checkDistance);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(wallCheck.position, Vector2.right * isRight * wallCheckDistance);
    }
}