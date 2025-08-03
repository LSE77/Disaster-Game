using UnityEngine;

public class CollapseTrigger : MonoBehaviour
{
    public GameObject normalObject;       // ���� ���� ������Ʈ
    public GameObject collapsedObject;    // ������ ���� ������Ʈ
    public CameraShake cameraShake;       // ī�޶� ��鸲 ����
    private bool hasCollapsed = false;    // �̹� ���������� ����

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasCollapsed)
        {
            hasCollapsed = true; // �� ���� �ߵ��ǵ��� ����
            StartCoroutine(CollapseSequence());
        }
    }

    private System.Collections.IEnumerator CollapseSequence()
    {
        // ī�޶� ��鸲 (����: ������ 2��)
        yield return StartCoroutine(cameraShake.Shake(0.5f, 0.4f));

        // ���� ������Ʈ ���� ������ ���� �ѱ�
        normalObject.SetActive(false);
        collapsedObject.SetActive(true);
    }
}
