using UnityEngine;
using TMPro; // TextMeshPro 사용

public class PlayerInteraction : MonoBehaviour
{
    public float interactDistance = 5f;           // 상호작용 최대 거리
    public TMP_Text interactionText;              // TMP_Text로 변경!
    public float messageDuration = 2f;            // 텍스트 보여지는 시간(초)

    private float messageTimer = 0f;

    void Update()
    {
        // UI 텍스트 자동 숨김 처리
        if (interactionText != null && interactionText.gameObject.activeSelf)
        {
            messageTimer += Time.deltaTime;
            if (messageTimer > messageDuration)
            {
                interactionText.gameObject.SetActive(false);
                messageTimer = 0f;
            }
        }

        // 마우스 클릭 감지 (좌클릭: 0)
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactDistance))
            {
                Debug.Log("Raycast hit: " + hit.collider.name);

                if (hit.collider.CompareTag("Interactable"))
                {
                    Debug.Log("Interactable hit: " + hit.collider.name);
                    ShowInteractionMessage("You can repair the pipe.");
                }
            }
            else
            {
                Debug.Log("Raycast miss");
            }
        }
    }

    void ShowInteractionMessage(string msg)
    {
        if (interactionText != null)
        {
            interactionText.text = msg;
            interactionText.gameObject.SetActive(true);
            messageTimer = 0f;
        }
    }
}
