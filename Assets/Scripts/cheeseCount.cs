using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    public Slider slider; // UI Slider 연결
    public float duration = 30f; // Slider가 채워지는 시간
    private float elapsedTime = 0f; // 경과 시간
    private bool isActive = false; // Slider가 채워지는 활성화 상태
    private bool isDecreasing = false; // Slider가 감소 중인지 상태

    void Start()
    {
        if (slider == null)
        {
            Debug.LogError("Slider가 연결되지 않았습니다.");
            return;
        }

        slider.value = 0f; // 초기값 설정
        slider.maxValue = 1f; // 최대값 설정
        slider.gameObject.SetActive(false); // 시작 시 Slider 비활성화
    }

    void Update()
    {
        if (isActive)
        {
            // Slider 채우기 로직
            if (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                slider.value = elapsedTime / duration;
            }
            else
            {
                CompleteSlider();
            }
        }
        else if (isDecreasing)
        {
            // Slider 감소 로직
            if (slider.value > 0)
            {
                slider.value -= Time.deltaTime / duration;
            }
            else
            {
                StopDecreasing();
            }
        }
    }

    public void ActivateSlider()
    {
        isActive = true;
        isDecreasing = false;
        slider.gameObject.SetActive(true); // Slider 표시
        elapsedTime = 0f; // 경과 시간 초기화
        slider.value = 0f; // 진행도 초기화
    }

    public void DeactivateSlider()
    {
        isActive = false;
        isDecreasing = true; // 감소 시작
    }

    private void StopDecreasing()
    {
        isDecreasing = false; // 감소 상태 종료
        slider.gameObject.SetActive(false); // Slider 숨김
        elapsedTime = 0f; // 경과 시간 초기화
    }

    private void CompleteSlider()
    {
        isActive = false;
        Debug.Log("Slider 완료!");
    }
}
