using UnityEngine;
using System.Collections.Generic;

public class ElectricEffectController : MonoBehaviour
{
    [Header("메인 노브 ON 오브젝트(활성 여부로 판단)")]
    public GameObject mainKnobOnObject;   // 이 오브젝트가 active일 때만 동작

    [Header("번개 이펙트 수집")]
    public Transform effectsRoot;          // 번개들의 부모(없으면 태그로 전체 검색)
    public string effectTag = "effect";    // 번개 오브젝트에 달아둔 태그
    public GameObject[] electricEffects;   // 수동 지정(비워두면 자동 수집)

    [Header("전기 사운드 (AudioClip만 필요)")]
    public AudioClip electricClip;         // AudioSource는 자동 생성됨

    [Header("플레이어 & 거리 설정")]
    public Transform player;               // 비우면 Camera.main 사용
    public float maxDistance = 8f;         // 이 거리 밖: 이펙트 Off + 사운드 페이드아웃
    public float minDistance = 1.5f;       // 이 거리 이내: 최대 볼륨
    public float volumeAtMin = 1.0f;       // 근거리 볼륨
    public float volumeAtMax = 0.0f;       // 원거리 볼륨
    public float volumeSmooth = 8f;        // 볼륨 변화 속도

    private AudioSource src;

    void Start()
    {
        // 플레이어 자동 할당
        if (player == null && Camera.main != null) player = Camera.main.transform;

        // 이펙트 자동 수집(비활성 포함)
        if (electricEffects == null || electricEffects.Length == 0)
        {
            if (effectsRoot != null)
            {
                var list = new List<GameObject>();
                var children = effectsRoot.GetComponentsInChildren<Transform>(true);
                foreach (var t in children)
                    if (t != effectsRoot && t.CompareTag(effectTag)) list.Add(t.gameObject);
                electricEffects = list.ToArray();
            }
            else
            {
                electricEffects = GameObject.FindGameObjectsWithTag(effectTag);
            }
        }

        // 오디오소스 동적 생성
        src = gameObject.AddComponent<AudioSource>();
        src.playOnAwake = false;
        src.loop = true;
        src.clip = electricClip;
        src.spatialBlend = 1f;                     // 3D
        src.rolloffMode = AudioRolloffMode.Linear; // 선형 감쇠
        src.minDistance = Mathf.Max(0.1f, minDistance);
        src.maxDistance = Mathf.Max(src.minDistance + 0.1f, maxDistance);
        src.volume = 0f;

        SetEffectsActive(false);
    }

    void Update()
    {
        if (player == null) return;

        // 메인 노브 ON 오브젝트가 꺼지면 즉시 정리
        bool knobOn = (mainKnobOnObject != null && mainKnobOnObject.activeInHierarchy);
        if (!knobOn)
        {
            FadeOutAndStop();
            SetEffectsActive(false);
            return;
        }

        // 거리 체크
        float dist = Vector3.Distance(player.position, transform.position);
        bool inRange = dist <= maxDistance;

        // 이펙트 on/off
        SetEffectsActive(inRange);

        // 사운드 위치 추적
        src.transform.position = transform.position;

        // 볼륨 처리
        if (inRange && src.clip != null)
        {
            if (!src.isPlaying) src.Play();

            // dist: maxDistance→minDistance로 갈수록 0→1
            float t = Mathf.InverseLerp(maxDistance, minDistance, dist);
            float targetVol = Mathf.Lerp(volumeAtMax, volumeAtMin, t);

            src.volume = Mathf.MoveTowards(src.volume, targetVol, Time.deltaTime * volumeSmooth);
        }
        else
        {
            FadeOutAndStop();
        }
    }

    private void SetEffectsActive(bool on)
    {
        if (electricEffects == null) return;
        for (int i = 0; i < electricEffects.Length; i++)
        {
            var obj = electricEffects[i];
            if (obj == null) continue;
            if (obj.activeSelf != on) obj.SetActive(on);
        }
    }

    private void FadeOutAndStop()
    {
        if (src.isPlaying)
        {
            src.volume = Mathf.MoveTowards(src.volume, 0f, Time.deltaTime * volumeSmooth);
            if (Mathf.Approximately(src.volume, 0f)) src.Stop();
        }
    }
}
