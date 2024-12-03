using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class DoorOpen : MonoBehaviourPun, IPunObservable
{
    public float moveSpeed = 2f; // Grid가 이동하는 속도
    public float targetYOffset = 50f; // 목표 y 좌표 상승량
    public Text messageText; // UI 텍스트 컴포넌트를 연결합니다.

    public GameObject 발전기TEXT;
    public Slider 발전기;
    float 게이지 = 0;
    public bool flag = true;
    bool 개인flag = false;

    private Transform gridTransform;
    InGameNetworkManager inGameNetworkManager;

    private void Start()
    {
        // Grid라는 자식 오브젝트를 찾습니다.
        gridTransform = transform.Find("Grid");
        if (gridTransform != null)
        {
            //Open();
        }
        else
        {
            Debug.LogError("Grid child not found!");
        }
        inGameNetworkManager = FindObjectOfType<InGameNetworkManager>();
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
                photonView.RPC("치즈삭제", RpcTarget.MasterClient, viewID);*/
                발전기TEXT.SetActive(true);
                발전기.gameObject.SetActive(true);
                발전기.value = 게이지;
                개인flag = true;
            }

        }
        print("더ㅚㅁ");
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("mouse") && flag)
        {
            PhotonView temp = other.gameObject.GetComponent<PhotonView>();
            if (temp != null && temp.IsMine)
            {
                발전기.value = 게이지;
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
                발전기TEXT.SetActive(false);
                발전기.gameObject.SetActive(false);
                개인flag = false;
            }

        }
        print("더ㅚㅁ");
    }

    public void 게이지증가()
    {
        photonView.RPC("게이지증가RPC", RpcTarget.MasterClient);
        발전기.value = 게이지;
        if (게이지 >= 발전기.maxValue)
        {
            발전기TEXT.SetActive(false);
            발전기.gameObject.SetActive(false);
            PhotonView photonView = gameObject.GetComponent<PhotonView>();
            photonView.RPC("OpenRPC", RpcTarget.All);
        }
    }

    [PunRPC]
    public void 게이지증가RPC()
    {
        게이지 += Time.deltaTime * 9;
    }
    [PunRPC]
    public void OpenRPC()
    {
        Open2();
    }
    public void Open()
    {
        ShowMessage("문이 열렸습니다!\n탈출하세요!");
        StartCoroutine(OpenDoorCoroutine());
        if (개인flag)
        {
            발전기TEXT.SetActive(false);
            발전기.gameObject.SetActive(false);
        }
    }

    public void Open2()
    {
        ShowMessage("문이 열렸습니다!\n탈출하세요!");
        게이지 = 0;
        StartCoroutine(OpenDoorCoroutine2());
        if (개인flag)
        {
            발전기TEXT.SetActive(false);
            발전기.gameObject.SetActive(false);
        }
        //inGameNetworkManager. <- 이 부분 완성 안됨
    }

    IEnumerator OpenDoorCoroutine()
    {
        float initialY = gridTransform.localPosition.y;
        float targetY = initialY + targetYOffset;

        // y 좌표를 천천히 상승
        while (gridTransform.localPosition.y < targetY)
        {
            Vector3 newPosition = gridTransform.localPosition;
            newPosition.y += moveSpeed * Time.deltaTime;
            newPosition.y = Mathf.Min(newPosition.y, targetY); // 목표 좌표를 초과하지 않도록 제한
            gridTransform.localPosition = newPosition;

            yield return null; // 다음 프레임까지 대기
        }
    }

    IEnumerator OpenDoorCoroutine2()
    {
        float initialY = gridTransform.localPosition.y;
        float targetY = initialY + targetYOffset;

        // 1. y 좌표를 천천히 상승
        while (gridTransform.localPosition.y < targetY)
        {
            Vector3 newPosition = gridTransform.localPosition;
            newPosition.y += moveSpeed * Time.deltaTime;
            newPosition.y = Mathf.Min(newPosition.y, targetY); // 목표 좌표를 초과하지 않도록 제한
            gridTransform.localPosition = newPosition;

            yield return null; // 다음 프레임까지 대기
        }

        yield return new WaitForSeconds(5f);

        // 2. y 좌표를 천천히 하강
        while (gridTransform.localPosition.y > initialY)
        {
            Vector3 newPosition = gridTransform.localPosition;
            newPosition.y -= moveSpeed * Time.deltaTime;
            newPosition.y = Mathf.Max(newPosition.y, initialY); // 시작 좌표를 아래로 초과하지 않도록 제한
            gridTransform.localPosition = newPosition;

            yield return null; // 다음 프레임까지 대기
        }
    }

    void ShowMessage(string message)
    {
        if (messageText != null)
        {
            messageText.text = message; // 메시지 설정
            messageText.gameObject.SetActive(true); // 메시지 표시

            // 일정 시간 후 메시지를 숨깁니다.
            StartCoroutine(HideMessageAfterDelay(3f)); // 3초 후 숨김
        }
    }

    IEnumerator HideMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        messageText.gameObject.SetActive(false);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(게이지);
        }
        else
        {
            게이지 = (float)stream.ReceiveNext();
        }
    }
}
