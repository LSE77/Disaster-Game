using UnityEngine;
using UnityEngine.UI;

public class KeyPickup : MonoBehaviour
{
    public Image keyIconUI; // UI ���� ������
    private bool isCollected = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isCollected)
        {
            isCollected = true;
            keyIconUI.gameObject.SetActive(true); // ���� UI ǥ��
            Destroy(gameObject); // ���� ������Ʈ ����
        }
    }
}
