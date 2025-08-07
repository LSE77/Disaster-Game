using UnityEngine;
using cakeslice;

public class OutlineHighlighter : MonoBehaviour
{
    public float highlightDistance = 40f; // 40m로 설정

    private Outline lastOutline;

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            // 40m 이내에서만 윤곽선 표시
            if (hit.collider.CompareTag("Interactable") && hit.distance <= highlightDistance)
            {
                Outline outline = hit.collider.GetComponent<Outline>();
                if (outline != null)
                {
                    if (lastOutline != null && lastOutline != outline)
                        lastOutline.enabled = false;

                    outline.enabled = true;
                    lastOutline = outline;
                    return;
                }
            }
        }

        if (lastOutline != null)
        {
            lastOutline.enabled = false;
            lastOutline = null;
        }
    }
}
