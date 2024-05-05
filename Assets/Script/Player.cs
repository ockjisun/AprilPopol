using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit
{
    public static Player Instance { get; private set; }

    [SerializeField] Ability grow;

    [SerializeField] Movement2D movement2D;
    [SerializeField] float attackRadius;

    Animator anim;                      // 애니메이터.
    Vector2 currentInput;               // 현재 입력 값.
    SpriteRenderer spriteRenderer;      // 스프라이트 렌더러.
    LayerMask enemyMask;                // 에너미 레이어 마스크

    [SerializeField] float jumpPower;
    [SerializeField] float groundRadius;
    [SerializeField] Vector3 groundOffset;

    Rigidbody2D rigid;
    bool isGrounded;
    int jumpCount;

    public bool IsGrounded => isGrounded;

    void Start()
    {
        anim = GetComponent<Animator>();
        currentInput = Vector2.zero;
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyMask = 1 << LayerMask.NameToLayer("Enemy");
        movement2D = GetComponent<Movement2D>();

        rigid = GetComponent<Rigidbody2D>();
        isGrounded = false;
        jumpCount = 0;
    }

    private void Update()
    {
        currentInput.x = Input.GetAxisRaw("Horizontal");
        currentInput.y = Input.GetAxisRaw("Vertical");
        movement2D.Movement(currentInput);

        if (currentInput.x != 0f)
            spriteRenderer.flipX = currentInput.x < 0;

        // 점프 입력키를 눌러 Jump를 호출한다.
        // 점프 가능 여부는 Movement2D가 판단한다.
        if (Input.GetKeyDown(KeyCode.Q) && movement2D.Jump())
            anim.SetTrigger("onJump");

        // 발로 적 공격
        LayerMask mask = 1 << LayerMask.NameToLayer("Enemy");
        Collider2D check = Physics2D.OverlapCircle(transform.position, attackRadius, mask);
        if (check != null)
        {
            check.GetComponent<Enemy>().Hit();
            movement2D.Throw(6f);
        }

        // 점프
        LayerMask groundMask = 1 << LayerMask.NameToLayer("Ground");
        isGrounded = Physics2D.OverlapCircle(transform.position + groundOffset, groundRadius, groundMask);

        if (isGrounded && rigid.velocity.y <= 2)
            jumpCount = 1;
    }
    public bool Jump()
    {
        if (jumpCount <= 0)
            return false;

        // ForceMode2D.Force : 지속적인 힘, 미는 거
        // ForceMode2D.Impulse : 응축된 힘, 때리는 거
        rigid.velocity = new Vector2(rigid.velocity.x, 0f);             // 현재 속력 중 y값을 0으로 변경.
        rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);    // 위로 n의 힘만큼 힘을 가한다.
        jumpCount -= 1;                                                 // 점프 가능 횟수 1 감소.        
        return true;
    }

    private void Attack()
    {
        bool facingRight = spriteRenderer.flipX == false;                               // 플레이어가 오른쪽을 바라보고 있는지 여부를 확인
        Vector2 attackDirection = facingRight ? Vector2.right : Vector2.left;           // 레이캐스트를 쏠 방향을 결정
        Debug.DrawRay(transform.position, attackDirection * attackRadius, Color.red);   // 적을 공격하는 범위를 나타내는 원

        
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, attackRadius, attackDirection, attackRadius, enemyMask);   // attackRadius 범위 내의 적을 찾기

        // 찾은 적들에게 데미지를 가합니다.
        foreach (RaycastHit2D hit in hits)
        {
            // 적에게 데미지를 가하는 코드 작성
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                // 적에게 데미지를 가하는 함수 호출 (예시로 Damage 함수를 호출)
                enemy.Damage(grow.power);
            }
        }
    }

    public void Hit()
    {
        if(movement2D.Throw(8f))
        {
            anim.SetTrigger("onHurt");
            StartCoroutine(IEGodMode());
        }
    }
    IEnumerator IEGodMode()
    {
        float godModeTime = 2.0f;   // 지속 시간
        float offset = 0.05f;       // 반짝이는 시간 텀
        float time = offset;        // 반짝이는 시간
        int prevLayer = gameObject.layer;  // 태그 값 캐싱

        spriteRenderer.ChangeAlpha(0.8f);

        while((godModeTime -= Time.deltaTime) >= 0.0f)
        {
            if((time -= Time.deltaTime) <= 0.0f)
            {
                time = offset;
                spriteRenderer.enabled = !spriteRenderer.enabled;
            }

            yield return null;
        }

        spriteRenderer.enabled = true;   // 렌더러 활성화
        spriteRenderer.ChangeAlpha(1f);   
        gameObject.layer = prevLayer;  // 이전 태그로 돌리기

        Collider2D collider = GetComponent<Collider2D>();
        collider.enabled = false;
        yield return null;
        collider.enabled = true;
    }

  
    void LateUpdate()
    {
        // 애니메이터의 파라미터 갱신.
        anim.SetBool("isRun", currentInput.x != 0);
        anim.SetBool("isGround", movement2D.IsGrounded);
        anim.SetFloat("velocityY", Mathf.Round(movement2D.Veclocity.y));

        currentInput = Vector2.zero;
    }
    protected override Ability GetIncrease()
    {
        // 공격력(power)을 기준으로 증가시키는 코드
        float increasedPower = grow.power * GameManager.Instance.gameLevel;
        float increasedSpeed = grow.speed * GameManager.Instance.gameLevel;
        float increasedHp = grow.hp * GameManager.Instance.gameLevel;

        // 증가된 값으로 새로운 Ability 객체를 생성하여 반환합니다.
        return new Ability
        {
            hp = increasedHp,
            power = increasedPower,
            speed = increasedSpeed,
            cooltime = grow.cooltime
        };
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.back, attackRadius);
    }
}

public static class Method
{
    public static void ChangeAlpha(this SpriteRenderer target, float alpha)
    {
        Color color = target.color;
        color.a = alpha;
        target.color = color;
    }
}
