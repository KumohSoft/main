using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScrolling : MonoBehaviour
{
    #region Inspector

    public Renderer renderer;
    public float speed = 0.2f;

    #endregion

    private void Update()
    {
        float move = Time.deltaTime * speed;
        renderer.material.mainTextureOffset += Vector2.right * move;
        renderer.material.mainTextureOffset += Vector2.up * move;

    }
}