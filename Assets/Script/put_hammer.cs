using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;        // ← 코루틴
using cakeslice;                 // 명확하게 cakeslice Outline 사용

public class puthammer : MonoBehaviour
{
    [Header("망치 세팅")]
    public GameObject[] hammerObjects; // 맵에 배치된 망치 오브젝트들 (콜라이더 필수)

    [Header("UI")]
    public Image hammerIconUI;         // UI에 표시될 망치 아이콘
    public TMP_Text hammerCountText;   // UI에 표시될 망치 개수 텍스트

    [Header("사운드")]
    public AudioClip hammerPickupClip; // 업로드한 wav 지정
    [Range(0f, 1f)] public float volume = 1f;
    public bool playAs2D = true;       // 2D(전체에 크게) / 3D(공간감)

    private cakeslice.Outline[] hammerOutlines; // 망치 아웃라인 (cakeslice)
    private bool[] hammerObtained;              // 각 망치를 먹었는지 여부

    public static int hammerCount = 0; // 전체 망치 개수

    // 코루틴 러너(씬 전역 1개 유지)
    private static CoroutineRunner _runner;

    void Awake()
    {
        // 전역 코루틴 러너 확보 (비활성화되어도 죽지 않음)
        if (_runner == null)
        {
            var go = new GameObject("[AudioCoroutineRunner]");
            DontDestroyOnLoad(go);
            _runner = go.AddComponent<CoroutineRunner>();
        }
    }

    void Start()
    {
        int count = hammerObjects.Length;
        hammerOutlines = new cakeslice.Outline[count];
        hammerObtained = new bool[count];
        hammerCount = 0;

        for (int i = 0; i < count; i++)
        {
            if (hammerObjects[i] != null)
            {
                hammerOutlines[i] = hammerObjects[i].GetComponent<cakeslice.Outline>();
                if (hammerOutlines[i] != null)
                    hammerOutlines[i].enabled = false;
            }
            hammerObtained[i] = false;
        }

        // UI 비활성화로 시작
        if (hammerIconUI != null) hammerIconUI.gameObject.SetActive(false);
        if (hammerCountText != null) hammerCountText.gameObject.SetActive(false);
    }

    void Update()
    {
        for (int i = 0; i < hammerObjects.Length; i++)
        {
            if (hammerObjects[i] == null || hammerObtained[i]) continue;

            bool highlight = false;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f))
            {
                if (hit.collider.gameObject == hammerObjects[i])
                {
                    highlight = true;

                    // 마우스 클릭 시 망치 줍기
                    if (Input.GetMouseButtonDown(0))
                    {
                        PickUpHammer(i, hit.point);
                    }
                }
            }

            // 아웃라인 표시 여부
            if (hammerOutlines[i] != null)
                hammerOutlines[i].enabled = highlight;
        }
    }

    void PickUpHammer(int i, Vector3 hitPoint)
    {
        hammerObtained[i] = true;

        if (hammerOutlines[i] != null)
            hammerOutlines[i].enabled = false;

        // === 소리 먼저 러너에게 맡기고, 오브젝트는 바로 숨김 ===
        if (hammerPickupClip != null && _runner != null)
        {
            // 3D는 망치 위치에서, 2D는 아무 위치여도 상관없음
            Vector3 audioPos = (hammerObjects[i] != null) ? hammerObjects[i].transform.position : hitPoint;
            _runner.StartCoroutine(PlayClipExactlyForSeconds(hammerPickupClip, 2f, audioPos));
        }

        // 망치 오브젝트는 즉시 비활성화 (러너가 따로 돌기 때문에 에러 없음)
        if (hammerObjects[i] != null)
            hammerObjects[i].SetActive(false);

        hammerCount++;

        // UI 업데이트
        if (hammerIconUI != null) hammerIconUI.gameObject.SetActive(true);
        if (hammerCountText != null)
        {
            hammerCountText.gameObject.SetActive(true);
            hammerCountText.text = hammerCount.ToString();
        }
    }

    // 3초만 재생 후 정리
    IEnumerator PlayClipExactlyForSeconds(AudioClip clip, float seconds, Vector3 worldPos)
    {
        // 재생용 임시 오디오 오브젝트
        GameObject go = new GameObject("PickupOneShotAudio");
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
            src.minDistance = 2f;
            src.maxDistance = 20f;
            go.transform.position = worldPos;
        }

        src.Play();
        yield return new WaitForSeconds(seconds);

        if (src.isPlaying) src.Stop();
        Destroy(go);
    }

    // 전역 코루틴 러너: 별도 게임오브젝트에 붙어서 항상 살아있음
    private class CoroutineRunner : MonoBehaviour { }
}
