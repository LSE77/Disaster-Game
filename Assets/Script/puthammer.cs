using UnityEngine;
using cakeslice;

public class puthammer : MonoBehaviour
{
    public GameObject[] hammerObjects;
    private Outline[] hammerOutlines;
    private bool[] hammerObtained;
    public static int hammerCount = 0; // ✅ 먹은 망치 개수

    void Start()
    {
        int count = hammerObjects.Length;
        hammerOutlines = new Outline[count];
        hammerObtained = new bool[count];
        hammerCount = 0;

        for (int i = 0; i < count; i++)
        {
            if (hammerObjects[i] != null)
            {
                hammerOutlines[i] = hammerObjects[i].GetComponent<Outline>();
                if (hammerOutlines[i] != null)
                    hammerOutlines[i].enabled = false;
            }
            hammerObtained[i] = false;
        }
    }

    void Update()
    {
        for (int i = 0; i < hammerObjects.Length; i++)
        {
            if (hammerObjects[i] == null || hammerObtained[i]) continue;

            bool highlight = false;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f))
            {
                if (hit.collider.gameObject == hammerObjects[i])
                {
                    highlight = true;
                    if (Input.GetMouseButtonDown(0))
                    {
                        PickUpHammer(i);
                    }
                }
            }
            if (hammerOutlines[i] != null)
                hammerOutlines[i].enabled = highlight;
        }
    }

    void PickUpHammer(int i)
    {
        hammerObtained[i] = true;
        if (hammerOutlines[i] != null) hammerOutlines[i].enabled = false;
        if (hammerObjects[i] != null) hammerObjects[i].SetActive(false);
        hammerCount++; // ✅ 먹을 때마다 +1
    }
}
