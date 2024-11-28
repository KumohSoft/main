using JetBrains.Annotations;
using Photon.Pun;
using Photon.Pun.Demo.SlotRacer.Utils;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using StarterAssets;

public class InGameNetworkManager : MonoBehaviourPunCallbacks
{
    public GameObject[] Player;
    public Text Time;
    private GameObject Mycharactor;
    public GameObject 대기바닥;
    public Text 치즈개수Text;
    private int 치즈개수 = 4;
    private int 쥐목숨 = 2;
    private int count = 0;
    private List<Vector3> spawnPositions = new List<Vector3> {
        new Vector3(-44.56f, 6.227f, -18.27f),
        new Vector3(-57.4f, 6.227f, -18.27f),
        new Vector3(-30f, 6.227f, -18.27f),
        new Vector3(-44.56f, 6.227f, -10) };
    void Awake()
    {
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;

        /* if (networkManager.Mycharacter == 0)
         {
             Mycharactor = PhotonNetwork.Instantiate("쥐1", new Vector3(-0, 0, -0), Quaternion.identity);
         }

         else if (networkManager.Mycharacter == 1)
         {
             Mycharactor = PhotonNetwork.Instantiate("제리", new Vector3(-0, 0, -0), Quaternion.identity);
         }*/


        if (PhotonNetwork.IsMasterClient)
        {
            if (networkManager.Mycharacter2 == 0)
            {
                Mycharactor = PhotonNetwork.Instantiate("Cat1", new Vector3(-45, 87, 1), Quaternion.identity);//추후에 고양이의 NickName은 보이지 않게 설정한다...
            }
            else if (networkManager.Mycharacter2 == 1)
            {
                Mycharactor = PhotonNetwork.Instantiate("Tom1", new Vector3(-45, 87, 1), Quaternion.identity);
            }

        }
        else
        {
            if (networkManager.Mycharacter == 0)
            {
                Mycharactor = PhotonNetwork.Instantiate("쥐1", new Vector3(-40, 87, 1), Quaternion.identity);
            }

            else if (networkManager.Mycharacter == 1)
            {
                Mycharactor = PhotonNetwork.Instantiate("제리1", new Vector3(-40, 87, 1), Quaternion.identity);
            }
        }


        photonView.RPC("LoadComplete", RpcTarget.MasterClient);
        치즈개수Text.text = "치즈개수:" + 치즈개수.ToString();
    }
    void Start()
    {

    }

    void Update()
    {

    }


    public void 치즈감소()
    {
        photonView.RPC("치즈개수RPC", RpcTarget.All);
    }
    [PunRPC]
    void 치즈개수RPC()
    {
        치즈개수--;
        치즈개수Text.text = "치즈개수:" + 치즈개수.ToString();
        if (치즈개수 == 0)
        {
            //문을 열수있는 로직;
        }
    }

    [PunRPC]
    void LoadComplete()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            count++;
            if (PhotonNetwork.PlayerList.Length == count)
            {
                int 고양이 = UnityEngine.Random.Range(0, PhotonNetwork.PlayerList.Length);
                for (int i = 0; i < spawnPositions.Count; i++)
                {
                    Vector3 temp = spawnPositions[i];
                    int randomIndex = UnityEngine.Random.Range(i, spawnPositions.Count);//겹치면 이렇게 UnityEngine.Random으로 지정해주면 됨.
                    spawnPositions[i] = spawnPositions[randomIndex];
                    spawnPositions[randomIndex] = temp;
                }

                // 각 플레이어에게 고유한 스폰 위치 할당
                for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
                {
                    if(i>0)
                    {
                        photonView.RPC("PlayerSet", RpcTarget.All, i);
                    }
                    photonView.RPC("GameStart", PhotonNetwork.PlayerList[i], spawnPositions[i]);
                }
            }
        }
    }

    [PunRPC]
    void GameStart(Vector3 spawnPositioni)
    {
        Debug.Log($"spawnPositioni - X: {spawnPositioni.x}, Y: {spawnPositioni.y}, Z: {spawnPositioni.z}");

        StartCoroutine(CountStart(spawnPositioni));
        
    }

    [PunRPC]
    void PlayerSet(int num)
    {
        Player[num-1].SetActive(true);
        Player[num-1].transform.GetChild(0).GetComponent<Text>().text = PhotonNetwork.PlayerList[num].NickName;
        Player[num - 1].transform.GetChild(2).GetComponent<Text>().text = "X"+쥐목숨.ToString();
    }

    public void 쥐목숨Update(string NickName,int count)
    {
        photonView.RPC("쥐목숨UpdateRPC", RpcTarget.All,NickName,count);
    }
    [PunRPC]
    void 쥐목숨UpdateRPC(string NickName, int count)
    {
        for(int i=1; i<PhotonNetwork.PlayerList.Length; i++)
        {
            if (Player[i-1].transform.GetChild(0).GetComponent<Text>().text==NickName)
            {
                Player[i-1].transform.GetChild(2).GetComponent<Text>().text= "X" + count.ToString();
                break;
            }
        }
    }

    IEnumerator CountStart(Vector3 spawnPositioni)
    {
        for(int i=10; i>=0; i--)
        {
            Time.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }
        Time.gameObject.SetActive(false);
        //대기바닥.SetActive(false);
        print("순간이동함");
        ThirdPersonController temp = Mycharactor.GetComponent<ThirdPersonController>();
        temp.순간이동(spawnPositioni);
        
        
    }
}
