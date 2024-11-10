using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using Firebase;
using Firebase.Extensions;
using Firebase.Firestore;
using System.Linq;

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

    // Start is called before the first frame update
    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            app = FirebaseApp.DefaultInstance;
            auth = FirebaseAuth.DefaultInstance;
            db = FirebaseFirestore.DefaultInstance;
            networkManager = gameObject.GetComponent<networkManager>();
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
                NickName = nickName.text
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

    private void getData()
    {
        listenerRegistration = db.Collection("PlayerInfos").Document(user.Email).Listen(snapshot =>
        {
            PlayerInfo playerInfo = snapshot.ConvertTo<PlayerInfo>();
            string playerNickName = playerInfo.NickName;
            networkManager.Connect(playerNickName);
        });
    }
}
