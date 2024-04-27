using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] BoxCollider2D limitArea;
    [SerializeField] Vector3 offset;
    [SerializeField] float speed;

    Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();

        Rect a = new Rect(Vector2.zero, new Vector2(10, 10));
        Rect b = new Rect(new Vector2(0, 5f), Vector2.one * 2f);
    }

    private bool InsideBoundary(Rect a, Rect b)
    {
        Bounds boundA = new Bounds(a.position, a.size);
        Bounds boundB = new Bounds(b.position, b.size);
        return boundA.Intersects(boundB);
    }

    private void LateUpdate()
    {
        // ī�޶� ������ ��� 
        float camWidth = cam.orthographicSize * cam.aspect;
        float camHeight = cam.orthographicSize;

        // ���� ���� ��ġ, �ʺ�, ����
        Vector2 limitPos = limitArea.transform.position;
        float limitWidth = limitArea.size.x / 2f;
        float limitHeight = limitArea.size.y / 2f;

        // ī�޶��� �ּ�, �ִ� �̵� ��ġ��
        Vector2 min = new Vector2(limitPos.x - limitWidth + camWidth, limitPos.y - limitHeight + camHeight);
        Vector2 max = new Vector2(limitPos.x + limitWidth - camWidth, limitPos.y + limitHeight - camHeight);

        // ������ ����
        Vector3 destination = target.position + offset;
        destination.x = Mathf.Clamp(destination.x, min.x, max.x);
        destination.y = Mathf.Clamp(destination.y, min.y, max.y);

        // ���� ��ġ ����
        transform.position = Vector3.Lerp(transform.position, destination, speed * Time.deltaTime);
    }

    private void OnDrawGizmos()
    {
        if (limitArea == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(limitArea.transform.position, limitArea.size);
    }
}
