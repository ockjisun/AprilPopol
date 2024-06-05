using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Unit
{
    [SerializeField] Ability grow;
    [SerializeField] Transform damagePivot;   // 데미지가 출력되는 기준

    [SerializeField] Movement2D movement2D;
    [SerializeField] bool isLeft;

    SpriteRenderer spriteRenderer;
    CapsuleCollider2D collider2D;
    Rigidbody2D rigid;
    Player target;
    Animator anim;

    LayerMask groundMask;
    Vector3 rayHeight;  // 레이캐스트 시작 위치 오프셋
    Vector3 rayPoint;   // 레이캐스트 시작 위치

    float delayTime;

    public float health = 100f;  // 적의 체력

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        groundMask = 1 << LayerMask.NameToLayer("Ground");  // "Ground" 레이어 마스크 생성
        rayHeight = Vector3.up * 0.5f;
        collider2D = GetComponent<CapsuleCollider2D>();
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        movement2D = GetComponent<Movement2D>();
    }


    // 플레이어가 트리거 안으로 들어왔을 때 호출되는 메서드
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player != null)
            target = player;  // 플레이어를 타겟으로 설정
    }

    // 플레이어가 트리거 밖으로 나갔을 때 호출되는 메서드
    private void OnTriggerExit2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player != null)
            target = null;  // 타겟 해제
    }

    void Update()
    {
        Raycast();        // 레이캐스트를 사용하여 장애물 및 절벽 감지
        Movement();       // 적의 이동 처리  
        AttackToPlayer(); // 플레이어를 공격
    }

    // 레이캐스트를 사용하여 장애물 및 절벽 감지
    void Raycast()
    {
        // 정면 오브젝트 체크: 레이어에 충돌하면 방향 전환
        float dir = isLeft ? -1f : 1f;  // 방향
        Vector3 point = transform.position + (Vector3.right * 0.5f * dir) + (Vector3.up * collider2D.size.y);  // ray의 시작점
        if (Physics2D.Raycast(point, Vector3.down, collider2D.size.y * 0.9f, groundMask))                      // raycast (충돌 시 true)
        {
            isLeft = !isLeft;              // 방향 전환
            spriteRenderer.flipX = isLeft; // 스프라이트 뒤집기
            dir = isLeft ? -1f : 1f;
        }

        // 절벽 체크: 바닥이 없으면 방향 전환
        rayPoint = transform.position + (Vector3.right * 0.5f * dir) + rayHeight;
        if (!Physics2D.Raycast(rayPoint, Vector2.down, 1f, groundMask))
        {
            isLeft = !isLeft;
            spriteRenderer.flipX = isLeft;
        }
    }

    // 적의 이동 처리
    void Movement()
    {
        Vector2 input = new Vector2(isLeft ? -1f : 1f, 0f); // 방향에 따른 입력 값 설정
        movement2D.Movement(input);                         // 이동 처리
    }

    // 플레이어를 공격하는 메서드
    private void AttackToPlayer()
    {
        if (delayTime > 0f)
        {
            delayTime = Mathf.Clamp(delayTime - Time.deltaTime, 0.0f, cooltime);  // 공격 딜레이 감소
            return;
        }

        if (target != null)
        {
            // target.TakeDamage(power);  // 플레이어에게 데미지 입히기 (주석 처리됨)
            delayTime = cooltime;
        }
    }

    // 데미지를 입었을 때 호출되는 메서드
    public void Damage(float damageAmount)
    {
        health -= damageAmount;  // 체력 감소

        if (health <= 0)
        {
            Dead();  // 체력이 0 이하이면 사망 처리
        }
    }

    // 유닛이 데미지를 받을 때 호출되는 메서드 (오버라이드)
    public override void TakeDamage(float power, float knockback = 0)
    {
        base.TakeDamage(power, knockback);
        DownUI.Instance.AppearDamage(damagePivot.position, power);  // 데미지 UI 출력
        Hit(knockback);    // 데미지를 받을 때마다 Hit 메서드를 호출하여 knockback 적용
    }


    // 적이 죽었을 때 호출되는 메서드
    public virtual void DeadForce()
    {
        hp = 0;  // 체력을 0으로 설정
        Dead();
    }

    // 적이 데미지를 받을 때 호출되는 메서드
    public void Hit()
    {
        collider2D.enabled = false;
        enabled = false;
        GetComponent<Animator>().SetTrigger("onDead");
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;   // 리지드바디 타입을 Static으로 변경
    }

    // 사망 처리 메서드 (오버라이드)
    protected override void Dead()
    {
        GetComponent<Collider2D>().enabled = false;
        rigid.velocity = Vector2.zero;
        anim.SetTrigger("onDead");
        StartCoroutine(IEDead());     // 사망 코루틴 시작
        gameObject.SetActive(false);
        // ExpObject exp = ExpObjectPool.Instance.GetRandomExpObject();
        // exp.transform.position = transform.position;   // 경험치 오브젝트 생성 

    }


    // 사망 후 페이드 아웃 처리하는 코루틴
    private IEnumerator IEDead()
    {
        float fadeTime = 1.0f;
        Color color = spriteRenderer.color;
        while (fadeTime > 0.0f)
        {
            fadeTime = Mathf.Clamp(fadeTime - Time.deltaTime, 0.0f, 1.0f);
            color.a = fadeTime;
            spriteRenderer.color = color;

            yield return null;
        }

        Destroy(gameObject);   // 게임 오브젝트 삭제
    }


    // 게임 일시 정지 처리 (오버라이드)
    protected override void OnPauseGame(bool isPause)
    {
        base.OnPauseGame(isPause);  // 기본 일시 정지 처리
        rigid.constraints = isPause ? RigidbodyConstraints2D.FreezeAll : RigidbodyConstraints2D.None;
    }

    protected override Ability GetIncrease()
    {
        return grow * GameManager.Instance.gameLevel;    // 성장 능력치 반환
    }
}
