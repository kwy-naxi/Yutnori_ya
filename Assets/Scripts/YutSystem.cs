using UnityEngine;
using System.Collections;

public class YutSystem : MonoBehaviour
{
    public static YutSystem Instance { get; private set; }

    [SerializeField] private GameObject yutStickPrefab;
    [SerializeField] private Transform throwPoint;
    [SerializeField] private float throwForce = 5f;
    [SerializeField] private float spinForce = 10f;

    private GameObject[] yutSticks;
    private bool isThrowing = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        InitializeYutSticks();
    }

    private void InitializeYutSticks()
    {
        yutSticks = new GameObject[4];
        for (int i = 0; i < 4; i++)
        {
            yutSticks[i] = Instantiate(yutStickPrefab, throwPoint.position, Quaternion.identity);
            yutSticks[i].transform.parent = transform;
            yutSticks[i].SetActive(false);
        }
    }

    public void ThrowYut()
    {
        if (isThrowing) return;
        StartCoroutine(ThrowYutCoroutine());
    }

    private IEnumerator ThrowYutCoroutine()
    {
        isThrowing = true;
        int frontCount = 0;

        // 모든 윷을 던지기
        foreach (var stick in yutSticks)
        {
            stick.SetActive(true);
            stick.transform.position = throwPoint.position;
            stick.transform.rotation = Random.rotation;

            Rigidbody rb = stick.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.AddForce(Vector3.up * throwForce, ForceMode.Impulse);
                rb.AddTorque(Random.onUnitSphere * spinForce, ForceMode.Impulse);
            }

            yield return new WaitForSeconds(0.2f);
        }

        // 윷이 안정화될 때까지 대기
        yield return new WaitForSeconds(2f);

        // 윷의 결과 확인
        foreach (var stick in yutSticks)
        {
            if (IsFront(stick))
            {
                frontCount++;
            }
        }

        // 결과에 따른 이동 횟수 계산
        int moveCount = CalculateMoveCount(frontCount);
        Debug.Log($"윷 결과: {GetYutName(moveCount)}");

        // GameManager에 결과 전달
        GameManager.Instance.OnYutThrown(moveCount);

        // 윷 정리
        foreach (var stick in yutSticks)
        {
            stick.SetActive(false);
        }

        isThrowing = false;
    }

    private bool IsFront(GameObject stick)
    {
        // 윷의 앞면 판정 (예: 윷의 위쪽이 위를 향하는지 확인)
        return Vector3.Dot(stick.transform.up, Vector3.up) > 0.5f;
    }

    private int CalculateMoveCount(int frontCount)
    {
        switch (frontCount)
        {
            case 0: return 5;  // 모
            case 1: return 1;  // 도
            case 2: return 2;  // 개
            case 3: return 3;  // 걸
            case 4: return 4;  // 윷
            default: return 0;
        }
    }

    private string GetYutName(int moveCount)
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
} 