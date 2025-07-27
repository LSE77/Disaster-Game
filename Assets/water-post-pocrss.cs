using UnityEngine;
using UnityEngine.Rendering.PostProcessing; // Post Processing 네임스페이스 추가

public class WaterEffectController : MonoBehaviour
{
    public PostProcessVolume waterPostProcessVolume; // 인스펙터에서 Post Process Volume을 할당

    void OnTriggerEnter(Collider other)
    {
        // 플레이어 태그 확인 (플레이어 오브젝트에 "Player" 태그가 있어야 함)
        if (other.CompareTag("Player")) 
        {
            if (waterPostProcessVolume != null)
            {
                waterPostProcessVolume.enabled = true; // 볼륨 활성화
                Debug.Log("물 속에 들어왔습니다.");
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        // 플레이어 태그 확인
        if (other.CompareTag("Player"))
        {
            if (waterPostProcessVolume != null)
            {
                waterPostProcessVolume.enabled = false; // 볼륨 비활성화
                Debug.Log("물 밖으로 나갔습니다.");
            }
        }
    }
}