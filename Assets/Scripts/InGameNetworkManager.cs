using Photon.Pun;
using Photon.Pun.Demo.SlotRacer.Utils;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameNetworkManager : MonoBehaviourPunCallbacks
{
    private GameObject Mycharactor;
    void Awake()
    {
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
        Mycharactor = PhotonNetwork.Instantiate("Cat", new Vector3(-33.32f, 6.227f, -18.504f), Quaternion.identity);
    }
    void Start()
    {
    }

    void Update()
    {
        
    }
}
