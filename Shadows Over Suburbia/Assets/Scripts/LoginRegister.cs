using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine.Events;

public class LoginRegister : MonoBehaviour
{

    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TextMeshProUGUI displayText;
    public UnityEvent onLoggedIn;
    public static LoginRegister instance;
    public GameObject mainScreen;
    [HideInInspector]
    public string playFabId;
  
    // Start is called before the first frame update

    void Awake () { instance = this; }
    public void OnRegister()
    {

        RegisterPlayFabUserRequest registerRequest = new RegisterPlayFabUserRequest
    {
        Username = usernameInput.text,
        DisplayName = usernameInput.text,
        Password = passwordInput.text,
        RequireBothUsernameAndEmail = false
    };


    PlayFabClientAPI.RegisterPlayFabUser(registerRequest,
        result =>
        {
           SetDisplayText(result.PlayFabId, Color.green);
        },
        error =>
        {
            SetDisplayText(error.ErrorMessage, Color.red);
        }
    );
    }


    public void OnLoginButton()
    {
        LoginWithPlayFabRequest loginRequest = new LoginWithPlayFabRequest
        {
            Username = usernameInput.text,
            Password = passwordInput.text
        };

        PlayFabClientAPI.LoginWithPlayFab(loginRequest,
            result =>
            {
                playFabId = result.PlayFabId;
                SetDisplayText("Logged in as: " + result.PlayFabId, Color.green);
                if(onLoggedIn != null)
                    onLoggedIn.Invoke();
            },
            error => SetDisplayText(error.ErrorMessage, Color.red)
        );
        mainScreen.SetActive(true);


    }

    public void SetDisplayText (string text, Color color)
    {
        displayText.text = text;
        displayText.color = color;
    }
    // Update is called once per frame
    void Update()
    {
        
    }

}
