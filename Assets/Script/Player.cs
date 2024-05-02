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

    void Start()
    {
        anim = GetComponent<Animator>();
        currentInput = Vector2.zero;
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyMask = 1 << LayerMask.NameToLayer("Enemy");
        movement2D = GetComponent<Movement2D>();
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

