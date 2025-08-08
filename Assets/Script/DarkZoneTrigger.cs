using UnityEngine;
using UnityEngine.UI;

public class DarkZoneTrigger : MonoBehaviour
{
    public Image darkOverlay;  // UI 이미지 연결
    public float fadeSpeed = 2f;

    private float targetAlpha = 0f;

    void Update()
    {
        if (darkOverlay != null)
        {
            Color color = darkOverlay.color;
            color.a = Mathf.Lerp(color.a, targetAlpha, Time.deltaTime * fadeSpeed);
            darkOverlay.color = color;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            targetAlpha = 1f;  // 어두워지는 정도
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            targetAlpha = 0f;    // 다시 밝게
        }
    }
}
