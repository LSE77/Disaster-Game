using UnityEngine;
using System.Collections.Generic;

public class ElectricEffectController : MonoBehaviour
{
    [Header("메인 노브 ON 오브젝트(활성 여부로 판단)")]
    public GameObject mainKnobOnObject;

    [Header("번개 이펙트 태그")]
    public string effectTag = "effect";
    private GameObject[] electricEffects;

    [Header("투명 벽 태그")]
    public string invisibleWallTag = "invisible";
    private GameObject[] invisibleWalls;

    [Header("전기 사운드")]
    public AudioClip electricClip;

    [Header("플레이어 & 거리 설정")]
    public Transform player;
    public float maxDistance = 8f;
    public float minDistance = 1.5f;
    public float volumeAtMin = 1.0f;
    public float volumeAtMax = 0.0f;
    public float volumeSmooth = 8f;

    private AudioSource src;

    void Start()
    {
        if (player == null && Camera.main != null)
            player = Camera.main.transform;

        electricEffects = GameObject.FindGameObjectsWithTag(effectTag);
        invisibleWalls = GameObject.FindGameObjectsWithTag(invisibleWallTag);

        src = gameObject.AddComponent<AudioSource>();
        src.playOnAwake = false;
        src.loop = true;
        src.clip = electricClip;
        src.spatialBlend = 1f;
        src.rolloffMode = AudioRolloffMode.Linear;
        src.minDistance = Mathf.Max(0.1f, minDistance);
        src.maxDistance = Mathf.Max(src.minDistance + 0.1f, maxDistance);
        src.volume = 0f;

        SetActiveObjects(electricEffects, false);
        SetActiveObjects(invisibleWalls, false);
    }

    void Update()
    {
        if (player == null) return;

        bool knobOn = (mainKnobOnObject != null && mainKnobOnObject.activeInHierarchy);

        if (!knobOn)
        {
            FadeOutAndStop();
            SetActiveObjects(electricEffects, false);
            SetActiveObjects(invisibleWalls, false);
            return;
        }

        float dist = Vector3.Distance(player.position, transform.position);
        bool inRange = dist <= maxDistance;

        SetActiveObjects(electricEffects, inRange);
        SetActiveObjects(invisibleWalls, inRange);

        src.transform.position = transform.position;

        if (inRange && src.clip != null)
        {
            if (!src.isPlaying) src.Play();

            float t = Mathf.InverseLerp(maxDistance, minDistance, dist);
            float targetVol = Mathf.Lerp(volumeAtMax, volumeAtMin, t);

            src.volume = Mathf.MoveTowards(src.volume, targetVol, Time.deltaTime * volumeSmooth);
        }
        else
        {
            FadeOutAndStop();
        }
    }

    private void SetActiveObjects(GameObject[] objects, bool on)
    {
        if (objects == null) return;
        foreach (var obj in objects)
            if (obj != null && obj.activeSelf != on)
                obj.SetActive(on);
    }

    private void FadeOutAndStop()
    {
        if (src.isPlaying)
        {
            src.volume = Mathf.MoveTowards(src.volume, 0f, Time.deltaTime * volumeSmooth);
            if (Mathf.Approximately(src.volume, 0f))
                src.Stop();
        }
    }
}
