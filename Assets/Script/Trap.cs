using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    bool hasHitPlayer = false;      // �÷��̾�� �ǰ� ���� �÷���

    private void OnTriggerStay2D(Collider2D collision)
    {
        // �浹�� ��ü�� Player ������Ʈ�� ������ �ִ��� Ȯ��
        Player player = collision.GetComponent<Player>();

        // �浹�� ��ü�� Player�̰�, ���� �ǰݵ��� �ʾҴٸ�
        if (player != null && !hasHitPlayer)
        {
            player.Hit();
            hasHitPlayer = true;    // �÷��׸� true�� �����Ͽ� ���� ȣ�⿡�� �ǰݵ��� �ʵ��� ��
        }
    }

    // �÷��̾ �������� ������ �ǰ� �÷��׸� ����
    private void OnTriggerExit2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player != null)
        {
            hasHitPlayer = false;
        }
    }
}


