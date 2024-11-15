using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using Firebase;
using Firebase.Extensions;
using Firebase.Firestore;
using System.Linq;
using static Cinemachine.DocumentationSortingAttribute;
using UnityEngine.TextCore.Text;
using static UnityEditor.Progress;

public class firebaseLogin : MonoBehaviour
{
    private FirebaseAuth auth;
    private FirebaseUser user;
    private FirebaseFirestore db;
    private FirebaseApp app;
    private ListenerRegistration listenerRegistration;

    public InputField nickName;
    public InputField signUpEmail;
    public InputField signUpPassword;
    public InputField signUpPassword2;
    public GameObject SignUpPanel;

    public InputField email;
    public InputField password;

    private networkManager networkManager;

    public Text gold;//일단 임시로 여기 나중에 스크립트 분리
    public Text level;

    public int Level;
    public int Gold;

    public static PlayerInfo playerInfo;//static으로 선언하고 networkManager에서 접근

    int Charactornum;
    int CharactorPrice;

    // Start is called before the first frame update
    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            app = FirebaseApp.DefaultInstance;
            auth = FirebaseAuth.DefaultInstance;
            db = FirebaseFirestore.DefaultInstance;
            networkManager = gameObject.GetComponent<networkManager>();
            playerInfo = new PlayerInfo();
        });
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Create()
    {
        auth.CreateUserWithEmailAndPasswordAsync(signUpEmail.text, signUpPassword.text).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.Log("회원가입 취소");
            }

            if (task.IsFaulted)
            {
                Debug.Log("회원가입 실패");

            }
            AuthResult authResult = task.Result;
            FirebaseUser newuser = authResult.User;


            DocumentReference newdata = db.Collection("PlayerInfos").Document(newuser.Email);

            PlayerInfo newPlayer = new PlayerInfo
            {
                NickName = nickName.text,
                Gold = 0,
                WinCount = 0,
                LoseCount = 0,
                Level = 1,
                Character = new int[4],
                Item = new int[4]
            };
            newdata.SetAsync(newPlayer).ContinueWithOnMainThread(task =>
            {

            });
            SignUpPanel.SetActive(false);
        });
    }

    public void Login()
    {
        auth.SignInWithEmailAndPasswordAsync(email.text, password.text).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.Log("로그인 취소");
            }
            if (task.IsFaulted)
            {
                Debug.Log("로그인 실패");
                //로그인오류.SetActive(true);
            }
            AuthResult authResult = task.Result;
            user = authResult.User;
            Debug.Log("로그인 성공");

            getData();

        });
    }

    public void SignUp()
    {
        SignUpPanel.SetActive(true);
    }

    public void Back()
    {
        SignUpPanel.SetActive(false);
    }

    bool sig = false;
    private void getData()
    {
        listenerRegistration = db.Collection("PlayerInfos").Document(user.Email).Listen(snapshot =>
        {
            PlayerInfo temp = snapshot.ConvertTo<PlayerInfo>();
            playerInfo = temp;
            string playerNickName = playerInfo.NickName;
            level.text = "Level:" + playerInfo.Level.ToString();
            gold.text = playerInfo.Gold.ToString() + "$";
            if (!sig)
            {
                sig = true;
                networkManager.Connect(playerNickName);
            }

        });
    }

    private void SavePlayerData()
    {
        if (user != null)
        {
            DocumentReference docRef = db.Collection("PlayerInfos").Document(user.Email);

            // PlayerInfo 객체 전체를 Firestore에 저장
            docRef.SetAsync(playerInfo).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("Player data updated in Firestore");
                }
                else
                {
                    Debug.LogError("Failed to update player data");
                }
            });
        }
        else
        {
            Debug.LogError("No user is logged in");
        }
    }

    public void GoldPlus()
    {
        playerInfo.Gold++;
        SavePlayerData();
    }
    public void LevelUp()
    {
        playerInfo.Level++;
        SavePlayerData();
    }
    public void Click상점Charactor(int num)
    {
        CharactorPrice = num;
    }
    public void Click상점Charactor2(int num)
    {
        Charactornum = num;
    }

    bool skillSig = false;
    public void Skill구매(int num)
    {
        if(num==1)
        {
            skillSig = true;
        }
        else
        {
            skillSig = false;
        }
    }
    public void 구매()
    {
        if(playerInfo.Gold< CharactorPrice)
        {
            //돈이 부족함
        }
        else
        {
            if(skillSig)
            {
                playerInfo.Item[Charactornum] = 1;
            }
            else
            {
                playerInfo.Character[Charactornum] = 1;
            }
            
            playerInfo.Gold -= CharactorPrice;
            SavePlayerData();
        }
    }
}