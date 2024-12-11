using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.InputSystem;
public class DoorOpen : MonoBehaviourPun, IPunObservable
{
    public float moveSpeed = 2f; // Grid�� �̵��ϴ� �ӵ�
    public float targetYOffset = 50f; // ��ǥ y ��ǥ ��·�
    public Text messageText; // UI �ؽ�Ʈ ������Ʈ�� �����մϴ�.

    public GameObject ������TEXT;
    public Slider ������;
    float ������ = 0;
    public bool flag = true;
    bool ����flag = false;

    public GameObject Ż��Object;
    public GameObject Ż��Obejct2;

    private Transform gridTransform;
    InGameNetworkManager inGameNetworkManager;

    private bool ������flag = false;

    [Header("Sound")]
    public AudioSource SwitchSound;
    private void Start()
    {
        // Grid��� �ڽ� ������Ʈ�� ã���ϴ�.
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
            if (temp != null && temp.IsMine)
            {
                ������TEXT.SetActive(true);
                ������.gameObject.SetActive(true);
                ������.value = ������;
                ����flag = true;
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

    public void ����������()
    {
        if(!������flag)
        {
            photonView.RPC("����������RPC", RpcTarget.MasterClient);
        }
        ������.value = ������;
        ������TEXT.SetActive(true);
        ������.gameObject.SetActive(true);
        if (������ >= ������.maxValue&&!������flag)
        {
            ������flag = true;
            ������TEXT.SetActive(false);
            ������.gameObject.SetActive(false);
            PhotonView photonView = gameObject.GetComponent<PhotonView>();
            photonView.RPC("OpenRPC", RpcTarget.All);
        }
    }

    [PunRPC]
    public void ����������RPC()
    {
        ������ += Time.fixedDeltaTime * 9;
    }
    [PunRPC]
    public void OpenRPC()
    {
        Open2();
    }
    public void Open()
    {

        ShowMessage("���� ���Ƚ��ϴ�!\nŻ���ϼ���!");
        StartCoroutine(OpenDoorCoroutine());
        if (����flag)
        {
            ������TEXT.SetActive(false);
            ������.gameObject.SetActive(false);
        }
    }

    public void Open2()
    {
        ShowMessage("���� ���Ƚ��ϴ�!\nŻ���ϼ���!");
        ������ = 0;
        SwitchSound.Play();
        StartCoroutine(OpenDoorCoroutine2());
        if (����flag)
        {
            ������TEXT.SetActive(false);
            ������.gameObject.SetActive(false);
        }
        //���⼭ �ݶ��̴��� Ȱ��ȭ?? �׸��� thirdperson���� Ʈ���ŷ� Ȯ��? �ٽ� ���ʵڿ� �ݶ��̴� ��Ȱ��ȭ �� ��ũ��Ʈ������ ����� 0�� ������ �浹�� �����ϰ� �ϰ� ���� �浹�� �ϰ��� ���ʵ��� �浹 �Ұ���. �׸��� ����� ������Ʈ �ϴ� �Լ��� ingamemanager�� ������.
        //inGameNetworkManager.Ż��();// <- �� �κ� �ϼ� �ȵ�
    }

    IEnumerator OpenDoorCoroutine()
    {
        float initialY = gridTransform.localPosition.y;
        float targetY = initialY + targetYOffset;

        // y ��ǥ�� õõ�� ���
        while (gridTransform.localPosition.y < targetY)
        {
            Vector3 newPosition = gridTransform.localPosition;
            newPosition.y += moveSpeed * Time.fixedDeltaTime;
            newPosition.y = Mathf.Min(newPosition.y, targetY); // ��ǥ ��ǥ�� �ʰ����� �ʵ��� ����
            gridTransform.localPosition = newPosition;

            yield return null; // ���� �����ӱ��� ���
        }
    }

    IEnumerator OpenDoorCoroutine2()
    {
        Ż��Object.SetActive(true);
        float initialY = gridTransform.localPosition.y;
        float targetY = initialY + targetYOffset;

        // 1. y ��ǥ�� õõ�� ���
        while (gridTransform.localPosition.y < targetY)
        {
            Vector3 newPosition = gridTransform.localPosition;
            newPosition.y += moveSpeed * Time.fixedDeltaTime;
            newPosition.y = Mathf.Min(newPosition.y, targetY); // ��ǥ ��ǥ�� �ʰ����� �ʵ��� ����
            gridTransform.localPosition = newPosition;

            yield return null; // ���� �����ӱ��� ���
        }

        yield return new WaitForSeconds(5f);

        // 2. y ��ǥ�� õõ�� �ϰ�
        while (gridTransform.localPosition.y > initialY)
        {
            Vector3 newPosition = gridTransform.localPosition;
            newPosition.y -= moveSpeed * Time.fixedDeltaTime;
            newPosition.y = Mathf.Max(newPosition.y, initialY); // ���� ��ǥ�� �Ʒ��� �ʰ����� �ʵ��� ����
            gridTransform.localPosition = newPosition;

            yield return null; // ���� �����ӱ��� ���
        }
        Ż��Object.SetActive(false);
        ������flag = false;
    }

    void ShowMessage(string message)
    {
        if (messageText != null)
        {
            messageText.text = message; // �޽��� ����
            messageText.gameObject.SetActive(true); // �޽��� ǥ��

            // ���� �ð� �� �޽����� ����ϴ�.
            StartCoroutine(HideMessageAfterDelay(3f)); // 3�� �� ����
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
            stream.SendNext(������);
        }
        else
        {
            ������ = (float)stream.ReceiveNext();
        }
    }
}
