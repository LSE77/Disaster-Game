using UnityEngine;
using TMPro;

public class DoorController : MonoBehaviour
{
    public TextMeshProUGUI doorMessage;  // 문 근처에서 뜨는 문구
    public GameObject closedDoor;        // 닫힌 문 오브젝트
    public GameObject openedDoor;        // 열린 문 오브젝트
    public float interactDistance = 5f;  // 문 상호작용 거리

    private bool isPlayerNearby = false;
    private bool doorOpened = false;

    void Start()
    {
        if (doorMessage != null)
            doorMessage.gameObject.SetActive(false);

        if (openedDoor != null)
            openedDoor.SetActive(false);  // 시작할 땐 열린 문 비활성화
    }

    void Update()
    {
        if (isPlayerNearby && !doorOpened)
        {
            if (KeyPickup.hasKey)  // 열쇠가 있을 때
            {
                if (Input.GetKeyDown(KeyCode.R))  // R 키로 문 열기
                {
                    OpenDoor();
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !doorOpened)
        {
            isPlayerNearby = true;

            if (doorMessage != null)
            {
                if (KeyPickup.hasKey)
                    doorMessage.text = "Press the R key to open the door.";
                else
                    doorMessage.text = "You need a key to open the door. The area near the car is suspicious.";

                doorMessage.gameObject.SetActive(true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            if (doorMessage != null)
                doorMessage.gameObject.SetActive(false);
        }
    }

    void OpenDoor()
    {
        doorOpened = true;

        if (doorMessage != null)
            doorMessage.gameObject.SetActive(false);

        if (closedDoor != null)
            closedDoor.SetActive(false);
        if (openedDoor != null)
            openedDoor.SetActive(true);

        Debug.Log("문이 열렸습니다!");
    }
}
