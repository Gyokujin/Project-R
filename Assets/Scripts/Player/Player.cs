using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance = null;

    [SerializeField]
    private int num;

    [Header("Status")]
    [Range(0, 100)]
    public float playerHP;
    [Range(0, 10)]
    public float playerATK;
    [SerializeField][Range(0, 10)]
    private float runSpeed = 3;
    private float defaultSpeed;
    [SerializeField][Range(0, 12)]
    private float jumpForce;

    [Header("Move")]
    [SerializeField]
    private int jumpCount;
    private int jumpCnt;
    private bool onMove;
    private bool onGround;
    private bool onCrouch;
    [SerializeField]
    private float checkDistance;
    private float inputX;
    private float inputY;
    private float isRight = 1; // �ٶ󺸴� ���� 1 = ������, -1 = ����
    [SerializeField]
    private float slidingSpeed;
    [SerializeField]
    private float wallJumpPower;
    private bool isWallJump;

    [Header("Action")]
    [SerializeField]
    private float dashSpeed;
    [SerializeField]
    private float defaultDashTime;
    private float dashTime;
    private bool onDash;
    private bool dashCool;
    [SerializeField]
    private float dashCooldown;
    [SerializeField]
    private int atkCount;
    private int atkCnt;
    [SerializeField]
    private GameObject attackBox;
    private bool onAttack;
    private bool onDamaged;
    private bool onDie;
    public GameObject targetObject;

    [Header("Physics")]
    [SerializeField]
    private Transform groundCheckFront; // �ٴ� üũ position
    [SerializeField]
    private Transform groundCheckBack; // �ٴ� üũ position
    private float standColOffsetY = -0.1564108f;
    private float standColSizeY = 1.030928f;
    private float crouchColOffsetY = -0.32f;
    private float crouchColSizeY = 0.7f;
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
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rigid;
    private BoxCollider2D collider;

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
        collider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        defaultSpeed = runSpeed;
        dashCool = true;
        jumpCnt = jumpCount;
        atkCnt = atkCount;
    }

    void Update()
    {
        GetInput();
        Crouch();
        Jump();
        Sliding();
        Interaction();
    }

    void FixedUpdate()
    {
        Move();
    }

    void GetInput()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");

        if (Input.GetButtonDown("Attack") && atkCount > 0 && !onMove && onGround && !onCrouch && !onDamaged && !onDie && !onDash)
        {
            Attack();
        }

        if (Input.GetButtonDown("Dash") && !onAttack && onGround && !onDamaged && !onDie && !onDash && dashCool)
        {
            StartCoroutine("Dash");
        }
    }

    void Move()
    {
        if (onCrouch || onAttack || onDamaged || onDie || isWallJump || onDash)
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

    void Crouch()
    {
        if (onMove || !onGround || onAttack || onDamaged || onDie || isWall || onDash)
            return;

        // #. ĳ���� ���̱�
        onCrouch = inputY < 0 ? true : false;
        animator.SetBool("onCrouch", onCrouch);

        if (onCrouch)
        {
            rigid.velocity = new Vector2(0, rigid.velocity.y);
            collider.offset = new Vector2(collider.offset.x, crouchColOffsetY);
            collider.size = new Vector2(collider.size.x, crouchColSizeY);
        }
        else
        {
            collider.offset = new Vector2(collider.offset.x, standColOffsetY);
            collider.size = new Vector2(collider.size.x, standColSizeY);
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

    IEnumerator Dash()
    {
        dashCool = false;
        onDash = true;
        gameObject.layer = 9;
        animator.SetBool("onDash", true);
        dashTime = defaultDashTime;

        if (onMove && !onCrouch)
        {
            while (dashTime > 0 && inputX != 0)
            {
                rigid.velocity = new Vector2(inputX * dashSpeed, rigid.velocity.y);
                dashTime -= Time.deltaTime;
                yield return null;
            }

            if (dashTime > 0)
            {
                dashTime = 0;
                gameObject.layer = 3;
                animator.SetBool("onMove", false);
            }

            yield return new WaitForSeconds(dashTime);
            runSpeed = defaultSpeed;
            animator.SetBool("onDash", false);
        }
        else if (!onMove && onCrouch)
        {
            onCrouch = true;
            animator.SetBool("onCrouch", true);
            rigid.AddForce(Vector2.left * -isRight * 280);

            yield return new WaitForSeconds(dashTime);
            rigid.velocity = new Vector2(0, rigid.velocity.y);
            animator.SetBool("onDash", false);
        }
        else if (!onMove && !onCrouch)
        {
            while (dashTime > 0 && inputX == 0)
            {
                dashTime -= Time.deltaTime;
                rigid.velocity = new Vector2(dashSpeed * isRight, 0);
                animator.SetBool("onMove", true);
                animator.SetBool("onDash", true);
                yield return null;
            }

            dashTime = 0;
            rigid.velocity = new Vector2(rigid.velocity.x, rigid.velocity.y);
            animator.SetBool("onMove", false);
            animator.SetBool("onDash", false);
        }
        onDash = false;
        gameObject.layer = 3;

        yield return new WaitForSeconds(dashCooldown);
        dashCool = true;
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

    void Interaction()
    {
        if (Input.GetButtonDown("Interaction") && targetObject != null)
        {
            switch (targetObject.GetComponent<Object>().objectType.ToString())
            {
                case "TreasureBox":
                    targetObject.GetComponent<TreasureBox>().Spawn();
                    break;
                case "PortalRing":
                    targetObject.GetComponent<Portal>().Teleport(gameObject);
                    break;
            }
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

        if (collision.gameObject.CompareTag("Object"))
        {
            targetObject = collision.gameObject;
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