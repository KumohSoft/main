using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Unity.VisualScripting;

public class RankingSystem : MonoBehaviour
{
    public GameObject text1, text2, text3, text4, my랭크Text; // Legacy Text 오브젝트
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
        Debug.Log("전체 유저 랭킹 불러오기 시작");
        // GameManager에서 firebaseLogin 스크립트를 찾기
        GameObject gameManager = GameObject.Find("GameManager");
        if (gameManager != null)
        {
            firebaseLogin = gameManager.GetComponent<firebaseLogin>();

            if (firebaseLogin != null)
            {
                // 데이터 로드 시작
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
        Debug.Log("유저 데이터 정렬 시작");
        while (!SomeCondition())
        {
            // 플레이어 리스트를 정렬 (Level 내림차순)
            playerList.Sort((a, b) => b.Level.CompareTo(a.Level));
            await Task.Yield(); // 프레임 대기
        }

        // 정렬 완료 후 내 순위 계산
        myRank = -1; // 기본값 -1 (못 찾았을 경우)
        for (int i = 0; i < playerList.Count; i++)
        {
            if (playerList[i].NickName == myName)
            {
                myRank = i + 1; // 순위는 0-based 인덱스이므로 1 추가
                break;
            }
        }

        if (myRank != -1)
        {
            Debug.Log($"내 순위: {myRank}등");
        }
        else
        {
            Debug.LogWarning("내 순위를 찾을 수 없습니다. 플레이어 리스트를 확인하세요.");
        }
        Debug.Log("정렬 완료");
        await Task.Yield();
    }

    async Task Task3()
    {
        Debug.Log("UI 적용 시작");
        // UI 업데이트
        UpdateRankingUI();

        // 현재 랭킹 텍스트 업데이트
        if (my랭크Text != null)
        {
            if (myRank != -1)
            {
                my랭크Text.GetComponent<Text>().text = $"현재 랭킹 : {myRank}위";
            }
            else
            {
                my랭크Text.GetComponent<Text>().text = "현재 랭킹 : --위";
            }
        }
        else
        {
            Debug.LogWarning("my랭크Text 오브젝트가 설정되지 않았습니다.");
        }

        Debug.Log("UI 적용 완료");
        await Task.Yield();
    }
    bool SomeCondition()
    {
        // 예: 데이터가 모두 로드되었는지 확인하는 조건 추가
        return playerList.Count > 0 && Time.timeSinceLevelLoad > 3f;
    }
    private void UpdateRankingUI()
    {
        int num = slotNum * 4;
        // 텍스트 업데이트
        UpdateText(text1, num + 1, playerList.Count > num + 0 ? playerList[num+0] : null);
        UpdateText(text2, num + 2, playerList.Count > num + 1 ? playerList[num+1] : null);
        UpdateText(text3, num + 3, playerList.Count > num + 2 ? playerList[num+2] : null);
        UpdateText(text4, num + 4, playerList.Count > num + 3 ? playerList[num+3] : null);
        // 리스트 출력 (디버그)
        PrintPlayerList();
    }

    private void UpdateText(GameObject textObject, int rank, PlayerInfo? player)
    {
        if (player == null)
        {
            textObject.GetComponent<Text>().text = $"{rank}등          ---          ---";

            // 부모 오브젝트를 흰색으로 설정
            var parentImage = textObject.transform.parent.GetComponent<Image>();
            if (parentImage != null)
            {
                parentImage.color = Color.white;
            }
        }
        else
        {
            // 닉네임과 레벨의 위치를 고정
            string nickname = (player?.NickName ?? "---").PadRight(15); // 닉네임의 폭을 15자로 고정
            string level = (player?.Level.ToString() ?? "0").PadLeft(5); // 레벨의 폭을 5자로 고정
            string formattedText = $"{rank}등    {nickname}{level}";
            textObject.GetComponent<Text>().text = formattedText;

            // 부모 오브젝트 색상 변경
            var parentImage = textObject.transform.parent.GetComponent<Image>();
            if (parentImage != null)
            {
                if (nickname.Trim() == myName) // Trim()으로 공백 제거 후 비교
                {
                    parentImage.color = new Color(0.5f, 1.0f, 0.5f); // 연두색
                }
                else
                {
                    parentImage.color = Color.white; // 흰색으로 설정
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
