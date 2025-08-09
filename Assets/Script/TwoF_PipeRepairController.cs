using UnityEngine;

public class SimplePipeRepair : MonoBehaviour
{
    [Header("파이프 오브젝트")]
    public GameObject brokenPipe;
    public GameObject fixedPipe;
    public GameObject wallObject;

    [Header("UI 패널")]
    public GameObject repairPanel;
    public GameObject notPanel;

    [Header("상호작용 설정")]
    public float interactDistance = 5f;
    public LayerMask pipeLayer;                  // 비워두면 전체
    public MonoBehaviour playerControllerScript; // 움직임 스크립트
    public Camera cam;

    [Header("필요 망치 설정")]
    public GameObject[] hammers;                 // ✅ 망치 전부 드래그
    [Min(1)] public int requiredCount = 3;       // ✅ 몇 개 모으면 수리 가능?

    [Header("태그")]
    public string playerTag = "Player";
    public string interactableTag = "Interactable"; // 파이프 콜라이더(또는 부모)에 붙이기

    [Header("카운트 통합 옵션")]
    public bool alsoUseGlobalCounter = true;     // ✅ puthammer.hammerCount도 인정할지
    public int globalRequiredCountOverride = 0;  // 0이면 requiredCount와 동일

    // 내부 상태
    private bool[] collected; // 각 망치 수집 여부(이 스크립트 기준)
    private int collectedCount = 0;
    private bool isRepaired = false;

    void Start()
    {
        if (!cam) cam = Camera.main;

        if (brokenPipe) brokenPipe.SetActive(true);
        if (fixedPipe)  fixedPipe.SetActive(false);
        if (wallObject) wallObject.SetActive(true);
        if (repairPanel) repairPanel.SetActive(false);
        if (notPanel) notPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 망치 세팅
        if (hammers == null) hammers = new GameObject[0];
        collected = new bool[hammers.Length];
        collectedCount = 0;

        for (int i = 0; i < hammers.Length; i++)
        {
            SetupHammerDeep(hammers[i], i);
            collected[i] = false;
        }

        Debug.Log($"[Mgr] {name} ready. hammers={hammers.Length}, required={requiredCount}, useGlobal={alsoUseGlobalCounter}");
    }

    // ⭐ 자식까지 모든 콜라이더에 설치
    private void SetupHammerDeep(GameObject root, int index)
    {
        if (!root) return;

        var colliders = root.GetComponentsInChildren<Collider>(true);
        if (colliders == null || colliders.Length == 0)
        {
            var c = root.AddComponent<SphereCollider>();
            c.isTrigger = true;
            AttachPickupAndRB(root, index);
            Debug.Log($"[SetupHammer] {root.name} (no colliders) -> added SphereCollider & pickup idx={index}");
            return;
        }

        foreach (var c in colliders)
        {
            c.isTrigger = true;
            AttachPickupAndRB(c.gameObject, index);
            Debug.Log($"[SetupHammer] Attached to {c.gameObject.name} (idx={index})");
        }
    }

    private void AttachPickupAndRB(GameObject go, int index)
    {
        var hp = go.GetComponent<PipeHammerPickup>();
        if (!hp) hp = go.AddComponent<PipeHammerPickup>();
        hp.manager = this;
        hp.playerTag = playerTag;
        hp.index = index;

        var rb = go.GetComponent<Rigidbody>();
        if (!rb) rb = go.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        var col = go.GetComponent<Collider>();
        if (!col) col = go.AddComponent<SphereCollider>();
        col.isTrigger = true;
    }

    void Update()
    {
        if (isRepaired) return;

        // 패널 꺼져 있을 때만 클릭 체크
        if (AllPanelsClosed())
        {
            if (Input.GetMouseButtonDown(0))
                TryOpenPanelByClick();
        }
        else
        {
            // R 또는 ESC로 닫기/수리
            if (repairPanel && repairPanel.activeSelf && Input.GetKeyDown(KeyCode.R))
                DoRepair();
            else if (notPanel && notPanel.activeSelf && Input.GetKeyDown(KeyCode.R))
                ClosePanelsOnly();
            else if (Input.GetKeyDown(KeyCode.Escape))
                ClosePanelsOnly();
        }
    }

    private bool AllPanelsClosed()
    {
        return (repairPanel == null || !repairPanel.activeSelf) &&
               (notPanel == null || !notPanel.activeSelf);
    }

    // ⭐ 로컬 OR 글로벌 카운트로 충족 여부 판단
    private bool HasEnoughHammers()
    {
        int need = (globalRequiredCountOverride > 0) ? globalRequiredCountOverride : requiredCount;
        bool localOK = collectedCount >= need;
        bool globalOK = false;
        if (alsoUseGlobalCounter)
        {
            // puthammer가 프로젝트에 없는 경우를 대비한 try
            try { globalOK = puthammer.hammerCount >= need; } catch { /* 무시 */ }
        }
        return localOK || globalOK;
    }

    private bool HasTagOnSelfOrParents(Transform t, string tag)
    {
        if (string.IsNullOrEmpty(tag)) return true;
        for (var cur = t; cur != null; cur = cur.parent)
            if (cur.CompareTag(tag)) return true;
        return false;
    }

    private void TryOpenPanelByClick()
    {
        if (!cam) { Debug.LogWarning("Camera ref missing"); return; }

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        int mask = (pipeLayer.value == 0) ? ~0 : pipeLayer.value; // 비워두면 전체

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, mask))
        {
            // 태그(부모 포함) 검사
            if (!string.IsNullOrEmpty(interactableTag) &&
                !HasTagOnSelfOrParents(hit.collider.transform, interactableTag))
                return;

            // brokenPipe 트리(부모/자식) 내인지 확인
            if (brokenPipe)
            {
                var t = hit.collider.transform;
                bool hitBroken = (t == brokenPipe.transform) || t.IsChildOf(brokenPipe.transform) ||
                                 brokenPipe.transform.IsChildOf(t);
                if (!hitBroken) return;
            }

            Debug.Log($"[PipeClick] local={collectedCount}/{requiredCount} global={(alsoUseGlobalCounter ? TryGetGlobal() : -1)} hasEnough={HasEnoughHammers()}");

            if (HasEnoughHammers()) OpenRepairPanel();
            else OpenNotPanel();
        }
        else
        {
            Debug.Log("[PipeClick] Raycast missed.");
        }
    }

    private int TryGetGlobal()
    {
        try { return puthammer.hammerCount; } catch { return -1; }
    }

    private void OpenRepairPanel()
    {
        if (repairPanel) repairPanel.SetActive(true);
        if (notPanel) notPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (playerControllerScript) playerControllerScript.enabled = false;

        Debug.Log("[UI] repairPanel ON");
    }

    private void OpenNotPanel()
    {
        if (notPanel) notPanel.SetActive(true);
        if (repairPanel) repairPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (playerControllerScript) playerControllerScript.enabled = false;

        Debug.Log("[UI] notPanel ON");
    }

    private void ClosePanelsOnly()
    {
        if (repairPanel) repairPanel.SetActive(false);
        if (notPanel) notPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (playerControllerScript) playerControllerScript.enabled = true;

        Debug.Log("[UI] Panels OFF");
    }

    private void DoRepair()
    {
        if (brokenPipe) brokenPipe.SetActive(false);
        if (fixedPipe)  fixedPipe.SetActive(true);
        if (wallObject) wallObject.SetActive(false);

        isRepaired = true;
        ClosePanelsOnly();

        Debug.Log("[Pipe] Repaired!");
    }

    // ---- 망치 픽업 콜백 (인덱스로 식별) ----
    public void ReportHammerCollected(int index)
    {
        if (index < 0 || index >= collected.Length) return;
        if (collected[index]) return; // 이미 먹은 망치면 무시

        collected[index] = true;
        collectedCount++;
        Debug.Log($"[Pickup] idx={index} -> localCount={collectedCount}/{requiredCount}, global={TryGetGlobal()}");
    }
}

public class PipeHammerPickup : MonoBehaviour
{
    [HideInInspector] public SimplePipeRepair manager;
    [HideInInspector] public string playerTag = "Player";
    [HideInInspector] public int index; // 이 망치의 인덱스

    private bool IsPlayer(Collider c)
    {
        if (c.CompareTag(playerTag)) return true;
        if (c.attachedRigidbody && c.attachedRigidbody.CompareTag(playerTag)) return true;
        if (c.transform.root && c.transform.root.CompareTag(playerTag)) return true;

        Transform t = c.transform;
        while (t != null)
        {
            if (t.CompareTag(playerTag)) return true;
            t = t.parent;
        }
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!manager) return;
        if (!IsPlayer(other)) return;

        manager.ReportHammerCollected(index);
        gameObject.SetActive(false);
        Debug.Log($"[Pickup] {name} -> collected (idx={index}).");
    }
}
