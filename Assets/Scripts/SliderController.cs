using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviourPun, IPunObservable
{
    public Slider slider; // 연결된 Slider
    public RandomSlider randomSlider; // RandomSlider 연결
    public float duration = 30f; // Slider가 채워지는 데 걸리는 시간
    private float elapsedTime = 0f; // 경과 시간
    private bool isIncreasing = false; // Slider가 올라가는 상태
    private bool isDecreasing = false; // Slider가 내려가는 상태
    private bool isCompleted = false; // Slider가 완료된 상태
    public delegate void SliderCompleted(); // Slider 완료 이벤트
    public event SliderCompleted OnSliderCompleted;

    private CanvasGroup canvasGroup; // CanvasGroup을 사용해 숨김 처리
    private float randomSliderTimer = 0f; // RandomSlider 표시 타이머
    private float randomSliderDelay; // RandomSlider 활성화 대기 시간

    void Start()
{
    if (slider == null)
    {
        Debug.LogError("Slider가 연결되지 않았습니다.");
        return;
    }

    slider.value = 0f; // 초기값 설정
    slider.maxValue = 100f; // 최대값 설정
    isIncreasing = false; // 시작 시 증가 상태 비활성화
    isCompleted = false; // 시작 시 완료 상태 초기화

    // CanvasGroup 추가 또는 참조
    canvasGroup = slider.GetComponent<CanvasGroup>();
    if (canvasGroup == null)
    {
        canvasGroup = slider.gameObject.AddComponent<CanvasGroup>();
    }

    HideSlider(); // 시작 시 Slider 숨김

    if (randomSlider != null)
    {
        randomSlider.DeactivateRandomSlider(); // RandomSlider 초기화
    }
}



    void Update()
    {
        if (isIncreasing)
        {
            if (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                slider.value = Mathf.Clamp((elapsedTime / duration) * 100f, 0, 100f); // Value를 0~100 범위로 조정
                ShowSlider(); // 증가 중에는 Slider 표시

                HandleRandomSliderActivation();
            }
            else
            {
                CompleteSlider();
            }
        }
        else if (isDecreasing)
        {
            if (slider.value > 0)
            {
                elapsedTime = Mathf.Max(0, elapsedTime - Time.deltaTime); // 감소
                slider.value = Mathf.Clamp((elapsedTime / duration) * 100f, 0, 100f);
                HideSlider(); // 감소 중에는 Slider 숨김
            }
            else
            {
                StopDecreasing(); // 최소값에 도달하면 감소 정지
            }
        }
    }

    
public void StartIncreasing()
{
    if (isCompleted || isIncreasing) return; // 중복 실행 방지

    isIncreasing = true;
    isDecreasing = false;
    elapsedTime = (slider.value / 100f) * duration; // 현재 value에 기반한 경과 시간 계산
    ResetRandomSliderTimer();
}

    public void StartDecreasing()
    {
        if (isCompleted) return;

        isIncreasing = false;
        isDecreasing = true;
        HideSlider(); // 감소 시작 시 Slider 숨김
    }

    public void StopDecreasing()
    {
        isDecreasing = false;
        HideSlider(); // 감소가 끝난 후에도 Slider 숨김 유지
    }

    /// <summary>
    /// 진행 시간을 감소시킵니다.
    /// </summary>
    /// <param name="seconds">감소할 시간 (초)</param>
    public void DecreaseProgressBySeconds(float seconds)
    {
        if (isCompleted)
        {
            Debug.LogWarning("이미 완료된 Slider입니다.");
            return;
        }

        elapsedTime = Mathf.Max(0, elapsedTime - seconds); // 0 이하로 감소하지 않도록 설정
        slider.value = Mathf.Clamp((elapsedTime / duration) * 100f, 0, 100f); // Value 갱신
        Debug.Log($"{gameObject.name} 진행 시간 감소: -{seconds}초");
    }

    private void CompleteSlider()
    {
        isIncreasing = false;
        isCompleted = true; // 완료 상태로 설정
        HideSlider(); // 완료 시 Slider 숨김

        OnSliderCompleted?.Invoke(); // 완료 이벤트 호출
        Debug.Log($"{gameObject.name} slider completed!");
    }

    private void ShowSlider()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1; // Slider 표시
            canvasGroup.blocksRaycasts = true; // 상호작용 허용
        }
    }

    private void HideSlider()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0; // Slider 숨김
            canvasGroup.blocksRaycasts = false; // 상호작용 차단
        }
    }

    public bool IsCompleted()
    {
        return isCompleted; // isCompleted 변수 값 반환
    }

    public bool IsIncreasing()
    {
        return isIncreasing; // 현재 증가 상태 반환
    }

    public bool IsDecreasing()
    {
        return isDecreasing; // 현재 감소 상태 반환
    }

    private void ResetRandomSliderTimer()
    {
        randomSliderDelay = Random.Range(6f, 12f); // 6~12초 랜덤 시간
        randomSliderTimer = 0f;
    }

    private void HandleRandomSliderActivation()
    {
        if (randomSlider == null || !isIncreasing) return;

        randomSliderTimer += Time.deltaTime;

        if (randomSliderTimer >= randomSliderDelay)
        {
            randomSlider.ActivateRandomSlider(); // RandomSlider 활성화
            ResetRandomSliderTimer(); // 다음 RandomSlider 대기 시간 초기화
        }
    }

    public void PauseIncreasing()
    {
        if (isIncreasing)
        {
            isIncreasing = false; // 증가 상태 일시 정지
            Debug.Log($"{gameObject.name} 증가 일시 정지");
        }
    }

    public void ResumeIncreasing()
    {
        if (!isIncreasing && !isCompleted)
        {
            isIncreasing = true; // 증가 상태 재개
            Debug.Log($"{gameObject.name} 증가 재개");
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) // 로컬 플레이어가 데이터를 전송 중인 경우
        {
            stream.SendNext(slider.value); // 슬라이더 값을 네트워크에 전송
            stream.SendNext(isCompleted); // 완료 상태도 동기화
        }
        else if (stream.IsReading) // 네트워크 데이터를 수신 중인 경우
        {
            slider.value = (float)stream.ReceiveNext(); // 슬라이더 값 수신
            isCompleted = (bool)stream.ReceiveNext(); // 완료 상태 수신
        }
    }
}
