using UnityEngine;

public class CollapseTrigger : MonoBehaviour
{
    public GameObject normalObject;       // 정상 상태 오브젝트
    public GameObject collapsedObject;    // 무너진 상태 오브젝트
    public CameraShake cameraShake;       // 카메라 흔들림 참조
    private bool hasCollapsed = false;    // 이미 무너졌는지 여부

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasCollapsed)
        {
            hasCollapsed = true; // 한 번만 발동되도록 설정
            StartCoroutine(CollapseSequence());
        }
    }

    private System.Collections.IEnumerator CollapseSequence()
    {
        // 카메라 흔들림 (강도: 기존의 2배)
        yield return StartCoroutine(cameraShake.Shake(0.5f, 0.4f));

        // 정상 오브젝트 끄고 무너진 상태 켜기
        normalObject.SetActive(false);
        collapsedObject.SetActive(true);
    }
}
