using UnityEngine;
using TMPro;

public class DoorController : MonoBehaviour
{
    public TextMeshProUGUI doorMessage;  // �� ��ó���� �ߴ� ����
    public GameObject closedDoor;        // ���� �� ������Ʈ
    public GameObject openedDoor;        // ���� �� ������Ʈ
    public float interactDistance = 5f;  // �� ��ȣ�ۿ� �Ÿ�

    private bool isPlayerNearby = false;
    private bool doorOpened = false;

    void Start()
    {
        if (doorMessage != null)
            doorMessage.gameObject.SetActive(false);

        if (openedDoor != null)
            openedDoor.SetActive(false);  // ������ �� ���� �� ��Ȱ��ȭ
    }

    void Update()
    {
        if (isPlayerNearby && !doorOpened)
        {
            if (KeyPickup.hasKey)  // ���谡 ���� ��
            {
                if (Input.GetKeyDown(KeyCode.R))  // R Ű�� �� ����
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

        Debug.Log("���� ���Ƚ��ϴ�!");
    }
}
