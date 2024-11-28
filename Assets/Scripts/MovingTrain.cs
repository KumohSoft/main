using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTrain : MonoBehaviour
{
    public float speed = 0.1f; // 이동 속도

    void Update()
    {
        transform.position += Vector3.right * speed * Time.deltaTime;

        if (transform.position.x >= 0f)
        {
            transform.position = new Vector3(-150f, transform.position.y, transform.position.z);
        }
    }
}
