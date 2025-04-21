using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class YutThrowSystem : MonoBehaviour
{
    public static YutThrowSystem Instance { get; private set; }

    [System.Serializable]
    public class YutStick
    {
        public GameObject stickObject;
        public float throwForce = 5f;
        public float rotationForce = 10f;
        public Vector3 initialPosition;
        public Quaternion initialRotation;
    }

    public YutStick[] yutSticks = new YutStick[4];
    public float throwDelay = 0.1f;
    public float resultDelay = 2f;
    public UnityEvent<int> onYutThrown;

    private bool isThrowInProgress = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 초기 위치와 회전값 저장
        for (int i = 0; i < yutSticks.Length; i++)
        {
            if (yutSticks[i].stickObject != null)
            {
                yutSticks[i].initialPosition = yutSticks[i].stickObject.transform.position;
                yutSticks[i].initialRotation = yutSticks[i].stickObject.transform.rotation;
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ThrowYut();
        }
    }

    public void ThrowYut()
    {
        if (!isThrowInProgress)
        {
            StartCoroutine(ThrowYutCoroutine());
        }
    }

    private IEnumerator ThrowYutCoroutine()
    {
        isThrowInProgress = true;
        Debug.Log("윷 던지기 시작");

        // 각 윷 막대기 던지기
        foreach (YutStick stick in yutSticks)
        {
            if (stick.stickObject != null)
            {
                Rigidbody rb = stick.stickObject.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = false;
                    
                    // 랜덤한 방향으로 던지기
                    Vector3 throwDirection = new Vector3(
                        Random.Range(-1f, 1f),
                        Random.Range(0.5f, 1f),
                        Random.Range(-1f, 1f)
                    ).normalized;

                    rb.AddForce(throwDirection * stick.throwForce, ForceMode.Impulse);
                    rb.AddTorque(Random.insideUnitSphere * stick.rotationForce, ForceMode.Impulse);
                }
            }
            yield return new WaitForSeconds(throwDelay);
        }

        // 결과 확인을 위한 대기
        yield return new WaitForSeconds(resultDelay);

        // 결과 계산
        int moveCount = CalculateResult();
        Debug.Log($"윷 결과: {GetYutResultName(moveCount)} ({moveCount}칸)");

        // 결과 이벤트 발생
        onYutThrown?.Invoke(moveCount);

        // 윷 초기화
        yield return new WaitForSeconds(1f);
        ResetYutSticks();
        isThrowInProgress = false;
    }

    private int CalculateResult()
    {
        int backCount = 0;
        foreach (YutStick stick in yutSticks)
        {
            if (stick.stickObject != null)
            {
                // 윷이 뒤집어졌는지 확인 (y축 기준)
                float angle = stick.stickObject.transform.eulerAngles.x;
                if (angle > 180f) angle -= 360f;
                
                if (Mathf.Abs(angle) > 90f)
                {
                    backCount++;
                }
            }
        }

        // 윷놀이 규칙에 따른 이동 칸 수 계산
        switch (backCount)
        {
            case 0: return 5; // 모
            case 1: return 1; // 도
            case 2: return 2; // 개
            case 3: return 3; // 걸
            case 4: return 4; // 윷
            default: return 0;
        }
    }

    private string GetYutResultName(int moveCount)
    {
        switch (moveCount)
        {
            case 1: return "도";
            case 2: return "개";
            case 3: return "걸";
            case 4: return "윷";
            case 5: return "모";
            default: return "알 수 없음";
        }
    }

    public void ResetYutSticks()
    {
        foreach (YutStick stick in yutSticks)
        {
            if (stick.stickObject != null)
            {
                Rigidbody rb = stick.stickObject.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = true;
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
                stick.stickObject.transform.position = stick.initialPosition;
                stick.stickObject.transform.rotation = stick.initialRotation;
            }
        }
    }
} 