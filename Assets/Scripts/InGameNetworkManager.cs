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
using WebSocketSharp;

public class InGameNetworkManager : MonoBehaviourPunCallbacks
{
    public GameObject[] Player;
    public Text Time;
    private GameObject Mycharactor;
    public GameObject ���ٴ�;
    public Text ġ���Text;
    public DoorOpen DoorOpenscript;
    public GameObject Lobbyĵ����;
    public GameObject GameOverPanel;
    public Text �¸�����;
    public Text GameOverText;
    public Text ����ġText;
    public Text ���Text;

    private int ġ��� = 10;
    private int ���� = 2;
    private int count = 0;

    firebaseLogin firebasescript;

    private bool �������࿩��=true;
    public GameObject Ż��Obejct2;

    private string MasterNickName;

    int ����� = 0;
    private List<Vector3> spawnPositions = new List<Vector3> {
        new Vector3(-44.56f, 6.227f, -18.27f),
        new Vector3(-57.4f, 6.227f, -18.27f),
        new Vector3(-30f, 6.227f, -18.27f),
        new Vector3(-44.56f, 6.227f, -10) };

    public AudioSource CheeseSound;

    public GameObject ESCPanel;

    HashSet<string> ����set = new HashSet<string>();


    void Awake()
    {
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;

        /* if (networkManager.Mycharacter == 0)
         {
             Mycharactor = PhotonNetwork.Instantiate("��1", new Vector3(-0, 0, -0), Quaternion.identity);
         }

         else if (networkManager.Mycharacter == 1)
         {
             Mycharactor = PhotonNetwork.Instantiate("����", new Vector3(-0, 0, -0), Quaternion.identity);
         }*/


        if (PhotonNetwork.IsMasterClient)
        {
            
                if (networkManager.Mycharacter2 == 0)
                {
                    Mycharactor = PhotonNetwork.Instantiate("Cat1", new Vector3(-45, 153, 1), Quaternion.identity);//���Ŀ� ������� NickName�� ������ �ʰ� �����Ѵ�...
                }
                else if (networkManager.Mycharacter2 == 1)
                {
                    Mycharactor = PhotonNetwork.Instantiate("Tom1", new Vector3(-45, 153, 1), Quaternion.identity);
                }

        }
        else
        {
            
                if (networkManager.Mycharacter == 0)
                {
                    Mycharactor = PhotonNetwork.Instantiate("��1", new Vector3(-40, 153, 1), Quaternion.identity);
                }

                else if (networkManager.Mycharacter == 1)
                {
                    Mycharactor = PhotonNetwork.Instantiate("����1", new Vector3(-40, 153, 1), Quaternion.identity);
                }
        }
       
        photonView.RPC("LoadComplete", RpcTarget.MasterClient);
        ġ���Text.text = "ġ���:" + ġ���.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(photonView.IsMine && other.gameObject.CompareTag("mouse") && PhotonNetwork.IsMasterClient&& �������࿩��)
        {
            �������࿩�� = false;
            photonView.RPC("GameOverRPC", RpcTarget.All,0);
        }
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.Destroy(Mycharactor);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //photonView.RPC("GameOverRPC", RpcTarget.All, 2);
       //rint("�����͸���?" + otherPlayer.IsMasterClient);
        if (otherPlayer.NickName== MasterNickName)
        {
            MasterNickName = PhotonNetwork.MasterClient.NickName;
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("GameOverRPC", RpcTarget.All, 0);
            }
        }
        else
        {
            //���⼭ �� ���¸� ������Ʈ�Ѵ�.
            if (����set.Contains(otherPlayer.NickName))
            {
                ����set.Remove(otherPlayer.NickName);
                �����--;
                /*if (photonView.IsMine && PhotonNetwork.IsMasterClient && ����� == PhotonNetwork.PlayerList.Length - 1)
                {
                    photonView.RPC("GameOverRPC", RpcTarget.All, 1);
                }*/
            }
            else
            {
                if (photonView.IsMine && PhotonNetwork.IsMasterClient && ����� == PhotonNetwork.PlayerList.Length - 1)
                {
                    photonView.RPC("GameOverRPC", RpcTarget.All, 1);
                }
            }
            ����UpdateRPC(otherPlayer.NickName, 0);
        }

        // ���� ������ Ŭ���̾�Ʈ Ȯ��
        Debug.Log("Current Master Client: " + PhotonNetwork.MasterClient.NickName);
    }

    void Start()
    {
        firebasescript= FindObjectOfType<firebaseLogin>();
        MasterNickName = PhotonNetwork.MasterClient.NickName;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (ESCPanel.activeSelf)
            {
                ESCPanel.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                ESCPanel.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
            

        /*if (Input.GetKeyDown(KeyCode.K))
        {
            *//*Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            networkManager.Lobbyĵ����.SetActive(true);
            SceneManager.LoadScene("LobbyScene");*//*
            if(photonView.IsMine)
            {
                photonView.RPC("GameOverRPC", RpcTarget.All, 1);
            }
            
        }*/
    }


    public void ġ���()
    {
        photonView.RPC("ġ���RPC", RpcTarget.All);
    }
    [PunRPC]
    void ġ���RPC()
    {
        CheeseSound.Play();
        ġ���--;
        ġ���Text.text = "ġ���:" + ġ���.ToString();
        if (ġ��� == 0)
        {
            DoorOpenscript.Open();
            //SceneManager.LoadScene("LobbyScene");
            //���� �����ִ� ����;
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
                int ����� = UnityEngine.Random.Range(0, PhotonNetwork.PlayerList.Length);
                for (int i = 0; i < spawnPositions.Count; i++)
                {
                    Vector3 temp = spawnPositions[i];
                    int randomIndex = UnityEngine.Random.Range(i, spawnPositions.Count);//��ġ�� �̷��� UnityEngine.Random���� �������ָ� ��.
                    spawnPositions[i] = spawnPositions[randomIndex];
                    spawnPositions[randomIndex] = temp;
                }

                // �� �÷��̾�� ������ ���� ��ġ �Ҵ�
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
        Player[num - 1].transform.GetChild(2).GetComponent<Text>().text = "X"+����.ToString();
    }
    

    public void ����Update(string NickName,int count)
    {   
        photonView.RPC("����UpdateRPC", RpcTarget.All,NickName,count);
    }
    [PunRPC]
    void ����UpdateRPC(string NickName, int count)
    {
        
        for (int i=1; i<4; i++)
        {
            if(Player[i-1].activeSelf)
            {
                if (Player[i - 1].transform.GetChild(0).GetComponent<Text>().text == NickName)
                {
                    Player[i - 1].transform.GetChild(2).GetComponent<Text>().text = "X" + count.ToString();
                    break;
                }
            }
        }
        if(count==0)
        {
            ����set.Add(NickName);
        }
        if(count==2)
        {
            ����set.Remove(NickName);
            �����--;//���⼭ �����Ѵ�. �������
            print("�����:" + �����);
        }
        
    }

    IEnumerator CountStart(Vector3 spawnPositioni)
    {
        

        for (int i=10; i>=0; i--)
        {
            Time.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }
        Time.gameObject.SetActive(false);
        //���ٴ�.SetActive(false);
        print("�����̵���");
        ThirdPersonController temp = Mycharactor.GetComponent<ThirdPersonController>();
        temp.�����̵�(spawnPositioni,0);
    }

    [PunRPC]
    public void GameOverRPC(int num)
    {
        StartCoroutine(GameOver�ڷ�ƾ(num));
    }

    int Playercount = 0;
    [PunRPC]
    void ConfirmDestroy(int viewnum)
    {
        StartCoroutine(CheckDestroy(viewnum));
    }
    IEnumerator CheckDestroy(int viewnum)
    {
        while(true)
        {
            PhotonView targetView = PhotonView.Find(viewnum);

            if (targetView==null)
            {
                Playercount++;
                break;
            }
            yield return null;
        }
        if (Playercount == PhotonNetwork.PlayerList.Length)
        {
            Time.gameObject.SetActive(false);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            networkManager.Lobbyĵ����.SetActive(true);
            if (PhotonNetwork.IsMasterClient)
            {
                networkManager.�溹��();
            }

            SceneManager.LoadScene("LobbyScene");
        }
    }
    IEnumerator DestroyMyCharacter()
    {
        int viewID = Mycharactor.GetComponent<PhotonView>().ViewID;
        if (Mycharactor != null)
        {
            PhotonNetwork.Destroy(Mycharactor);
            Debug.Log("Destroying Mycharactor...");

            while (true)
            {
                if(Mycharactor == null)
                {
                    break;
                }
                else
                {
                    print("������");
                    yield return null; // �� ������ ���
                }
                
            }
        }

        photonView.RPC("ConfirmDestroy", RpcTarget.All, viewID);

    }
    IEnumerator GameOver�ڷ�ƾ(int num)
    {
        GameOverPanel.SetActive(true);
        if(num==1)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                �¸�����.text = "Cat Win!";
                firebasescript.����ġȹ��(30);
                firebasescript.���ȹ��(15);
                ����ġText.text = "���� ����ġ:" + 30.ToString();
                ���Text.text = "���� ���:" + 15.ToString();
            }
            else
            {
                �¸�����.text = "Cat Win!";
                int exp = 10 * (PhotonNetwork.PlayerList.Length - �����);
                int gold = 2 * (PhotonNetwork.PlayerList.Length - �����);
                if (PhotonNetwork.PlayerList.Length - 1 - ����� == 0)
                {
                    exp += 10;
                    gold += 5;
                }
                firebasescript.����ġȹ��(exp);
                firebasescript.���ȹ��(gold);
                ����ġText.text = "���� ����ġ:" + exp.ToString();
                ���Text.text = "���� ���:" + gold.ToString();
            }
            
        }
        else if(num==0)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                �¸�����.text = "Mouse Win!";
                firebasescript.����ġȹ��(10);
                firebasescript.���ȹ��(10);
                ����ġText.text = "���� ����ġ:" + 10.ToString();
                ���Text.text = "���� ���:" + 10.ToString();
            }
            else
            {
                �¸�����.text = "Mouse Win!";
                int exp = 10 * (PhotonNetwork.PlayerList.Length - �����);
                int gold = 2 * (PhotonNetwork.PlayerList.Length - �����);
                if (PhotonNetwork.PlayerList.Length - 1 - ����� == 0)
                {
                    exp += 10;
                    gold += 5;
                }
                firebasescript.����ġȹ��(exp);
                firebasescript.���ȹ��(gold);
                ����ġText.text = "���� ����ġ:" + exp.ToString();
                ���Text.text = "���� ���:" + gold.ToString();
            }
            
        }
        else if(num==2)
        {
            �¸�����.text = "��ȿ!";
            firebasescript.����ġȹ��(0);
            firebasescript.���ȹ��(0);
            ����ġText.text = "���� ����ġ:" + 0.ToString();
            ���Text.text = "���� ���:" + 0.ToString();
        }
        
        
        //Time.gameObject.SetActive(true);
        for (int i = 10; i >= 0; i--)
        {
            GameOverText.text = i.ToString()+"�� �� �κ�� ���ư��ϴ�.";
            yield return new WaitForSeconds(1f);
        }
        StartCoroutine(DestroyMyCharacter());
    }

    public void �����UP()
    {
        photonView.RPC("�����UPRPC", RpcTarget.All);
    }

    [PunRPC]
    void �����UPRPC()
    {
        �����++;
        if (photonView.IsMine && PhotonNetwork.IsMasterClient &&����� == PhotonNetwork.PlayerList.Length - 1)
        {
            photonView.RPC("GameOverRPC", RpcTarget.All,1);
        }
    }

    public void Ż��()
    {
        //���� �ݶ��̴��� Ȱ��ȭ�Ѵ�.
        photonView.RPC("Ż��RPC", RpcTarget.All);
    }

    public void Ż��RPC()
    {
        �����=0;
    }

    public void Ż�⹮Open()
    {
        Ż��Obejct2.SetActive(false);
    }
    public void Ż�⹮Close()
    {
        Ż��Obejct2.SetActive(true);
    }

    public void ������()
    {
        PhotonNetwork.Destroy(Mycharactor);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        networkManager.Lobbyĵ����.SetActive(true);
        networkManager NetworkManager= FindObjectOfType<networkManager>();
        NetworkManager.LeftRoom();
        SceneManager.LoadScene("LobbyScene");
    }
    public void ESCPanelOff()
    {
        ESCPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
