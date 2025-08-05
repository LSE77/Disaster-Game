using UnityEngine;
using TMPro;

public class DoorController : MonoBehaviour
{
    public TextMeshProUGUI doorMessage;  // "E를 눌러 열쇠를 사용하세요"
    public GameObject closedDoor;        // 닫힌 문 오브젝트
    public GameObject openedDoor;        // 열린 문 오브젝트 (초기 비활성화)
    public KeyPickup keyPickup;          // 열쇠 스크립트 참조

    private bool isPlayerNearby = false;
    private bool doorOpened = false;

    void Start()
    {
        doorMessage.gameObject.SetActive(false);
        if (openedDoor != null)
            openedDoor.SetActive(false); // 시작 시 열린 문은 비활성화
    }

    void Update()
    {
        if (isPlayerNearby && !doorOpened && keyPickup == null) // 열쇠를 가진 상태
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                OpenDoor();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !doorOpened)
        {
            isPlayerNearby = true;
            if (keyPickup == null) // 열쇠를 가지고 있을 때만 메시지 표시
                doorMessage.gameObject.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            doorMessage.gameObject.SetActive(false);
        }
    }

    void OpenDoor()
    {
        doorOpened = true;
        doorMessage.gameObject.SetActive(false);

        // 닫힌 문 비활성화 & 열린 문 활성화
        closedDoor.SetActive(false);
        openedDoor.SetActive(true);

        Debug.Log("문이 열렸습니다!");
    }
}
