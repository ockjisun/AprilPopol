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
        // 카메라 사이즈 계산 
        float camWidth = cam.orthographicSize * cam.aspect;
        float camHeight = cam.orthographicSize;

        // 제한 구역 위치, 너비, 높이
        Vector2 limitPos = limitArea.transform.position;
        float limitWidth = limitArea.size.x / 2f;
        float limitHeight = limitArea.size.y / 2f;

        // 카메라의 최소, 최대 이동 위치값
        Vector2 min = new Vector2(limitPos.x - limitWidth + camWidth, limitPos.y - limitHeight + camHeight);
        Vector2 max = new Vector2(limitPos.x + limitWidth - camWidth, limitPos.y + limitHeight - camHeight);

        // 목적지 제한
        Vector3 destination = target.position + offset;
        destination.x = Mathf.Clamp(destination.x, min.x, max.x);
        destination.y = Mathf.Clamp(destination.y, min.y, max.y);

        // 실제 위치 대입
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
