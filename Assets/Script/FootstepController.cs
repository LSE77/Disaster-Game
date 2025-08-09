using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class FootstepController : MonoBehaviour
{
    [Header("�߼Ҹ� ����")]
    public AudioClip footstepClip;       // WAV
    public float stepDistance = 1f;      // 1m����
    public float playDuration = 0.5f;    // 0.5�ʸ� ���
    public float minInterval = 0.2f;     // �ּ� ����(���� ����� ����)

    [Header("����ȭ �ɼ�")]
    public float ignoreDeltaBelow = 0.001f;

    private AudioSource audioSource;
    private Vector3 lastPosition;
    private float accumulatedDistance = 0f;
    private bool isPlayingStep = false;
    private float nextAllowedTime = 0f;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        // 3D�� ����: audioSource.spatialBlend = 1f;
    }

    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        // �̵��Ÿ� ����
        float d = Vector3.Distance(transform.position, lastPosition);
        lastPosition = transform.position;

        if (d <= ignoreDeltaBelow) return;

        accumulatedDistance += d;

        // ������ ������ ������, �ܿ��� ����� �߼Ҹ� �õ�
        while (accumulatedDistance >= stepDistance)
        {
            TryPlayFootstep();
            accumulatedDistance -= stepDistance;
        }
    }

    void TryPlayFootstep()
    {
        if (footstepClip == null) return;

        // ��� ���̰ų� �ּҰ��� �� �������� ��ŵ
        if (isPlayingStep || Time.time < nextAllowedTime) return;

        StartCoroutine(PlayFor(playDuration));
        nextAllowedTime = Time.time + minInterval;
    }

    IEnumerator PlayFor(float duration)
    {
        isPlayingStep = true;

        audioSource.Stop();
        audioSource.time = 0f;
        audioSource.clip = footstepClip;
        audioSource.Play();

        yield return new WaitForSeconds(duration);

        audioSource.Stop();
        isPlayingStep = false;
    }
}
