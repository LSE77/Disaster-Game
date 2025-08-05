using UnityEngine;
using UnityEngine.UI;

public class KeyPickup : MonoBehaviour
{
    public Image keyIconUI; // UI 열쇠 아이콘
    private bool isCollected = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isCollected)
        {
            isCollected = true;
            keyIconUI.gameObject.SetActive(true); // 열쇠 UI 표시
            Destroy(gameObject); // 열쇠 오브젝트 제거
        }
    }
}
