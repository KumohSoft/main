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
        //resolutionDropdown.value = GetCurrentResolutionIndex();
        //resolutionDropdown.onValueChanged.AddListener(SetResolution);
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
            "창 모드(1920*1080)",
            "창 모드(1600*900)",
            "창 모드(1366*786)",
            "창 모드(1280*720)"
        };
        screenModeDropdown.AddOptions(screenModeOptions);

        if (Screen.fullScreen)
        {
            screenModeDropdown.value = 0; // 전체 화면
        }
        else
        {
            // 창 모드인 경우, 현재 해상도에 맞는 옵션 선택
            Resolution currentResolution = Screen.currentResolution;
            if (currentResolution.width == 1920 && currentResolution.height == 1080)
                screenModeDropdown.value = 1;
            else if (currentResolution.width == 1600 && currentResolution.height == 900)
                screenModeDropdown.value = 2;
            else if (currentResolution.width == 1366 && currentResolution.height == 786)
                screenModeDropdown.value = 3;
            else if (currentResolution.width == 1280 && currentResolution.height == 720)
                screenModeDropdown.value = 4;
            else
                screenModeDropdown.value = 1; // 기본값: 창 모드(1920*1080)
        }
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
        bool isFullScreen = index == 0;

        switch (index)
        {
            case 0: // 전체 화면
                Screen.SetResolution(1920, 1080, true);
                break;
            case 1: // 창 모드(1920*1080)
                Screen.SetResolution(1920, 1080, false);
                break;
            case 2: // 창 모드(1600*900)
                Screen.SetResolution(1600, 900, false);
                break;
            case 3: // 창 모드(1366*786)
                Screen.SetResolution(1366, 786, false);
                break;
            case 4: // 창 모드(1280*720)
                Screen.SetResolution(1280, 720, false);
                break;
            default:
                Debug.LogError("Unknown screen mode index");
                break;
        }
    }

    // 마스터 볼륨 설정
    void SetMasterVolume(float volume)
    {
        AudioListener.volume = volume;
    }
}
