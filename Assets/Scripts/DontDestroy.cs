using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    private static DontDestroy instance;
    public GameObject GameManager;
    public GameObject RoomPaenl;

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
            RoomPaenl.SetActive(true);
            Destroy(gameObject);
        }
    }
}
