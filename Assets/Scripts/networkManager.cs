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
    public static int Mycharacter2;
    public static int MySkill;
    public GameObject ��;
    public GameObject �����;

    public Text NickName;
    public GameObject LoginPanel;
    public GameObject LobbyPanel;
    public GameObject RoomPanel;
    public static GameObject RoomPanel2;

    public GameObject �α�����;
    public Text �α�����text;
    private Coroutine loadingTextCoroutine;

    [Header("LobbyPanel")]
    public static GameObject Lobbyĵ����;
    public InputField RoomInput;
    public Button[] CellBtn;
    public Button PreviousBtn;
    public Button NextBtn;
    public GameObject HomePanel;
    public GameObject PlayPanel;
    public GameObject ������Panel;
    public GameObject ����Panel;
    public GameObject[] GameChar1;
    public GameObject[] GameChar1�����;
    public GameObject[] charSlotPanel;
    public GameObject[] charSlot����Panel;
    public GameObject ����Ȯ��Panel;
    public Button[] CharactorBTN;
    public Button[] SkillBtn;
    public Image[] skillImage;
    public GameObject ĳ����Panel;
    public GameObject ��ųPanel;
    public GameObject ����ĳ����Panel;
    public GameObject ������ųPanel;
    public GameObject ����Panel;
    public GameObject �̹̺���Panel;
    public GameObject Money;
    public GameObject ��ŷPanel;

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
    public GameObject[] GameChar2�����;
    public GameObject[] PlayerChar;
    public GameObject MakeRoomPanel; 
    


    List<RoomInfo> myList = new List<RoomInfo>();
    int currentPage = 1, maxPage, multiple;
    bool[] playerReady = new bool[8];
    int[] playercharint = new int[8];
    private int slotNum=0;
    // Start is called before the first frame update
    void Start()
    {
        Lobbyĵ����= GameObject.Find("LobbyCanvas");
        RoomPanel2 = RoomPanel;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public InputField temp;
    public void Connect(string nickName)
    {
        �α�����.SetActive(true);
        loadingTextCoroutine = StartCoroutine(UpdateLoadingText(�α�����text, "�α���"));
        PhotonNetwork.ConnectUsingSettings();//���� ��ư�� ������ ����
        PhotonNetwork.LocalPlayer.NickName = nickName;
        //PhotonNetwork.LocalPlayer.NickName = temp.text;
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        print("�������ӿϷ�");
    }
    public void CreateRoom() => PhotonNetwork.CreateRoom(RoomInput.text == "" ? "Room" + UnityEngine.Random.Range(0, 100) : RoomInput.text, new RoomOptions { MaxPlayers = 8 });
    public void Disconnect() => PhotonNetwork.Disconnect();
    public override void OnJoinedRoom()
    {
        print("�� ���� �Ϸ�");
        RoomPanel.SetActive(true);
        LobbyPanel.SetActive(false);
        PlayerName.text = PhotonNetwork.LocalPlayer.NickName;
        RoomName.text = PhotonNetwork.CurrentRoom.Name;
        if (PhotonNetwork.IsMasterClient)
        {
            SetMynumRPC(PhotonNetwork.NickName,Mycharacter2+2);//������Ŭ���̾�Ʈ�� �����

            UpdateGameState(playerReady, playercharint);
        }
        else
        {
            photonView.RPC("SetMynumRPC", RpcTarget.All, PhotonNetwork.NickName, Mycharacter);//������ �ڽ��� ������ ĳ���� ������ �Ѹ���.
        }
    }
    public override void OnDisconnected(DisconnectCause cause) => print("�������");

    public void JoinLobby() => PhotonNetwork.JoinLobby();

    public void LeftRoom()
    {
        photonView.RPC("ReSetMynumRPC", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.NickName);
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        
        for (int i = 0; i < 4; i++)//���� �����鼭 �� ���� �ʱ�ȭ
        {
            playerBtn[i].interactable = false;
            PlayerChar[i].transform.GetChild(playercharint[i]).gameObject.SetActive(false);
            playerBtn[i].transform.GetChild(0).GetComponent<Text>().text = "";
            playerBtn[i].GetComponent<Image>().color = new Color(1f, 1f, 1f);  // RGB: 255, 255, 255
        }
        for (int i = 0; i < 9; i++)
        {
            ChatText[i].text = " ";
        }
        LobbyPanel.SetActive(true);
        MakeRoomPanel.SetActive(false);
        ClickPlayBTN();
        RoomPanel.SetActive(false);
        
        PhotonNetwork.JoinLobby();
        
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        ChatRPC("<color=yellow>" + newPlayer.NickName + "���� �����ϼ̽��ϴ�</color>", newPlayer.NickName);
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("UpdateGameState", RpcTarget.AllBuffered, playerReady, playercharint);
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        ChatRPC("<color=yellow>" + otherPlayer.NickName + "���� �����ϼ̽��ϴ�</color>", otherPlayer.NickName);
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("UpdateGameState", RpcTarget.AllBuffered, playerReady, playercharint);
        }
    }

    public override void OnJoinedLobby()//�κ� �����ϸ�??
    {
        �α�����.SetActive(false);
        LoginPanel.SetActive(false);
        LobbyPanel.SetActive(true);
        StopCoroutine(loadingTextCoroutine);//�ڷ�ƾ �ߴ�
        NickName.text = PhotonNetwork.LocalPlayer.NickName+"��";//�г��� text�� ���� �÷��̾� �г������� ����
        print(PhotonNetwork.LocalPlayer.NickName);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            HandleNewMasterClient();
        }

        void HandleNewMasterClient()
        {
            SetMynumRPC(PhotonNetwork.NickName, Mycharacter2 + 2);
        }
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
        // �ִ�������
        maxPage = (myList.Count % CellBtn.Length == 0) ? myList.Count / CellBtn.Length : myList.Count / CellBtn.Length + 1;

        // ����, ������ư
        PreviousBtn.interactable = (currentPage <= 1) ? false : true;
        NextBtn.interactable = (currentPage >= maxPage) ? false : true;

        // �������� �´� ����Ʈ ����
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
        /*if (PlayPanel.activeSelf == true)
        {
            
        }*/
        
    }

    [PunRPC]
    void ReSetMynumRPC(string name)
    {
        for (int i = 0; i < 4; i++)
        {
            if (i < PhotonNetwork.PlayerList.Length && PhotonNetwork.PlayerList[i].NickName == name)
            {
                playercharint[i] = 0;
                for (int j = i; j < PhotonNetwork.PlayerList.Length; j++)
                {
                    playerReady[j] = playerReady[j + 1];
                    playercharint[j] = playercharint[j + 1];
                }
            }
            
        }
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("UpdateGameState", RpcTarget.AllBuffered, playerReady, playercharint);
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
        if(PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("UpdateGameState", RpcTarget.AllBuffered, playerReady, playercharint);
        }
        
    }

    [PunRPC]
    void UpdateGameState(bool[] playerReady2, int[] playercharint2)//���� ��Ȳ�� ������ ĳ���͸� Ȱ��ȭ ��Ű�� �Լ�
    {
        playerReady = playerReady2;
        playercharint = playercharint2;
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
                for(int j=0; j<4;j++)//��ġ�� ���� �ذ�
                {
                    PlayerChar[i].transform.GetChild(j).gameObject.SetActive(false);
                }
                PlayerChar[i].transform.GetChild(playercharint[i]).gameObject.SetActive(true);//�÷��̾ ������ ĳ���͸� Ȱ��ȭ�ض�

            }
            else
            {
                playerBtn[i].interactable = false;
                PlayerChar[i].transform.GetChild(playercharint[i]).gameObject.SetActive(false);
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
        count.gameObject.SetActive(true);
        count.GetComponent<UnityEngine.UI.Text>().text = "3";
        yield return new WaitForSeconds(1f);
        count.GetComponent<UnityEngine.UI.Text>().text = "2";
        yield return new WaitForSeconds(1f);
        count.GetComponent<UnityEngine.UI.Text>().text = "1";
        yield return new WaitForSeconds(1f);
        count.GetComponent<UnityEngine.UI.Text>().text = "0";

        // �� �ε� ����
        SceneManager.sceneLoaded += OnSceneLoaded; // �� �ε� �̺�Ʈ ���
        SceneManager.LoadScene("Game Scene");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Game Scene�� �ε�Ǿ��� �� ����
        if (scene.name == "Game Scene")
        {
            for(int i=0; i<4; i++)
            {
                playerReady[i] = false;
                playerBtn[i].GetComponent<Image>().color = new Color(1f, 1f, 1f);
            }
            Lobbyĵ����.SetActive(false); // Lobby Canvas ��Ȱ��ȭ
            count.gameObject.SetActive(false); // ī��Ʈ�ٿ� �ؽ�Ʈ ��Ȱ��ȭ

            SceneManager.sceneLoaded -= OnSceneLoaded; // �̺�Ʈ ����
        }
    }

    IEnumerator UpdateLoadingText(Text temp, string S)
    {
        while (true)
        {
            Debug.Log("�Ǵ���");
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

    [PunRPC] // RPC�� �÷��̾ �����ִ� �� ��� �ο����� �����Ѵ�
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
        if (!isInput) // ������ ��ĭ�� ���� �ø�
        {
            for (int i = 1; i < ChatText.Length; i++) ChatText[i - 1].text = ChatText[i].text;
            ChatText[ChatText.Length - 1].text = msg;
        }
    }


    public void ClickPlayBTN()
    {
        PlayPanel.SetActive(true);
        HomePanel.SetActive(false);
        ����Panel.SetActive(false);
        ����Panel.SetActive(false);
        ������Panel.SetActive(false);
        Money.SetActive(false);
        MyListRenewal();
    }

    public void ClickHomeBTN()
    {
        ��ŷPanel.SetActive(false);
        HomePanel.SetActive(true);
        Money.SetActive(true);
        PlayPanel.SetActive(false);
        ������Panel.SetActive(false);
        ����Panel.SetActive(false);
        ����Panel.SetActive(false);
        MakeRoomPanel.SetActive(false);
    }

    public void ClickEnter������()//firebase�� �����͸� �а� Ȱ��ȭ ���θ� ����
    {
        ��ŷPanel.SetActive(false);
        ����Panel.SetActive(false);
        ����Panel.SetActive(false);
        MakeRoomPanel.SetActive(false);
        Money.SetActive(true);
        ������Panel.SetActive(true);
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

    public void ClickExit������()
    {
        ������Panel.SetActive(false);
    }

    public void ClickEnter����()
    {
        ������Panel.SetActive(false);
        ����Panel.SetActive(false);
        MakeRoomPanel.SetActive(false);
        ����Panel.SetActive(true);
        Money.SetActive(true);
        ��ŷPanel.SetActive(false);
    }

    public void ClickExit����()
    {
        ����Panel.SetActive(false);
        ������Panel.SetActive(false);
        ����Panel.SetActive(false);
        MakeRoomPanel.SetActive(false);
        �̹̺���Panel.SetActive(false);
        Money.SetActive(true);
    }
    public void ClickEnter��ŷ()
    {
        ������Panel.SetActive(false);
        ����Panel.SetActive(false);
        MakeRoomPanel.SetActive(false);
        ��ŷPanel.SetActive(true);
        Money.SetActive(true);
    }

    public void ClickExit��ŷ()
    {
        ��ŷPanel.SetActive(false);
        ������Panel.SetActive(false);
        ����Panel.SetActive(false);
        MakeRoomPanel.SetActive(false);
        �̹̺���Panel.SetActive(false);
        Money.SetActive(true);
    }

    public void Click��ŷPlus()
    {
        // ��ŷPanel�� ����� RankingSystem ��ũ��Ʈ�� ��������
        RankingSystem rankingSystem = ��ŷPanel.GetComponent<RankingSystem>();

        if (rankingSystem != null)
        {
            // RankingSystem ��ũ��Ʈ�� plusBtn �޼��� ����
            rankingSystem.plusBtn();
        }
        else
        {
            Debug.LogError("RankingSystem ��ũ��Ʈ�� ��ŷPanel�� ����Ǿ� ���� �ʽ��ϴ�.");
        }
    }
    public void Click��ŷMinus()
    {
        // ��ŷPanel�� ����� RankingSystem ��ũ��Ʈ�� ��������
        RankingSystem rankingSystem = ��ŷPanel.GetComponent<RankingSystem>();

        if (rankingSystem != null)
        {
            // RankingSystem ��ũ��Ʈ�� plusBtn �޼��� ����
            rankingSystem.minusBtn();
        }
        else
        {
            Debug.LogError("RankingSystem ��ũ��Ʈ�� ��ŷPanel�� ����Ǿ� ���� �ʽ��ϴ�.");
        }
    }
    public void ClickCharactorImage(int num)//�㸦 �����ϴ� �Լ�
    {
        if (firebaseLogin.playerInfo.Character[2*num] ==1)
        {
            for (int i = 0; i < GameChar1.Length; i++)
            {
                GameChar1[i].SetActive(false);
                GameChar2[i].SetActive(false);

            }
            GameChar1[num].SetActive(true);
            GameChar2[num].SetActive(true);
            Mycharacter = num;
        }
    }

    public void ClickCharactorImage2(int num)//����̸� �����ϴ� �Լ�
    {
        if (firebaseLogin.playerInfo.Character[2*num+1] == 1)
        {
            for (int i = 0; i < GameChar1�����.Length; i++)
            {
                GameChar1�����[i].SetActive(false);
                GameChar2�����[i].SetActive(false);

            }
            GameChar1�����[num].SetActive(true);
            GameChar2�����[num].SetActive(true);
            Mycharacter2 = num;
        }
    }


    public void ClickSkillImage(int num)
    {
        for(int i=0; i<skillImage.Length; i++)
        {
            skillImage[i].gameObject.SetActive(false);
        }
        skillImage[num-1].gameObject.SetActive(true);
        MySkill = num;
        print(num);
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
            charSlot����Panel[i].SetActive(false);
        }
        charSlotPanel[slotNum].SetActive(true);
        charSlot����Panel[slotNum].SetActive(true);
    }

    public void Click����Charactor(int num)
    {
        ����Ȯ��Panel.SetActive(true);
    }
    public void Click����CharactorExit()
    {
        ����Ȯ��Panel.SetActive(false);
    }

    public void ��������ųClick()
    {
        ĳ����Panel.SetActive(false);
        ��ųPanel.SetActive(true);
        ����ĳ����Panel.SetActive(false);
        ������ųPanel.SetActive(true);
    }

    public void ������ĳ����Click()
    {
        ĳ����Panel.SetActive(true);
        ��ųPanel.SetActive(false);
        ����ĳ����Panel.SetActive(true);
        ������ųPanel.SetActive(false);
        Money.SetActive(true);
    }

    public void ����Click()
    {
        ������Panel.SetActive(false);
        ����Panel.SetActive(false);
        MakeRoomPanel.SetActive(false);
        ����Panel.SetActive(true);
    }

    public void MakeRoomPanelClick()
    {
        ����Panel.SetActive(false);
        ������Panel.SetActive(false);
        ����Panel.SetActive(false);
        MakeRoomPanel.SetActive(true);
    }
    public void �̹̺���PanelExit()
    {
        �̹̺���Panel.SetActive(false);
    }
}
