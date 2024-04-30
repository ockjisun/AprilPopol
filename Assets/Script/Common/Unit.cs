using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEditor.MPE;
using UnityEngine;

public abstract class Unit : RootBehaviour
{
    [SerializeField] Ability origin;    // 기본 수치

    // 최종 어빌리티 수치
    private Ability ability;
    public float maxHp => ability.hp;
    public float power => ability.power;    
    public float speed => ability.speed;    
    public float cooltime => ability.cooltime;

    protected float hp;       // 체력
    protected int level;      // 레벨.
    protected int exp;        // 경험치.
    protected int killCount;  // 킬 카운트.

    protected Animator anim;
    public bool isAlive => hp > 0;

    protected override void OnPauseGame(bool isPause)
    {
        base.OnPauseGame(isPause);
        anim.speed = isPause ? 0f : 1f;
    }

    // 경험치 & 레벨
    protected virtual void AddExp(int amoun)
    {
        
    }

    // 능력치 계산 
    public virtual void Setup()
    {
        anim = GetComponent<Animator>();
        hp = int.MaxValue;
        level = 1;
        exp = 0;
        killCount = 0;

        UpdateAbility();
    }

    // 실제 적용 스테이터스 계산
    protected virtual void UpdateAbility()
    {
        ability = origin + GetIncrease();
        hp = Mathf.Clamp(hp, 0, maxHp);
    }

    protected abstract Ability GetIncrease();

    // 피격, 데미지 계산
    public virtual void TakeDamage(float power, float knockback = 0f)
    {
        if (!isAlive)
            return;

        if (Random.value < 0.15f)
        {
            power *= 2;
        }

        hp = Mathf.Clamp(hp - power, 0, maxHp);
        if (hp <= 0)
            Dead();
        else
            Hit(knockback);

    }

    protected virtual void Dead()
    {

    }
    protected virtual void Hit(float knockback)
    {

    }
}
