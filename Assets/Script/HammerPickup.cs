using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HammerPickup : MonoBehaviour
{
    public Image hammerIconUI;         // ��ġ ������ �̹��� (UI���� �Ҵ�)
    public TMP_Text hammerCountText;   // ��ġ ���� �ؽ�Ʈ (UI���� �Ҵ�)

    void Update()
    {
        int count = puthammer.hammerCount;

        // ��ġ�� �ϳ��� ������ UI ǥ��
        if (count > 0)
        {
            if (hammerIconUI != null)
                hammerIconUI.gameObject.SetActive(true);
            if (hammerCountText != null)
            {
                hammerCountText.gameObject.SetActive(true);
                hammerCountText.text = count.ToString();  // ���ڸ� ǥ��
            }
        }
        else
        {
            // ��ġ�� 0���� UI ����
            if (hammerIconUI != null)
                hammerIconUI.gameObject.SetActive(false);
            if (hammerCountText != null)
                hammerCountText.gameObject.SetActive(false);
        }
    }
}
