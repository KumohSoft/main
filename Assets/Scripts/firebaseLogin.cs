using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;
using Firebase;
using Firebase.Extensions;
using Firebase.Firestore;
using System;
<<<<<<< Updated upstream
=======
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
>>>>>>> Stashed changes

public class firebaseLogin : MonoBehaviour
{
    private FirebaseAuth auth;
    private FirebaseUser user;
    private FirebaseFirestore db;
    private FirebaseApp app;
    private ListenerRegistration listenerRegistration;
    public RankingSystem rankingSystem;

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
    public string playerNickName;

    public static PlayerInfo playerInfo;//static으로 선언하고 networkManager에서 접근

    int Charactornum;
    int CharactorPrice;

    public Text priceText;
    public GameObject 이미보유함Panel;

    // Start is called before the first frame update
    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                var options = new AppOptions
                {
                    ApiKey = "AIzaSyCmvzr6Q02Y0TSqR7VOo4zF8usI8ON5o80",
                    AppId = "1:602506567531:android:cc99021ab4414c6036f62e",
                    ProjectId = "opensource-4bdc4"
                };
                app = FirebaseApp.Create(options, "AppInstance_" + Guid.NewGuid().ToString());
                auth = FirebaseAuth.GetAuth(app);
                db = FirebaseFirestore.GetInstance(app);
                networkManager = gameObject.GetComponent<networkManager>();
                playerInfo = new PlayerInfo();
            }
            else
            {
                Debug.LogError("Firebase dependency error: " + task.Result);
            }
        });
    }
    // Update is called once per frame
    void Update()
    {
        GameObject currentObject = EventSystem.current.currentSelectedGameObject;

        if (currentObject == email.gameObject && Input.GetKeyDown(KeyCode.Tab))
        {
            EventSystem.current.SetSelectedGameObject(password.gameObject);
        }
        else if (currentObject == signUpEmail.gameObject && Input.GetKeyDown(KeyCode.Tab))
        {
            EventSystem.current.SetSelectedGameObject(signUpPassword.gameObject);
        }
        else if (currentObject == signUpPassword.gameObject && Input.GetKeyDown(KeyCode.Tab))
        {
            EventSystem.current.SetSelectedGameObject(signUpPassword2.gameObject);
        }
        else if (currentObject == signUpPassword2.gameObject && Input.GetKeyDown(KeyCode.Tab))
        {
            EventSystem.current.SetSelectedGameObject(nickName.gameObject);
        }
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
        //SignUpPanel.SetActive(true);
    }

    public void Back()
    {
        //SignUpPanel.SetActive(false);
    }

    bool sig = false;
    private void getData()
    {
        listenerRegistration = db.Collection("PlayerInfos").Document(user.Email).Listen(snapshot =>
        {
            PlayerInfo temp = snapshot.ConvertTo<PlayerInfo>();
            playerInfo = temp;
            playerNickName = playerInfo.NickName;
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
        priceText.text = num.ToString()+"$";
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
                if(playerInfo.Item[Charactornum]==0)
                {
                    playerInfo.Item[Charactornum] = 1;
                    playerInfo.Gold -= CharactorPrice;
                    SavePlayerData();
                }
                else
                {
                    이미보유함Panel.SetActive(true);
                }
            }
            else
            {
                if(playerInfo.Character[Charactornum]==0)
                {
                    playerInfo.Character[Charactornum] = 1;
                    playerInfo.Gold -= CharactorPrice;
                    SavePlayerData();
                }
                else
                {
                    이미보유함Panel.SetActive(true);
                }
            }
        }
    }

    private void OnDestroy()
    {
        if (listenerRegistration != null)
        {
            listenerRegistration.Stop();
            listenerRegistration = null;
        }
    }
<<<<<<< Updated upstream
=======

    IEnumerator 오류textTime(string text)
    {
        오류text.text =text;
        오류text.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        오류text.gameObject.SetActive(false);
    }
    IEnumerator 로그인오류textTime()
    {
        로그인오류text.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        로그인오류text.gameObject.SetActive(false);
    }

    public event Action OnRankingDataFetched;

    public void FetchAndStoreRanking()
    {
        List<PlayerInfo> playerList = rankingSystem.playerList;
        db.Collection("PlayerInfos").GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsCanceled && !task.IsFaulted)
            {
                QuerySnapshot snapshot = task.Result;
                playerList.Clear();

                foreach (DocumentSnapshot document in snapshot.Documents)
                {
                    PlayerInfo player = document.ConvertTo<PlayerInfo>();
                    playerList.Add(player);
                }

                // 랭킹 정렬
                playerList.Sort((a, b) => b.Level.CompareTo(a.Level));

                Debug.Log("Ranking data fetched and sorted.");
            }
            else
            {
                Debug.LogError("Failed to fetch ranking data.");
            }
        });
    }

>>>>>>> Stashed changes
}