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
        List<string> resolutionOptions = new List<string>();
        foreach (Resolution resolution in resolutions)
        {
            if (Mathf.Approximately((float)resolution.width / resolution.height, 16f / 9f))
            {
                resolutionOptions.Add(resolution.width + " x " + resolution.height);
            }
        }

        resolutionDropdown.AddOptions(resolutionOptions);

        // 현재 해상도를 기본값으로 설정
        resolutionDropdown.value = GetCurrentResolutionIndex();
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }

    // UI 초기화
    void InitializeUI()
    {
        // 기존 옵션 제거
        screenModeDropdown.ClearOptions();

        // 화면 모드 목록 추가 (전체 화면과 창 모드만 남김)
        List<string> screenModeOptions = new List<string>
        {
            "전체 화면",
            "창 모드"
        };
        screenModeDropdown.AddOptions(screenModeOptions);

        // 화면 모드 초기값 설정
        screenModeDropdown.value = Screen.fullScreen ? 0 : 1; // 전체 화면: 0, 창 모드: 1
        screenModeDropdown.onValueChanged.AddListener(SetScreenMode);

        // 마스터 볼륨 초기값 설정
        masterVolumeSlider.value = AudioListener.volume;
    }

    // 현재 해상도의 인덱스를 반환
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
        return 0; // 기본값
    }

    // 해상도 설정
    void SetResolution(int index)
    {
        if (index >= 0 && index < resolutions.Length)
        {
            Resolution resolution = resolutions[index];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }
    }

    // 화면 모드 설정
    void SetScreenMode(int index)
    {
        // index 0: 전체 화면, 1: 창 모드
        bool isFullScreen = index == 0;

        // 현재 선택된 해상도와 화면 모드 설정
        Resolution currentResolution = resolutions[resolutionDropdown.value];
        Screen.SetResolution(currentResolution.width, currentResolution.height, isFullScreen);
    }

    // 마스터 볼륨 설정
    void SetMasterVolume(float volume)
    {
        AudioListener.volume = volume;
    }
}
