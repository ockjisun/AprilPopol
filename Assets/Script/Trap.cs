using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    bool hasHitPlayer = false;      // 플레이어에게 피격 여부 플래그

    private void OnTriggerStay2D(Collider2D collision)
    {
        // 충돌한 객체가 Player 컴포넌트를 가지고 있는지 확인
        Player player = collision.GetComponent<Player>();

        // 충돌한 객체가 Player이고, 아직 피격되지 않았다면
        if (player != null && !hasHitPlayer)
        {
            player.Hit();
            hasHitPlayer = true;    // 플래그를 true로 설정하여 다음 호출에서 피격되지 않도록 함
        }
    }

    // 플레이어가 영역에서 나가면 피격 플래그를 리셋
    private void OnTriggerExit2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player != null)
        {
            hasHitPlayer = false;
        }
    }
}


