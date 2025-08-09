using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public class DarkZoneProximity : MonoBehaviour
{
    [Header("UI")]
    public Image darkOverlay;      // Canvas 밑 전체화면 Image (검정, 알파 0으로 시작)

    [Header("Player")]
    public Transform player;       // 플레이어 Transform (없으면 Start에서 Tag=Player로 자동 탐색)

    [Header("Fade Settings")]
    public float outerDistance = 15f;  // 이 거리보다 멀면 완전 밝음(알파 0)
    public float innerDistance = 8f;  // 이 거리보다 가까우면 최대 알파
    public float maxAlpha = 1f;     // 최대 어두움 정도
    public float fadeSpeed = 8f;      // 알파 변화 속도

    [Header("Distance Mode")]
    public bool useColliderClosestPoint = true; // 박스/이상형: 콜라이더 표면까지의 최근접점 기준

    private Collider zoneCol;
    private float currentAlpha = 0f;

    void Start()
    {
        zoneCol = GetComponent<Collider>();
        if (!zoneCol) Debug.LogError("[DarkZone] Collider가 없습니다.");
        if (darkOverlay == null) Debug.LogError("[DarkZone] darkOverlay(Image)가 연결되지 않았습니다.");

        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
            else Debug.LogError("[DarkZone] Player Transform이 비어 있고 Tag=Player도 찾지 못했습니다.");
        }

        // 트리거 필요 없음: 거리 기반이므로 IsTrigger 체크 안 해도 됩니다.
    }

    void Update()
    {
        if (darkOverlay == null || player == null) return;

        // 다크존 기준점 계산: 콜라이더 표면의 최근접점(박스/불규칙 오브젝트에 유리)
        Vector3 refPoint = transform.position;
        if (useColliderClosestPoint && zoneCol != null)
        {
            // 플레이어 위치에서 이 콜라이더까지의 최근접점을 기준으로 거리 계산
            refPoint = Physics.ClosestPoint(player.position, zoneCol, zoneCol.transform.position, zoneCol.transform.rotation);
        }

        float dist = Vector3.Distance(player.position, refPoint);

        // dist=outerDistance → 0, dist=innerDistance → 1
        float t = Mathf.InverseLerp(outerDistance, innerDistance, dist);
        float targetAlpha = maxAlpha * Mathf.Clamp01(t);

        // 부드럽게 알파 보간
        currentAlpha = Mathf.Lerp(currentAlpha, targetAlpha, Time.deltaTime * fadeSpeed);

        var c = darkOverlay.color;
        c.a = currentAlpha;
        darkOverlay.color = c;
    }

#if UNITY_EDITOR
    // 참고용 시각화(대략적인 구면 기준). useColliderClosestPoint=true 일 땐 정확한 박스 윤곽과 다를 수 있어요.
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, outerDistance);
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, innerDistance);
    }
#endif
}
