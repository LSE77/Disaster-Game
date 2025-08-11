using UnityEngine;

public class PlayerCameraWaterEffect : MonoBehaviour
{
    [Header("화면에 띄울 파란 오버레이 (UI Image)")]
    public GameObject blueOverlay;

    [Header("물 오브젝트 (큐브, 평면 등)")]
    public Transform waterTransform;

    [Header("물 표면 위치 보정값")]
    public float waterSurfaceOffset = 0f; // 필요한 경우 약간의 오차 조정용 (예: 0.5f)

    void Update()
    {
        // 연결되지 않은 경우 예외 방지
        if (Camera.main == null || blueOverlay == null || waterTransform == null)
            return;

        // ① 카메라의 월드 위치 Y
        float cameraY = Camera.main.transform.position.y;

        // ② 물 표면의 높이 = 물 위치 + (높이 스케일 / 2) + 보정값
        float waterSurfaceY = waterTransform.position.y + (waterTransform.localScale.y * 0.5f) + waterSurfaceOffset;

        // ③ 디버깅 로그 출력
        Debug.Log($"[Water Effect] 카메라Y: {cameraY} / 물표면Y: {waterSurfaceY}");

        // ④ 물 속에 있으면 오버레이 활성화
        if (cameraY < waterSurfaceY)
        {
            if (!blueOverlay.activeSelf)
            {
                blueOverlay.SetActive(true);
                Debug.Log(">> 물에 잠김: 파란 오버레이 ON");
            }
        }
        else
        {
            if (blueOverlay.activeSelf)
            {
                blueOverlay.SetActive(false);
                Debug.Log(">> 물 밖: 파란 오버레이 OFF");
            }
        }
    }
}