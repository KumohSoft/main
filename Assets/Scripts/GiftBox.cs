using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GiftBox : MonoBehaviourPun, IPunObservable
{
    private InGameNetworkManager inGameNetworkManager;
    public GameObject ������TEXT;
    public Slider ������;
    float ������ = 0;
    bool flag = true;

    bool ����flag = false;
    // Start is called before the first frame update
    void Start()
    {
        inGameNetworkManager = FindObjectOfType<InGameNetworkManager>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("mouse") && flag)
        {
            PhotonView temp = other.gameObject.GetComponent<PhotonView>();
            if (temp != null && temp.IsMine)
            {
                /*PhotonView photonView = gameObject.GetComponent<PhotonView>();
                int viewID = photonView.ViewID;
                photonView.RPC("ġ�����", RpcTarget.MasterClient, viewID);*/
                ������TEXT.SetActive(true);
                ������.gameObject.SetActive(true);
                ������.value = ������;
                ����flag = true;
            }

        }
        print("���ʤ�");
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("mouse") && flag)
        {
            PhotonView temp = other.gameObject.GetComponent<PhotonView>();
            if (temp != null && temp.IsMine)
            {
                ������TEXT.SetActive(true);
                ������.gameObject.SetActive(true);
                ������.value = ������;
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {

        if (other.CompareTag("mouse") && flag)
        {
            PhotonView temp = other.gameObject.GetComponent<PhotonView>();
            if (temp != null && temp.IsMine)
            {
                ������TEXT.SetActive(false);
                ������.gameObject.SetActive(false);
                ����flag = false;
            }

        }
        print("���ʤ�");
    }

    [PunRPC]
    void ������ȹ��()
    {
        if (flag)
        {
            //inGameNetworkManager.ġ���();
            //PhotonView view = PhotonView.Find(viewID);
            photonView.RPC("�ƾ���DestroyRPC", RpcTarget.All);
            flag = false;
        }

    }
    [PunRPC]
    void �ƾ���DestroyRPC()
    {
        gameObject.SetActive(false);
    }

    public int ����������()
    {
        if(flag)
        {
            photonView.RPC("����������RPC", RpcTarget.MasterClient);
            ������.value = ������;
            ������TEXT.SetActive(true);
            ������.gameObject.SetActive(true);
            if (������ >= ������.maxValue)
            {
                ������TEXT.SetActive(false);
                ������.gameObject.SetActive(false);
                //PhotonView photonView = gameObject.GetComponent<PhotonView>();
                //int viewID = photonView.ViewID;
                photonView.RPC("������ȹ��", RpcTarget.MasterClient);
                flag = false;
                return 1;
            }
        }
        
        return 0;
    }
    [PunRPC]
    public void ����������RPC()
    {
        ������ += Time.fixedDeltaTime * 9;
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(������);
        }
        else
        {
            ������ = (float)stream.ReceiveNext();
        }
    }

    private void OnDestroy()
    {
        if (����flag)
        {
            if(gameObject!=null)
            {
                if(������TEXT.gameObject!=null)
                {
                    ������TEXT.SetActive(false);
                }
                if(������.gameObject!=null)
                {
                    ������.gameObject.SetActive(false);
                }
            }
            
        }
    }
}
