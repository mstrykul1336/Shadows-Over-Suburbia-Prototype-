using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Photon.Pun;

public class ChatBox : MonoBehaviourPun
{
    public TextMeshProUGUI chatLogText;
    public TMP_InputField chatInput;
    
    // instance
    public static ChatBox instance;
    

    void Awake()
    {
        instance = this;
    }

    // called when the player wants to send a message
    public void OnChatInputSend()
    {
        if (chatInput.text.Length > 0)
        {
            photonView.RPC("Log", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName, chatInput.text);
            chatInput.text = "";
        }
        EventSystem.current.SetSelectedGameObject(null);
        var localPlayer = FindObjectOfType<FirstPersonController>();
        if (localPlayer != null)
        {
            localPlayer.EnablePlayerControls();
        }
    }
    
    [PunRPC]
    void Log(string playerName, string message)
    {
        chatLogText.text += string.Format("<br>{0}:</b> {1}", playerName, message);
        chatLogText.rectTransform.sizeDelta = new Vector2(chatLogText.rectTransform.sizeDelta.x, chatLogText.mesh.bounds.size.y + 20); // ?
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (EventSystem.current.currentSelectedGameObject == chatInput.gameObject){
                OnChatInputSend();
            }
            else
            {
                EventSystem.current.SetSelectedGameObject(chatInput.gameObject);
                var localPlayer = FindObjectOfType<FirstPersonController>();
                if (localPlayer != null)
                {
                    localPlayer.DisablePlayerControls();
                }
            }
            
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (chatInput.gameObject.activeInHierarchy)
            {
                ResumeChat(); // Close the chat if it's already open
            }
            else
            {
                OpenChat(); // Open the chat
            }
        }
    }

    private void OpenChat()
    {
        chatInput.gameObject.SetActive(true);
        chatInput.Select(); // Select the input field
        chatInput.ActivateInputField(); // Activate the input field
        var localPlayer = FindObjectOfType<FirstPersonController>();
        if (localPlayer != null)
        {
            localPlayer.DisablePlayerControls();
        }
    }

    private void ResumeChat()
    {
        chatInput.gameObject.SetActive(false); // Hide the chat input
        EventSystem.current.SetSelectedGameObject(null); // Deselect any selected UI object
        var localPlayer = FindObjectOfType<FirstPersonController>();
        if (localPlayer != null)
        {
            localPlayer.EnablePlayerControls();
        }
    }

}