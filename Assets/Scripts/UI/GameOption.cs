using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOption : MonoBehaviour
{
    public Dropdown resolutionDropdown;
    public Dropdown screenModeDropdown;
    public Slider masterVolumeSlider;

    private Resolution[] resolutions;

    void Start()
    {
        // 해상도 옵션 초기화
        InitializeResolutions();

        // UI 초기값 설정
        InitializeUI();

        // 슬라이더 값 변경 시 호출될 이벤트 등록
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
    }

    // 해상도 옵션 초기화
    void InitializeResolutions()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        // 해상도 목록 추가
        foreach (Resolution resolution in resolutions)
        {
            resolutionDropdown.options.Add(new Dropdown.OptionData(resolution.width + " x " + resolution.height));
        }

        // 현재 해상도를 기본값으로 설정
        resolutionDropdown.value = GetCurrentResolutionIndex();
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }

    // UI 초기화
    void InitializeUI()
    {
        // 기존 옵션 제거
        screenModeDropdown.ClearOptions();

        // 화면 모드 초기값 설정
        screenModeDropdown.value = (int)GetCurrentScreenMode();
        screenModeDropdown.onValueChanged.AddListener(SetScreenMode);

        // 화면 모드 목록 추가
        screenModeDropdown.AddOptions(new System.Collections.Generic.List<string>
        {
            "전체 화면",
            "창 모드",
            "무경계 창 모드"
        });

        // 마스터 볼륨 초기값 설정
        masterVolumeSlider.value = AudioListener.volume;
    }

    // 현재 해상도의 인덱스 반환
    int GetCurrentResolutionIndex()
    {
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                return i;
            }
        }
        return 0;
    }

    // 현재 화면 모드 반환
    FullScreenMode GetCurrentScreenMode()
    {
        if (Screen.fullScreenMode == FullScreenMode.FullScreenWindow)
            return FullScreenMode.FullScreenWindow;
        else if (Screen.fullScreenMode == FullScreenMode.Windowed)
            return FullScreenMode.Windowed;
        else
            return FullScreenMode.ExclusiveFullScreen;
    }

    // 해상도 설정
    void SetResolution(int index)
    {
        if (index >= 0 && index < resolutions.Length)
        {
            Resolution resolution = resolutions[index];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode);
        }
    }

    // 화면 모드 설정
    void SetScreenMode(int index)
    {
        FullScreenMode mode = (FullScreenMode)index;
        Screen.fullScreenMode = mode;
    }

    // 마스터 볼륨 설정
    void SetMasterVolume(float volume)
    {
        AudioListener.volume = volume;
    }
}
