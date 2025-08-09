using UnityEngine;
using System.Collections;

public class CollapseTrigger : MonoBehaviour
{
    [Header("������Ʈ ��ȯ")]
    public GameObject normalObject;          // ���� ����
    public GameObject collapsedObject;       // ������ ����

    [Header("ȿ�� ���� �ð�")]
    public float effectDuration = 3f;        // ��鸲/�Ҹ� ���� �ð�

    [Header("ī�޶� ��鸲")]
    public CameraShake cameraShake;          // CameraShake.Shake(duration, magnitude)
    public float shakeMagnitude = 0.4f;

    [Header("����")]
    public AudioClip collapseClip;           // �ر� �Ҹ�
    [Range(0f, 1f)] public float volume = 1f;
    public bool playAs2D = true;             // 2D(����) / 3D(������)

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
        // �Ҹ� ���
        if (collapseClip != null)
            StartCoroutine(PlayClipForSeconds(collapseClip, effectDuration, transform.position));

        // ��鸲
        if (cameraShake != null)
            StartCoroutine(cameraShake.Shake(effectDuration, shakeMagnitude));

        // ���� �ð� ���
        yield return new WaitForSeconds(effectDuration);

        // ������Ʈ ��ȯ
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
