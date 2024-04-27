using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] Movement2D movement2D;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] bool isLeft;

    CapsuleCollider2D collider2D;

    LayerMask groundMask;
    Vector3 rayHeight;
    Vector3 rayPoint;

    private void Start()
    {
        groundMask = 1 << LayerMask.NameToLayer("Ground");
        rayHeight = Vector3.up * 0.5f;
        collider2D = GetComponent<CapsuleCollider2D>();    
    }

    void Update()
    {
        
    }

    void Raycast()
    {
        // 정면 오브젝트 체크 : 레이어에 충돌하면 방향 돌리기
        float dir = isLeft ? -1f : 1f;  // 방향
        Vector3 point = transform.position + (Vector3.right * 0.5f * dir) + (Vector3.up * collider2D.size.y);  // ray의 시작점
        if(Physics2D.Raycast(point, Vector3.down, collider2D.size.y * 0.9f, groundMask))                       // raycast (충돌 시 true)
        {
            isLeft = !isLeft;
            spriteRenderer.flipX = isLeft;
            dir = isLeft ? -1f : 1f;    
        }

        // 절벽 체크 : 바닥이 없으면 절벽이라고 판단하고 방향 바꾸기
        rayPoint = transform.position + (Vector3.right * 0.5f * dir) + rayHeight;
        if(!Physics2D.Raycast(rayPoint, Vector2.down, 1f, groundMask))
        {
            isLeft = !isLeft;
            spriteRenderer.flipX = isLeft;  
        }
    }

    void Movement()
    {
        // 방향에 따른 입력 값
        Vector2 input = new Vector2(isLeft ? -1f : 1f, 0f);
        movement2D.Movement(input);
    }

    public void Hit()
    {
        collider2D.enabled = false;
        enabled = false;
        GetComponent<Animator>().SetTrigger("onDead");
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
    }
}
