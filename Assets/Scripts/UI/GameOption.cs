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
        // �ػ� �ɼ� �ʱ�ȭ
        InitializeResolutions();

        // UI �ʱⰪ ����
        InitializeUI();

        // �����̴� �� ���� �� ȣ��� �̺�Ʈ ���
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
    }

    // �ػ� �ɼ� �ʱ�ȭ
    void InitializeResolutions()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        // �ػ� ��� �߰�
        List<string> resolutionOptions = new List<string>();
        foreach (Resolution resolution in resolutions)
        {
            if (Mathf.Approximately((float)resolution.width / resolution.height, 16f / 9f))
            {
                resolutionOptions.Add(resolution.width + " x " + resolution.height);
            }
        }

        resolutionDropdown.AddOptions(resolutionOptions);

        // ���� �ػ󵵸� �⺻������ ����
        //resolutionDropdown.value = GetCurrentResolutionIndex();
        //resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }

    // UI �ʱ�ȭ
    void InitializeUI()
    {
        // ���� �ɼ� ����
        screenModeDropdown.ClearOptions();

        // ȭ�� ��� ��� �߰� (��ü ȭ��� â ��常 ����)
        List<string> screenModeOptions = new List<string>
        {
            "��ü ȭ��",
            "â ���(1920*1080)",
            "â ���(1600*900)",
            "â ���(1366*786)",
            "â ���(1280*720)"
        };
        screenModeDropdown.AddOptions(screenModeOptions);

        if (Screen.fullScreen)
        {
            screenModeDropdown.value = 0; // ��ü ȭ��
        }
        else
        {
            // â ����� ���, ���� �ػ󵵿� �´� �ɼ� ����
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
                screenModeDropdown.value = 1; // �⺻��: â ���(1920*1080)
        }
        screenModeDropdown.onValueChanged.AddListener(SetScreenMode);

        // ������ ���� �ʱⰪ ����
        masterVolumeSlider.value = AudioListener.volume;
    }

    // ���� �ػ��� �ε����� ��ȯ
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
        return 0; // �⺻��
    }

    // �ػ� ����
    void SetResolution(int index)
    {
        if (index >= 0 && index < resolutions.Length)
        {
            Resolution resolution = resolutions[index];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }
    }

    // ȭ�� ��� ����
    void SetScreenMode(int index)
    {
        bool isFullScreen = index == 0;

        switch (index)
        {
            case 0: // ��ü ȭ��
                Screen.SetResolution(1920, 1080, true);
                break;
            case 1: // â ���(1920*1080)
                Screen.SetResolution(1920, 1080, false);
                break;
            case 2: // â ���(1600*900)
                Screen.SetResolution(1600, 900, false);
                break;
            case 3: // â ���(1366*786)
                Screen.SetResolution(1366, 786, false);
                break;
            case 4: // â ���(1280*720)
                Screen.SetResolution(1280, 720, false);
                break;
            default:
                Debug.LogError("Unknown screen mode index");
                break;
        }
    }

    // ������ ���� ����
    void SetMasterVolume(float volume)
    {
        AudioListener.volume = volume;
    }
}
