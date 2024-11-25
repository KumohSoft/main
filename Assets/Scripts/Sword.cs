using Photon.Pun;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        ThirdPersonController parentScript = GetComponentInParent<ThirdPersonController>();
        if (other.CompareTag("mouse"))
        {
            print("맞음");
            PhotonView otherPhotonView = other.GetComponent<PhotonView>();

            if (otherPhotonView != null && otherPhotonView != parentScript.photonView) // 자기 자신을 제외
            {
                if (parentScript.photonView.IsMine && parentScript.isAttacking && parentScript.live)
                {
                    ThirdPersonController temp = other.GetComponent<ThirdPersonController>();
                    if (temp.live)
                    {
                        temp.공격받음();
                        //gmObject.Send(parentScript.NickName, temp.NickName);
                    }
                }
            }
        }
    }
}
