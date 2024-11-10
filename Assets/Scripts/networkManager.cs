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

    [Header("RoomPanel")]
    public Text PlayerName;
    public Text RoomName;
    public Text count;
    public Button[] playerBtn;

    List<RoomInfo> myList = new List<RoomInfo>();
    int currentPage = 1, maxPage, multiple;
    bool[] playerReady = new bool[8];
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Connect(string nickName)
    {
        로그인중.SetActive(true);
        loadingTextCoroutine = StartCoroutine(UpdateLoadingText(로그인중text, "로그인"));
        PhotonNetwork.ConnectUsingSettings();//시작 버튼을 누르면 연결
        PhotonNetwork.LocalPlayer.NickName = nickName;
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
        PlayerName.text = PhotonNetwork.LocalPlayer.NickName;
        RoomName.text = PhotonNetwork.CurrentRoom.Name;
        if (PhotonNetwork.IsMasterClient)
        {
            UpdateGameState(playerReady);
        }

    }
    public override void OnDisconnected(DisconnectCause cause) => print("연결끊김");

    public void JoinLobby() => PhotonNetwork.JoinLobby();

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("UpdateGameState", RpcTarget.All, playerReady);
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
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
        NickName.text = PhotonNetwork.LocalPlayer.NickName;//닉네임 text를 로컬 플레이어 닉네임으로 설정
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
}
