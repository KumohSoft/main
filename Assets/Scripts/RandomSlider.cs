using UnityEngine;
using UnityEngine.UI;

public class RandomSlider : MonoBehaviour
{
    public Slider slider; // RandomSlider UI 연결
    public RectTransform imageRectTransform; // 범위 이미지의 RectTransform 연결
    public Text statusText; // 성공/실패를 표시할 텍스트
    public SliderController linkedSliderController; // 1대1로 연결된 SliderController

    private bool isRandomSliderActive = false; // RandomSlider 활성화 여부
    private int randomRangeStart; // 랜덤 범위 시작점 (0~100)
    public int rangeLength = 40; // 범위의 길이 (디폴트값: 40)
    private float sliderSpeed = 300f; // 슬라이더 이동 속도
    private int sliderDirection = 1; // 슬라이더 이동 방향 (1: 증가, -1: 감소)

    // posY 계산 상수
    private const float posYMin = -24f;
    private const float posYMax = 24f;

    void Start()
    {
        UpdateStatusText(); // 텍스트를 초기화
    }

    void Update()
    {
        if (isRandomSliderActive)
        {
            HandleRandomSliderProgress();

            if (Input.GetKeyDown(KeyCode.F))
            {
                CheckSuccessOrFailure();
            }
        }
    }

    public void ActivateRandomSlider()
    {
        isRandomSliderActive = true;

        if (linkedSliderController != null)
        {
            linkedSliderController.PauseIncreasing();
        }

        gameObject.SetActive(true); // RandomSlider 활성화
        InitializeSlider(); // 슬라이더 초기화
        SetNewRandomRange();
        UpdateStatusText(); // 텍스트 업데이트

        Debug.Log("RandomSlider 활성화");
    }

    public void DeactivateRandomSlider()
    {
        isRandomSliderActive = false;

        if (linkedSliderController != null)
        {
            linkedSliderController.ResumeIncreasing();
        }

        gameObject.SetActive(false); // RandomSlider 비활성화
        Debug.Log("RandomSlider 비활성화");
    }

    private void HandleRandomSliderProgress()
    {
        slider.value += Time.deltaTime * sliderSpeed * sliderDirection;

        if (slider.value >= 100)
        {
            slider.value = 100;
            sliderDirection = -1; // 감소 방향
        }
        else if (slider.value <= 0)
        {
            slider.value = 0;
            sliderDirection = 1; // 증가 방향
        }
    }

    private void CheckSuccessOrFailure()
    {
        bool isSuccess = slider.value >= randomRangeStart && slider.value <= randomRangeStart + rangeLength;

        if (!isSuccess && linkedSliderController != null)
        {
            linkedSliderController.DecreaseProgressBySeconds(6f);
        }

        DeactivateRandomSlider();
    }

    private void SetNewRandomRange()
    {
        randomRangeStart = Random.Range(0, (100 - rangeLength) / 5 + 1) * 5;
        UpdateImagePosition();
    }

    private void UpdateImagePosition()
    {
        float normalizedValue = randomRangeStart / 100f;
        float posY = Mathf.Lerp(posYMin, posYMax, normalizedValue);
        imageRectTransform.anchoredPosition = new Vector2(imageRectTransform.anchoredPosition.x, posY);
    }

    private void InitializeSlider()
    {
        slider.value = 50; // 중앙값에서 시작
        sliderDirection = Random.Range(0, 2) == 0 ? 1 : -1; // 랜덤 방향 설정
        UpdateStatusText(); // 텍스트를 초기화
    }

    private void UpdateStatusText()
    {
        if (statusText != null)
        {
            statusText.text = "Press F"; // 텍스트 고정
        }
    }
}
