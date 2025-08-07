using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HammerPickup : MonoBehaviour
{
    public Image hammerIconUI;         // 망치 아이콘 이미지 (UI에서 할당)
    public TMP_Text hammerCountText;   // 망치 개수 텍스트 (UI에서 할당)

    void Update()
    {
        int count = puthammer.hammerCount;

        // 망치가 하나라도 있으면 UI 표시
        if (count > 0)
        {
            if (hammerIconUI != null)
                hammerIconUI.gameObject.SetActive(true);
            if (hammerCountText != null)
            {
                hammerCountText.gameObject.SetActive(true);
                hammerCountText.text = count.ToString();  // 숫자만 표시
            }
        }
        else
        {
            // 망치가 0개면 UI 숨김
            if (hammerIconUI != null)
                hammerIconUI.gameObject.SetActive(false);
            if (hammerCountText != null)
                hammerCountText.gameObject.SetActive(false);
        }
    }
}
