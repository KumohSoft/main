using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TabKeyMaster : MonoBehaviour
{
    public InputField email;
    public InputField password;
    public InputField nickName;
    public InputField signUpEmail;
    public InputField signUpPassword;
    public InputField signUpPassword2;
    // Start is called before the first frame update
    void Start()
    {
        
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
}
