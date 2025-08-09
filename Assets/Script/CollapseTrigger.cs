using UnityEngine;
using System.Collections;

public class CollapseTrigger : MonoBehaviour
{
    [Header("오브젝트 전환")]
    public GameObject normalObject;          // 정상 상태
    public GameObject collapsedObject;       // 무너진 상태

    [Header("효과 지속 시간")]
    public float effectDuration = 3f;        // 흔들림/소리 지속 시간

    [Header("카메라 흔들림")]
    public CameraShake cameraShake;          // CameraShake.Shake(duration, magnitude)
    public float shakeMagnitude = 0.4f;

    [Header("사운드")]
    public AudioClip collapseClip;           // 붕괴 소리
    [Range(0f, 1f)] public float volume = 1f;
    public bool playAs2D = true;             // 2D(전역) / 3D(공간감)

    private bool hasCollapsed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasCollapsed)
        {
            hasCollapsed = true;
            StartCoroutine(CollapseSequence());
        }
    }

    private IEnumerator CollapseSequence()
    {
        // 소리 재생
        if (collapseClip != null)
            StartCoroutine(PlayClipForSeconds(collapseClip, effectDuration, transform.position));

        // 흔들림
        if (cameraShake != null)
            StartCoroutine(cameraShake.Shake(effectDuration, shakeMagnitude));

        // 지속 시간 대기
        yield return new WaitForSeconds(effectDuration);

        // 오브젝트 전환
        if (normalObject) normalObject.SetActive(false);
        if (collapsedObject) collapsedObject.SetActive(true);
    }

    private IEnumerator PlayClipForSeconds(AudioClip clip, float seconds, Vector3 worldPos)
    {
        GameObject go = new GameObject("CollapseAudio");
        var src = go.AddComponent<AudioSource>();
        src.clip = clip;
        src.volume = volume;
        src.loop = false;

        if (playAs2D)
        {
            src.spatialBlend = 0f; // 2D
        }
        else
        {
            src.spatialBlend = 1f; // 3D
            src.minDistance = 5f;
            src.maxDistance = 50f;
            go.transform.position = worldPos;
        }

        src.Play();
        yield return new WaitForSeconds(seconds);
        if (src.isPlaying) src.Stop();
        Destroy(go);
    }
}
