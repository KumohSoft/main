using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    public static DontDestroy instance;
    public GameObject GameManager;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            GameManager.SetActive(true);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
