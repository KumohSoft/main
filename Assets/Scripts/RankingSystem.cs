using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Unity.VisualScripting;

public class RankingSystem : MonoBehaviour
{
    public GameObject text1, text2, text3, text4, my��ũText; // Legacy Text ������Ʈ
    private firebaseLogin firebaseLogin;

    private int slotNum;
    private int myRank;
    private string myName;

    public List<PlayerInfo> playerList = new List<PlayerInfo>();
    async void OnEnable()
    {
        await RunTasksSequentially();
    }
    async Task RunTasksSequentially()
    {
        await Task1();
        await Task2();
        await Task3();
    }
    async Task Task1()
    {
        Debug.Log("��ü ���� ��ŷ �ҷ����� ����");
        // GameManager���� firebaseLogin ��ũ��Ʈ�� ã��
        GameObject gameManager = GameObject.Find("GameManager");
        if (gameManager != null)
        {
            firebaseLogin = gameManager.GetComponent<firebaseLogin>();

            if (firebaseLogin != null)
            {
                // ������ �ε� ����
                firebaseLogin.FetchAndStoreRanking();
                Debug.Log("FetchAndStoreRanking called from RankingSystem.");
            }
            else
            {
                Debug.LogError("firebaseLogin script not found on GameManager.");
            }
        }
        else
        {
            Debug.LogError("GameManager object not found.");
        }
        myName = firebaseLogin.playerNickName;
        await Task.Yield();
    }

    async Task Task2()
    {
        Debug.Log("���� ������ ���� ����");
        while (!SomeCondition())
        {
            // �÷��̾� ����Ʈ�� ���� (Level ��������)
            playerList.Sort((a, b) => b.Level.CompareTo(a.Level));
            await Task.Yield(); // ������ ���
        }

        // ���� �Ϸ� �� �� ���� ���
        myRank = -1; // �⺻�� -1 (�� ã���� ���)
        for (int i = 0; i < playerList.Count; i++)
        {
            if (playerList[i].NickName == myName)
            {
                myRank = i + 1; // ������ 0-based �ε����̹Ƿ� 1 �߰�
                break;
            }
        }

        if (myRank != -1)
        {
            Debug.Log($"�� ����: {myRank}��");
        }
        else
        {
            Debug.LogWarning("�� ������ ã�� �� �����ϴ�. �÷��̾� ����Ʈ�� Ȯ���ϼ���.");
        }
        Debug.Log("���� �Ϸ�");
        await Task.Yield();
    }

    async Task Task3()
    {
        Debug.Log("UI ���� ����");
        // UI ������Ʈ
        UpdateRankingUI();

        // ���� ��ŷ �ؽ�Ʈ ������Ʈ
        if (my��ũText != null)
        {
            if (myRank != -1)
            {
                my��ũText.GetComponent<Text>().text = $"���� ��ŷ : {myRank}��";
            }
            else
            {
                my��ũText.GetComponent<Text>().text = "���� ��ŷ : --��";
            }
        }
        else
        {
            Debug.LogWarning("my��ũText ������Ʈ�� �������� �ʾҽ��ϴ�.");
        }

        Debug.Log("UI ���� �Ϸ�");
        await Task.Yield();
    }
    bool SomeCondition()
    {
        // ��: �����Ͱ� ��� �ε�Ǿ����� Ȯ���ϴ� ���� �߰�
        return playerList.Count > 0 && Time.timeSinceLevelLoad > 3f;
    }
    private void UpdateRankingUI()
    {
        int num = slotNum * 4;
        // �ؽ�Ʈ ������Ʈ
        UpdateText(text1, num + 1, playerList.Count > num + 0 ? playerList[num+0] : null);
        UpdateText(text2, num + 2, playerList.Count > num + 1 ? playerList[num+1] : null);
        UpdateText(text3, num + 3, playerList.Count > num + 2 ? playerList[num+2] : null);
        UpdateText(text4, num + 4, playerList.Count > num + 3 ? playerList[num+3] : null);
        // ����Ʈ ��� (�����)
        PrintPlayerList();
    }

    private void UpdateText(GameObject textObject, int rank, PlayerInfo? player)
    {
        if (player == null)
        {
            textObject.GetComponent<Text>().text = $"{rank}��          ---          ---";

            // �θ� ������Ʈ�� ������� ����
            var parentImage = textObject.transform.parent.GetComponent<Image>();
            if (parentImage != null)
            {
                parentImage.color = Color.white;
            }
        }
        else
        {
            // �г��Ӱ� ������ ��ġ�� ����
            string nickname = (player?.NickName ?? "---").PadRight(15); // �г����� ���� 15�ڷ� ����
            string level = (player?.Level.ToString() ?? "0").PadLeft(5); // ������ ���� 5�ڷ� ����
            string formattedText = $"{rank}��    {nickname}{level}";
            textObject.GetComponent<Text>().text = formattedText;

            // �θ� ������Ʈ ���� ����
            var parentImage = textObject.transform.parent.GetComponent<Image>();
            if (parentImage != null)
            {
                if (nickname.Trim() == myName) // Trim()���� ���� ���� �� ��
                {
                    parentImage.color = new Color(0.5f, 1.0f, 0.5f); // ���λ�
                }
                else
                {
                    parentImage.color = Color.white; // ������� ����
                }
            }
        }
    }

    public void plusBtn()
    {
        if(playerList.Count > slotNum * 4)
        {
            slotNum += 1;
        }
        UpdateRankingUI();
    }
    public void minusBtn()
    {
        if (slotNum > 0)
        {
            slotNum -= 1;
        }
        UpdateRankingUI();
    }
    public void PrintPlayerList()
    {
        Debug.Log("=== Player List ===");
        for (int i = 0; i < playerList.Count; i++)
        {
            PlayerInfo player = playerList[i];
            Debug.Log($"Rank {i + 1}: NickName = {player.NickName}, Level = {player.Level}");
        }
        Debug.Log("===================");
    }
}
