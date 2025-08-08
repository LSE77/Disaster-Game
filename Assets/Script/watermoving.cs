using UnityEngine;

public class watermoving : MonoBehaviour
{
    public float riseSpeed = 0.3f; // 초당 y scale 증가량
    private bool isRising = true;

    void Update()
    {
        if (!isRising) return;

        Vector3 newScale = transform.localScale;
        newScale.y += riseSpeed * Time.deltaTime;
        transform.localScale = newScale;

        // 참고: 이 아래에 플레이어나 기타 조건에 따른 처리 추가 가능
    }
}
