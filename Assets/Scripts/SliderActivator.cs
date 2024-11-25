using UnityEngine;

public class SliderActivator : MonoBehaviour
{
    public SliderController sliderController; // SliderController 연결
    public GameObject statusTextObject; // 트리거 안내 메시지
    public GameObject completionTextObject; // 완료 메시지
    private bool isPlayerInTrigger = false; // 플레이어가 트리거 안에 있는지 확인

    void Start()
{
    if (statusTextObject != null)
    {
        statusTextObject.SetActive(false); // 트리거 안내 메시지 초기 비활성화
    }

    if (completionTextObject != null)
    {
        completionTextObject.SetActive(false); // 완료 메시지 초기 비활성화
    }

    sliderController.OnSliderCompleted += OnSliderCompleted; // Slider 완료 이벤트 구독

    sliderController.StartDecreasing(); 
}

void Update()
{
    if (isPlayerInTrigger && !sliderController.IsCompleted() && Input.GetKeyDown(KeyCode.E))
    {
        Debug.Log("E key pressed. Activating slider...");
        sliderController.StartIncreasing(); // E 키로 슬라이더 시작
        UpdateTextVisibility();
    }

    if (sliderController != null && Input.anyKeyDown && IsCancelKeyPressed())
    {
        sliderController.StartDecreasing(); // 다른 키로 감소
    }
}


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = true;
            UpdateTextVisibility();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
            if (statusTextObject != null)
            {
                statusTextObject.SetActive(false);
            }

            if (completionTextObject != null)
            {
                completionTextObject.SetActive(false);
            }
        }
    }

    private void OnSliderCompleted()
    {
        UpdateTextVisibility();
    }

    private void UpdateTextVisibility()
    {
        if (sliderController.IsCompleted())
        {
            if (completionTextObject != null)
            {
                completionTextObject.SetActive(true);
            }

            if (statusTextObject != null)
            {
                statusTextObject.SetActive(false);
            }
        }
        else
        {
            if (statusTextObject != null)
            {
                statusTextObject.SetActive(true);
            }

            if (completionTextObject != null)
            {
                completionTextObject.SetActive(false);
            }
        }
    }

    private bool IsCancelKeyPressed()
    {
        return Input.GetKey(KeyCode.W) || 
               Input.GetKey(KeyCode.A) || 
               Input.GetKey(KeyCode.S) || 
               Input.GetKey(KeyCode.D) || 
               Input.GetKey(KeyCode.Space);
    }
}
