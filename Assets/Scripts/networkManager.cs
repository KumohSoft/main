using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Unity.VisualScripting;
using System;
using System.Threading;
using UnityEngine.SceneManagement;
using Photon.Pun.Demo.Cockpit;

public class networkManager : MonoBehaviourPunCallbacks
{
    [Header("Mycharacter")]
    public static int Mycharacter;
    public GameObject 쥐;
    public GameObject 고양이;

    public Text NickName;
    public GameObject LoginPanel;
    public GameObject LobbyPanel;
    public GameObject RoomPanel;

    public GameObject 로그인중;
    public Text 로그인중text;
    private Coroutine loadingTextCoroutine;

    [Header("LobbyPanel")]
    public InputField RoomInput;
    public Button[] CellBtn;
    public Button PreviousBtn;
    public Button NextBtn;
    public GameObject HomePanel;
    public GameObject PlayPanel;
    public GameObject 내정보Panel;
    public GameObject 상점Panel;
    public GameObject[] GameChar1;
    public GameObject[] charSlotPanel;
    public GameObject[] charSlot상점Panel;
    public GameObject 구매확인Panel;
    public Button[] CharactorBTN;
    public Button[] SkillBtn;
    public GameObject 캐릭터Panel;
    public GameObject 스킬Panel;
    public GameObject 상점캐릭터Panel;
    public GameObject 상점스킬Panel;

    [Header("RoomPanel")]
    public Text PlayerName;
    public Text RoomName;
    public Text count;
    public Button[] playerBtn;
    public GameObject[] playerchar;
    public GameObject character;
    public Text[] ChatText;
    public InputField ChatInput;
    public GameObject[] GameChar2;
    public GameObject[] PlayerChar;
    


    List<RoomInfo> myList = new List<RoomInfo>();
    int currentPage = 1, maxPage, multiple;
    bool[] playerReady = new bool[8];
    int[] playercharint = new int[8];
    private int slotNum=0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public InputField temp;
    public void Connect(string nickName)
    {
        로그인중.SetActive(true);
        loadingTextCoroutine = StartCoroutine(UpdateLoadingText(로그인중text, "로그인"));
        PhotonNetwork.ConnectUsingSettings();//시작 버튼을 누르면 연결
        PhotonNetwork.LocalPlayer.NickName = nickName;
        //PhotonNetwork.LocalPlayer.NickName = temp.text;
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        print("서버접속완료");
    }
    public void CreateRoom() => PhotonNetwork.CreateRoom(RoomInput.text == "" ? "Room" + UnityEngine.Random.Range(0, 100) : RoomInput.text, new RoomOptions { MaxPlayers = 8 });
    public void Disconnect() => PhotonNetwork.Disconnect();
    public override void OnJoinedRoom()
    {
        print("방 접속 완료");
        RoomPanel.SetActive(true);
        LobbyPanel.SetActive(false);
        PlayerName.text = PhotonNetwork.LocalPlayer.NickName;
        RoomName.text = PhotonNetwork.CurrentRoom.Name;
        if (PhotonNetwork.IsMasterClient)
        {
            SetMynumRPC(PhotonNetwork.NickName,Mycharacter);

            UpdateGameState(playerReady);
        }
        else
        {
            photonView.RPC("SetMynumRPC", RpcTarget.All, PhotonNetwork.NickName, Mycharacter);
        }
    }
    public override void OnDisconnected(DisconnectCause cause) => print("연결끊김");

    public void JoinLobby() => PhotonNetwork.JoinLobby();

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        ChatRPC("<color=yellow>" + newPlayer.NickName + "님이 참가하셨습니다</color>", newPlayer.NickName);
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("UpdateGameState", RpcTarget.All, playerReady);
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        ChatRPC("<color=yellow>" + otherPlayer.NickName + "님이 퇴장하셨습니다</color>", otherPlayer.NickName);
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("UpdateGameState", RpcTarget.All, playerReady);
        }
    }

    public override void OnJoinedLobby()//로비에 접속하면??
    {
        로그인중.SetActive(false);
        LoginPanel.SetActive(false);
        LobbyPanel.SetActive(true);
        StopCoroutine(loadingTextCoroutine);//코루틴 중단
        NickName.text = PhotonNetwork.LocalPlayer.NickName+"님";//닉네임 text를 로컬 플레이어 닉네임으로 설정
        print(PhotonNetwork.LocalPlayer.NickName);
    }

    public void MyListClick(int num)
    {
        if (num == -2) --currentPage;
        else if (num == -1) ++currentPage;
        else PhotonNetwork.JoinRoom(myList[multiple + num].Name);
        MyListRenewal();
    }

    void MyListRenewal()
    {
        // 최대페이지
        maxPage = (myList.Count % CellBtn.Length == 0) ? myList.Count / CellBtn.Length : myList.Count / CellBtn.Length + 1;

        // 이전, 다음버튼
        PreviousBtn.interactable = (currentPage <= 1) ? false : true;
        NextBtn.interactable = (currentPage >= maxPage) ? false : true;

        // 페이지에 맞는 리스트 대입
        multiple = (currentPage - 1) * CellBtn.Length;
        for (int i = 0; i < CellBtn.Length; i++)
        {
            CellBtn[i].interactable = (multiple + i < myList.Count) ? true : false;
            CellBtn[i].transform.GetChild(0).GetComponent<Text>().text = (multiple + i < myList.Count) ? myList[multiple + i].Name : "";
            CellBtn[i].transform.GetChild(1).GetComponent<Text>().text = (multiple + i < myList.Count) ? myList[multiple + i].PlayerCount + "/" + myList[multiple + i].MaxPlayers : "";
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if(PlayPanel.activeSelf == true)
        {
            int roomCount = roomList.Count;
            for (int i = 0; i < roomCount; i++)
            {
                if (!roomList[i].RemovedFromList)
                {
                    if (!myList.Contains(roomList[i])) myList.Add(roomList[i]);
                    else myList[myList.IndexOf(roomList[i])] = roomList[i];
                }
                else if (myList.IndexOf(roomList[i]) != -1) myList.RemoveAt(myList.IndexOf(roomList[i]));
            }
            MyListRenewal();
        }
        
    }

    [PunRPC]
    void SetMynumRPC(string name, int num)
    {
        for (int i = 0; i < 4; i++)
        {
            if (i < PhotonNetwork.PlayerList.Length && PhotonNetwork.PlayerList[i].NickName == name)
            {
                playercharint[i] = num;
            }
        }
        UpdateGameState(playerReady);
    }

    [PunRPC]
    void UpdateGameState(bool[] playerReady2)
    {
        playerReady = playerReady2;
        for (int i = 0; i < 4; i++)
        {
            if (i < PhotonNetwork.PlayerList.Length)
            {
                playerBtn[i].interactable = true;
                playerBtn[i].transform.GetChild(0).GetComponent<Text>().text = PhotonNetwork.PlayerList[i].NickName;
                if (playerReady2[i])
                {
                    playerBtn[i].GetComponent<Image>().color = new Color(1f, 1f, 0f);  // RGB: 255, 255, 0
                }
                else
                {
                    playerBtn[i].GetComponent<Image>().color = new Color(1f, 1f, 1f);  // RGB: 255, 255, 255
                }
                PlayerChar[i].transform.GetChild(playercharint[i]).gameObject.SetActive(true);

            }
            else
            {
                playerBtn[i].interactable = false;
                playerBtn[i].transform.GetChild(0).GetComponent<Text>().text = "";
                playerBtn[i].GetComponent<Image>().color = new Color(1f, 1f, 1f);  // RGB: 255, 255, 255
            }
        }
    }

    public void Ready()
    {
        int num = 0;
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (PhotonNetwork.PlayerList[i].NickName == PhotonNetwork.LocalPlayer.NickName)
            {
                num = i;
                break;
            }
        }
        photonView.RPC("ReadyRPC", RpcTarget.All, num);
    }

    [PunRPC]
    void ReadyRPC(int num)
    {
        if (!playerReady[num])
        {
            playerReady[num] = true;
            playerBtn[num].GetComponent<Image>().color = new Color(1f, 1f, 0f);  // RGB: 255, 255, 0
        }
        else
        {
            playerReady[num] = false;
            playerBtn[num].GetComponent<Image>().color = new Color(1f, 1f, 1f);  // RGB: 255, 255, 255
        }

        if (PhotonNetwork.PlayerList.Length >= 1)
        {
            bool sig = true;
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                if (!playerReady[i])
                {
                    sig = false;
                }
            }

            if (sig)
            {
                StartCoroutine(GameStart());
            }
        }
    }

    IEnumerator GameStart()
    {
        count.text = "3";
        yield return new WaitForSeconds(1f);
        count.text = "2";
        yield return new WaitForSeconds(1f);
        count.text = "1";
        yield return new WaitForSeconds(1f);
        count.text = "0";

        SceneManager.LoadScene("Game Scene");
    }

    IEnumerator UpdateLoadingText(Text temp, string S)
    {
        while (true)
        {
            Debug.Log("되는중");
            temp.text = S + ".";
            yield return new WaitForSeconds(0.2f);
            temp.text = S + "..";
            yield return new WaitForSeconds(0.2f);
            temp.text = S + "...";
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void Send()
    {
        string senderName = PhotonNetwork.NickName;
        photonView.RPC("ChatRPC", RpcTarget.All, PhotonNetwork.NickName + " : " + ChatInput.text, senderName);
        ChatInput.text = "";
        ChatInput.ActivateInputField();
    }

    [PunRPC] // RPC는 플레이어가 속해있는 방 모든 인원에게 전달한다
    void ChatRPC(string msg, string senderName)
    {
        if (senderName == PhotonNetwork.NickName)
        {
            msg = "<color=yellow>" + msg + "</color>";
        }
        else
        {
            msg = "<color=white>" + msg + "</color>";
        }
        bool isInput = false;
        for (int i = 0; i < ChatText.Length; i++)
            if (ChatText[i].text == "")
            {
                isInput = true;
                ChatText[i].text = msg;
                break;
            }
        if (!isInput) // 꽉차면 한칸씩 위로 올림
        {
            for (int i = 1; i < ChatText.Length; i++) ChatText[i - 1].text = ChatText[i].text;
            ChatText[ChatText.Length - 1].text = msg;
        }
    }


    public void ClickPlayBTN()
    {
        HomePanel.SetActive(false);
        PlayPanel.SetActive(true);
        MyListRenewal();
    }

    public void ClickHomeBTN()
    {
        HomePanel.SetActive(true);
        PlayPanel.SetActive(false);
        내정보Panel.SetActive(false);
        상점Panel.SetActive(false);
    }

    public void ClickEnter내정보()
    {
        상점Panel.SetActive(false);
        내정보Panel.SetActive(true);
        for (int i = 0; i < 4; i++)
        {
            if (firebaseLogin.playerInfo.Character[i] == 0)
            {
                CharactorBTN[i].interactable = false;
            }
            else
            {
                CharactorBTN[i].interactable = true;
            }
        }

        for(int i=0; i<2; i++)
        {
            if (firebaseLogin.playerInfo.Item[i] == 0)
            {
                SkillBtn[i].interactable = false;
            }
            else
            {
                SkillBtn[i].interactable = true;
            }
        }
    }

    public void ClickExit내정보()
    {
        내정보Panel.SetActive(false);
    }

    public void ClickEnter상점()
    {
        내정보Panel.SetActive(false);
        상점Panel.SetActive(true);
    }

    public void ClickExit상점()
    {
        상점Panel.SetActive(false);
    }

    public void ClickCharactorImage(int num)
    {
        if (firebaseLogin.playerInfo.Character[num] ==1)
        {
            for (int i = 0; i < 4; i++)
            {
                GameChar1[i].SetActive(false);
                GameChar2[i].SetActive(false);

            }
            GameChar1[num].SetActive(true);
            GameChar2[num].SetActive(true);
            Mycharacter = num;
        }
    }

    public void ClickCharPlus(int num)
    {
        if(num==0&& slotNum>0)
        {
            slotNum--;
        }
        else if(num==1&& slotNum<1)
        {
            slotNum++;
        }

        for (int i = 0; i < 2; i++)
        {
            charSlotPanel[i].SetActive(false);
            charSlot상점Panel[i].SetActive(false);
        }
        charSlotPanel[slotNum].SetActive(true);
        charSlot상점Panel[slotNum].SetActive(true);
    }

    public void Click상점Charactor(int num)
    {
        구매확인Panel.SetActive(true);
    }
    public void Click상점CharactorExit()
    {
        구매확인Panel.SetActive(false);
    }

    public void 내정보스킬Click()
    {
        캐릭터Panel.SetActive(false);
        스킬Panel.SetActive(true);
        상점캐릭터Panel.SetActive(false);
        상점스킬Panel.SetActive(true);
    }

    public void 내정보캐릭터Click()
    {
        캐릭터Panel.SetActive(true);
        스킬Panel.SetActive(false);
        상점캐릭터Panel.SetActive(true);
        상점스킬Panel.SetActive(false);
    }
}
