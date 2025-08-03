using UnityEngine;

public class watermoving : MonoBehaviour
{
    public float targetYScale = 14.0893f; 
    public float riseDuration = 300f;    

    private float initialYScale;         
    private float startTime;             
    private bool isRising = true;        

    void Start()
    {
        initialYScale = transform.localScale.y;
        startTime = Time.time;
    }

    void Update()
    {
        if (!isRising)
        {
            return;
        }

        float elapsedTime = Time.time - startTime;
        float t = Mathf.Clamp01(elapsedTime / riseDuration);
        float currentYScale = Mathf.Lerp(initialYScale, targetYScale, t);

        Vector3 newScale = transform.localScale;
        newScale.y = currentYScale;
        transform.localScale = newScale;

        if (t >= 1f)
        {
            isRising = false;
            Debug.Log("물이 모두 차올랐습니다!");
            // 물이 다 차오른 후 실행할 코드 추가
        }
    }
}