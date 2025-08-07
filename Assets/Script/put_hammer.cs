using UnityEngine;
using UnityEngine.UI;
using TMPro;
using cakeslice; // 명확하게 cakeslice Outline 사용

public class puthammer : MonoBehaviour
{
    public GameObject[] hammerObjects; // 맵에 배치된 망치 오브젝트들
    public Image hammerIconUI;         // UI에 표시될 망치 아이콘
    public TMP_Text hammerCountText;   // UI에 표시될 망치 개수 텍스트

    private cakeslice.Outline[] hammerOutlines; // 망치 아웃라인 (cakeslice)
    private bool[] hammerObtained;              // 각 망치를 먹었는지 여부

    public static int hammerCount = 0; // 전체 망치 개수

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
                        PickUpHammer(i);
                    }
                }
            }

            // 아웃라인 표시 여부
            if (hammerOutlines[i] != null)
                hammerOutlines[i].enabled = highlight;
        }
    }

    void PickUpHammer(int i)
    {
        hammerObtained[i] = true;

        if (hammerOutlines[i] != null)
            hammerOutlines[i].enabled = false;

        if (hammerObjects[i] != null)
            hammerObjects[i].SetActive(false);

        hammerCount++;

        // UI 업데이트
        if (hammerIconUI != null) hammerIconUI.gameObject.SetActive(true);
        if (hammerCountText != null)
        {
            hammerCountText.gameObject.SetActive(true);
            hammerCountText.text = "" + hammerCount.ToString();
        }
    }
}
