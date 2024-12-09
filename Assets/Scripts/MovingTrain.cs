using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MovingTrain : MonoBehaviourPun
{
    public float speed = 0.1f; // �̵� �ӵ�

    void Update()
    {
        if(photonView.IsMine)
        {
            transform.position += Vector3.right * speed * Time.deltaTime;

            if (transform.position.x >= 0f)
            {
                transform.position = new Vector3(-150f, transform.position.y, transform.position.z);
            }
        }
       
    }
}
