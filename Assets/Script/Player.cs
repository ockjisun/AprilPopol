using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit
{
    public static Player Instance { get; private set; }    // // Player 클래스의 싱글턴 인스턴스

    [SerializeField] Ability grow;  // Player 클래스의 싱글턴 인스턴스

    Movement2D movement2D; // Player 클래스의 싱글턴 인스턴스
    [SerializeField] float attackRadius;    // 플레이어가 적을 공격할 수 있는 반경

    Animator anim;                      // 애니메이터.
    Vector2 currentInput;               // 현재 입력 값.
    SpriteRenderer spriteRenderer;      // 스프라이트 렌더러.
    LayerMask enemyMask;                // 에너미 레이어 마스크

    [SerializeField] float groundRadius;    // 지면 탐지 반경
    [SerializeField] Vector3 groundOffset;  // 지면 탐지 오프셋

    bool isGrounded;  // 플레이어가 지면에 있는지 여부
    Rigidbody2D rigid;
    float moveSpeed;

    public bool IsGrounded => isGrounded;  // 플레이어가 지면에 있는지 확인하는 속성

    void Start()
    {
        anim = GetComponent<Animator>();  // 애니메이터 컴포넌트 가져오기
        currentInput = Vector2.zero;      // 현재 입력을 0으로 초기화
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyMask = 1 << LayerMask.NameToLayer("Enemy"); // "Enemy" 레이어에 해당하는 비트 마스크를 생성하여 enemyMask 변수에 저장하는 역할
        rigid = GetComponent<Rigidbody2D>();
        isGrounded = false;   // 초기화: 지면에 있지 않음

        movement2D = GetComponent<Movement2D>();  // movement2D 변수에 Movement2D 컴포넌트 할당

        moveSpeed = movement2D.MoveSpeed;  // moveSpeed 변수에 Movement2D의 moveSpeed 값 할당
    }

    private void Update()
    {
        currentInput.x = Input.GetAxisRaw("Horizontal");
        currentInput.y = Input.GetAxisRaw("Vertical");

        

        // 점프 입력 처리
        if (Input.GetKeyDown(KeyCode.Space) && movement2D.Jump())
            anim.SetTrigger("onJump");

        // 이동
        movement2D.Movement(currentInput);

        // 공격
        Attack();

        // 플레이어가 지면에 있는지 확인
        isGrounded = Physics2D.OverlapCircle(transform.position + groundOffset, groundRadius, LayerMask.GetMask("Ground"));
    }    

    private void Attack()
    {
        // 플레이어가 바라보는 방향 결정
        bool facingRight = spriteRenderer.flipX == false;                               // 플레이어가 오른쪽을 바라보고 있는지 여부를 확인
        Vector2 attackDirection = facingRight ? Vector2.right : Vector2.left;           // 레이캐스트를 쏠 방향을 결정
        Debug.DrawRay(transform.position, attackDirection * attackRadius, Color.red);   // 적을 공격하는 범위를 나타내는 원

        // 공격 반경 내의 적 탐지
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, attackRadius, attackDirection, attackRadius, enemyMask);   // attackRadius 범위 내의 적을 찾기

        // 감지된 적에게 데미지 입히기
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
    private void Movement(Vector2 currentInput)
    {
        // 아래 방향키를 누르면 좌, 우 움직임을 멈춰야한다. (=움직일 수 없다)
        if (isGrounded && currentInput.y == -1)
            currentInput.x = 0f;

        // 리지드바디의 속도 설정 (좌, 우 방향 이동)
        rigid.velocity = new Vector2(moveSpeed * currentInput.x, rigid.velocity.y);

        // 이동 방향에 따라 스프라이트 뒤집기
        if (currentInput.x != 0f)
            spriteRenderer.flipX = currentInput.x < 0;
    }

    public void Hit()
    {
        if(movement2D.Throw(8f))
        {
            anim.SetTrigger("onHurt");   // 피해 애니메이션 트리거
            StartCoroutine(IEGodMode()); // 무적 상태 코루틴 시작
        }
    }
    IEnumerator IEGodMode()
    {
        float godModeTime = 2.0f;   // 무적 상태 지속 시간
        float offset = 0.05f;       // 깜빡이는 간격
        float time = offset;        // 반짝이는 시간
        int prevLayer = gameObject.layer;  // 이전 레이어 저장

        spriteRenderer.ChangeAlpha(0.8f);  // 무적 상태를 나타내기 위해 알파 값 변경

        while ((godModeTime -= Time.deltaTime) >= 0.0f)
        {
            if((time -= Time.deltaTime) <= 0.0f)
            {
                time = offset;
                spriteRenderer.enabled = !spriteRenderer.enabled;
            }

            yield return null;
        }

        spriteRenderer.enabled = true;   // 렌더러 활성화
        spriteRenderer.ChangeAlpha(1f);  // 알파 값을 1로 복원
        gameObject.layer = prevLayer;    // 이전 태그로 돌리기

        Collider2D collider = GetComponent<Collider2D>();
        collider.enabled = false; // 콜라이더 비활성화
        yield return null;
        collider.enabled = true;  // 콜라이더 활성화
    }

  
    void LateUpdate()
    {
        // 애니메이터 파라미터 갱신.
        anim.SetBool("isRun", currentInput.x != 0);
        anim.SetBool("isGround", movement2D.IsGrounded);
        anim.SetFloat("velocityY", Mathf.Round(movement2D.Velocity.y));

        currentInput = Vector2.zero; // 입력 값을 처리 후 초기화
    }

    // 게임 레벨에 따라 증가된 능력치를 반환하는 메서드
    protected override Ability GetIncrease()
    {
        // 공격력(power)을 기준으로 증가시키는 코드
        float increasedPower = grow.power * GameManager.Instance.gameLevel;
        float increasedSpeed = grow.speed * GameManager.Instance.gameLevel;
        float increasedHp = grow.hp * GameManager.Instance.gameLevel;

        // 증가된 값으로 새로운 Ability 객체를 생성하여 반환
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
        // 에디터에서 공격 반경을 나타내는 원 그리기
        Gizmos.color = Color.red;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.back, attackRadius);
    }
}


// 스프라이트의 알파 값을 변경하는 확장 메서드
public static class Method
{
    public static void ChangeAlpha(this SpriteRenderer target, float alpha)
    {
        Color color = target.color;
        color.a = alpha;
        target.color = color;
    }
}
