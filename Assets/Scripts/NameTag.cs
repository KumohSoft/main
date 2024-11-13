using UnityEngine;
using TMPro;
using Photon.Pun;

public class NameTag : MonoBehaviourPunCallbacks
{
    private string playerName; // 닉네임을 저장할 변수

    private TextMesh textMesh;

    private void Awake()
    {
        textMesh = GetComponent<TextMesh>();
    }

    private void Start()
    {
        if(photonView.IsMine)
        {
            photonView.RPC("SetNickName", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName);
        }
        
    }

    [PunRPC]
    void SetNickName(string name)
    {
        textMesh.text = name;
    }

    private void Update()
    {
        Vector3 lookDirection = Camera.main.transform.forward; // 카메라의 바라보는 방향
        lookDirection.y = 0; // Y축 방향을 0으로 설정하여 수평으로만 회전
        transform.rotation = Quaternion.LookRotation(lookDirection); // 새로운 방향으로 회전
    }
}
