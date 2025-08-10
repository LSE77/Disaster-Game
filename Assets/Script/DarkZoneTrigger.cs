using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public class DarkZoneProximity : MonoBehaviour
{
    [Header("UI")]
    public Image darkOverlay;           // Canvas 밑 전체화면 Image (검정, 알파 0으로 시작)

    [Header("Player")]
    public Transform player;            // 비우면 Tag=Player로 자동 탐색
    private PlayerLightState playerLight;

    [Header("Fade Settings")]
    public float outerDistance = 30f;   // 이 거리보다 멀면 완전 밝음(알파 0)
    public float innerDistance = 8f;    // 이 거리보다 가까우면 최대 알파
    public float maxAlpha = 1f;         // 손전등 없을 때 최대 어두움
    public float fadeSpeed = 8f;        // 알파 변화 속도

    [Header("Distance Mode")]
    public bool useColliderClosestPoint = true; // 콜라이더 표면 최근접점 기준

    [Header("Flashlight Effect")]
    public bool useFixedWhenFlashlight = true;  // 손전등 보유 시 고정 알파 사용할지
    [Range(0f, 1f)] public float flashlightFixedAlpha = 0.5f; // 손전등 보유 시 고정 알파
    public bool ignoreDarkZoneWhenFlashlight = false;        // true면 손전등 보유 시 항상 밝음(알파 0)

    private Collider zoneCol;
    private float currentAlpha = 0f;

    void Start()
    {
        zoneCol = GetComponent<Collider>();
        if (!zoneCol) Debug.LogError("[DarkZone] Collider가 없습니다.");
        if (!darkOverlay) Debug.LogError("[DarkZone] darkOverlay(Image)가 연결되지 않았습니다.");

        if (!player)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
            else Debug.LogError("[DarkZone] Player Transform을 찾지 못했습니다.");
        }

        // 플레이어에서 PlayerLightState 찾아두기(부모/자식까지 탐색)
        if (player)
        {
            playerLight = player.GetComponent<PlayerLightState>();
            if (!playerLight) playerLight = player.GetComponentInChildren<PlayerLightState>(true);
            if (!playerLight) playerLight = player.GetComponentInParent<PlayerLightState>();
        }
    }

    void Update()
    {
        if (!darkOverlay || !player || !zoneCol) return;

        // 1) 기본: 거리 기반 목표 알파 계산
        Vector3 refPoint = transform.position;
        if (useColliderClosestPoint && zoneCol != null)
        {
            refPoint = Physics.ClosestPoint(player.position, zoneCol, zoneCol.transform.position, zoneCol.transform.rotation);
        }
        float dist = Vector3.Distance(player.position, refPoint);
        float t = Mathf.InverseLerp(outerDistance, innerDistance, dist); // outer->0, inner->1
        float targetAlpha = maxAlpha * Mathf.Clamp01(t);

        // 2) 손전등 보유 시 처리
        if (playerLight != null && playerLight.hasFlashlight)
        {
            if (ignoreDarkZoneWhenFlashlight)
            {
                targetAlpha = 0f; // 완전 밝음
            }
            else if (useFixedWhenFlashlight)
            {
                targetAlpha = flashlightFixedAlpha; // 고정 밝기
            }
            // else: 손전등 있어도 거리 기반 유지(원하면 이 분기 삭제/변경)
        }

        // 3) 부드러운 보간
        currentAlpha = Mathf.Lerp(currentAlpha, targetAlpha, Time.deltaTime * fadeSpeed);

        var c = darkOverlay.color;
        c.a = currentAlpha;
        darkOverlay.color = c;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, outerDistance);
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, innerDistance);
    }
#endif
}
