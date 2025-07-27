using UnityEngine;
using TMPro; // TextMeshPro를 사용하려면 이 네임스페이스가 필요합니다.

public class TriggerEffectManager : MonoBehaviour
{
    [Header("감지할 트리거 존 태그")]
    public string targetTriggerTag = "clear"; // ★ 플레이어가 닿으면 효과를 발생시킬 임의의 안 보이는 큐브의 태그

    [Header("화면 오버레이 설정")]
    public GameObject screenOverlayCube; // 인스펙터에서 검은색 큐브 오브젝트를 할당
    public TextMeshProUGUI textMeshPro; // ★ UI용 TextMeshProUGUI (3D TextMeshPro Text와 다름)
    public string textOnEnter = "특정 구역에 진입했습니다!"; // 진입 시 띄울 텍스트

    void Start()
    {
        // 초기에는 화면 오버레이와 텍스트를 비활성화합니다.
        if (screenOverlayCube != null)
        {
            screenOverlayCube.SetActive(false);
        }
        if (textMeshPro != null)
        {
            textMeshPro.gameObject.SetActive(false); 
        }
    }

    // 트리거에 다른 콜라이더가 진입했을 때 호출됩니다.
    void OnTriggerEnter(Collider other)
    {
        // 진입한 오브젝트의 태그가 'targetTriggerTag'와 일치하는지 확인합니다.
        if (other.CompareTag(targetTriggerTag)) // ★ 여기를 수정했습니다.
        {
            // 화면 오버레이 큐브와 텍스트를 활성화합니다.
            if (screenOverlayCube != null)
            {
                screenOverlayCube.SetActive(true);
            }
            if (textMeshPro != null)
            {
                textMeshPro.gameObject.SetActive(true);
                textMeshPro.text = textOnEnter; // 텍스트를 설정합니다.
            }
            Debug.Log($"'{targetTriggerTag}' 영역에 진입했습니다.");
        }
    }

    // 트리거에서 다른 콜라이더가 나갔을 때 호출됩니다.
    void OnTriggerExit(Collider other)
    {
        // 나간 오브젝트의 태그가 'targetTriggerTag'와 일치하는지 확인합니다.
        if (other.CompareTag(targetTriggerTag)) // ★ 여기를 수정했습니다.
        {
            // 화면 오버레이 큐브와 텍스트를 비활성화합니다.
            if (screenOverlayCube != null)
            {
                screenOverlayCube.SetActive(false);
            }
            if (textMeshPro != null)
            {
                textMeshPro.gameObject.SetActive(false);
            }
            Debug.Log($"'{targetTriggerTag}' 영역에서 나갔습니다.");
        }
    }
}