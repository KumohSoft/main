using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chees : MonoBehaviourPun, IPunObservable
{
    private InGameNetworkManager inGameNetworkManager;
    public GameObject ������TEXT;
    public Slider ������;
    float ������=0;
    bool flag=true;

    bool ����flag = false;
    // Start is called before the first frame update
    void Start()
    {
        inGameNetworkManager=FindObjectOfType<InGameNetworkManager>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("mouse")&&flag)
        {
            PhotonView temp = other.gameObject.GetComponent<PhotonView>();
            if(temp != null && temp.IsMine)
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
            if (temp!=null&&temp.IsMine)
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
    void ġ�����(int viewID)
    {
        if(flag)
        {
            inGameNetworkManager.ġ���();
            //PhotonView view = PhotonView.Find(viewID);
            photonView.RPC("ġ��DestoryRPC", RpcTarget.All);
            flag = false;
        }
        
    }

    [PunRPC]
    void ġ��DestoryRPC()
    {
        gameObject.SetActive(false);
    }

    public void ����������()
    {
        photonView.RPC("����������RPC", RpcTarget.MasterClient);
        ������.value = ������;
        ������TEXT.SetActive(true);
        ������.gameObject.SetActive(true);
        if (������>=������.maxValue)
        {
            ������TEXT.SetActive(false);
            ������.gameObject.SetActive(false);
            PhotonView photonView = gameObject.GetComponent<PhotonView>();
            int viewID = photonView.ViewID;
            photonView.RPC("ġ�����", RpcTarget.MasterClient, viewID);
        }
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
        if(����flag)
        {
            if(gameObject!=null)
            {
                if(������TEXT!=null)
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
