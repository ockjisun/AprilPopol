using System;
using System.Collections;
using UnityEngine;

public class ExpObject : RootBehaviour
{
    public enum TYPE
    {
        KIWI,
        APPLE,
        MELON,
    }

    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Sprite[] sprites;

    int amount;

    public void Setup(TYPE type)
    {
        spriteRenderer.sprite = sprites[(int)type];
        amount = type switch
        {
            TYPE.KIWI => 10,
            TYPE.APPLE => 20,
            TYPE.MELON => 30,
            _ => -1,
        };
    }
    public void ContactPlayer(Transform target, Action<int> getExpEvent)
    {
        gameObject.layer = 0;
        StartCoroutine(IEUpdate(target, getExpEvent));
    }

    private const float DETEACT_POWER = -6f;   // 최초에 접속되었을 떄 받는 힘
    private const float GRAVITY = 17f;         // 중력
                                      
    IEnumerator IEUpdate(Transform target, Action<int> getExpEvent)
    {
        // 최초에 타겟의 반대 방향으로 벡터를 구해 오브젝트 이동
        float gravity = DETEACT_POWER;
        while (true)
        {
            if(!isPauseObject)
            {
                Vector3 dir = (target.position - transform.position).normalized;   // 타겟을 향하는 단위 벡터
                transform.position += dir * gravity * Time.deltaTime;              // 타겟 중심으로 현재 힘(gravity)만큼 이동
                if (Vector3.Distance(transform.position, target.position) <= 0.1f) // 남은 거리가 0.1보다 적을 경우
                    break;

                gravity += GRAVITY * Time.deltaTime;                               // 중력 가속도 더하기
            }

            yield return null;  
        }

        getExpEvent?.Invoke(amount);
        Destroy(gameObject);
    }
}
