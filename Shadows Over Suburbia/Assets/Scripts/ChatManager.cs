using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviourPun
{
    //public static ChatManager Instance { get; private set; } 
    public TMP_InputField inputField;
    public TextMeshProUGUI chatDisplay;
    private string chatHistory = "";
    private GameManager gameManager; // Reference to GameManager
    public Button sendButton; // Reference to the Send button
    public Button toggleButton; // Reference to the toggle (minimize) button
    public GameObject chatPanel; 
    private bool isChatVisible = true; // Track chat visibility
    public Button reopenButton; 
    private PlayerController playerController;
    public static ChatManager Instance;
   // private PhotonView photonView;


    private void Awake()
    {
        // Ensure that there's only one instance of ChatManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this instance alive across scenes\
            //photonView = GetComponentInParent<PhotonView>();
            Debug.Log("ChatManager instance created.");
        }
        else
        {
            Debug.Log("Duplicate ChatManager instance destroyed.");
            Destroy(gameObject); // Destroy duplicates
        }
    }

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        // playerController = GetComponentInParent<PlayerController>();
        // if (!photonView.IsMine)
        // {
        //     // If this is not the local player, disable the chat UI
        //     chatPanel.SetActive(false);
        //     return;
        // }

        sendButton.onClick.AddListener(SendMessage); // Link the Send button to SendMessage
        toggleButton.onClick.AddListener(ToggleChatVisibility); 
        reopenButton.onClick.AddListener(ReopenChat); // Link TMP button to ReopenChat
        reopenButton.gameObject.SetActive(false); // Hide reopen button initially
        chatPanel.SetActive(true);
        
    }

    public void DisplayLocalMessage(string message)
    {
        // if (!photonView.IsMine) return;
        chatHistory += $"[Local]: {message}\n";
        chatDisplay.text = chatHistory;
    }

    public void SendMessage()
    {
        PlayerController localPlayerController = FindObjectOfType<PlayerController>();
        if (playerController != null && !playerController.canChat)
        {
            DisplayLocalMessage("You cannot chat right now.");
            return; // Prevent sending the message
        }
        if (gameManager.isNightCycle)
        {
            DisplayLocalMessage("You cannot chat at night!");
            return; // Prevent sending the global message
        }

        string message = inputField.text;
        if (!string.IsNullOrEmpty(message) && localPlayerController.photonView != null)
        {
            Debug.Log($"Sending message from player {PhotonNetwork.LocalPlayer.NickName}: {message}");
            localPlayerController.photonView.RPC("ReceiveGlobalMessage", RpcTarget.All, message, PhotonNetwork.LocalPlayer.NickName);
            inputField.text = ""; // Clear input after sending
        }
        else
        {
            Debug.Log("Message is empty or PhotonView is null, not sending.");
        }
        inputField.text = "";
    }

    // [PunRPC]
    // public void ReceiveGlobalMessage(string message, string senderName)
    // {
    //     if (!chatPanel.activeSelf) chatPanel.SetActive(true);
    //     chatHistory += $"{senderName}: {message}\n";
    //     chatDisplay.text = chatHistory;
    // }

    public void DisplayRemoteMessage(string message, string senderName)
    {
        if (!chatPanel.activeSelf) chatPanel.SetActive(true);
        chatHistory += $"{senderName}: {message}\n";
        chatDisplay.text = chatHistory;
    }

    public void ToggleChatVisibility()
    {
        // if (!photonView.IsMine) return;
        isChatVisible = !isChatVisible;
        chatPanel.SetActive(isChatVisible);
        reopenButton.gameObject.SetActive(!isChatVisible);
    }
    public void ReopenChat()
    {
        // if (!photonView.IsMine) return;
        isChatVisible = true;
        chatPanel.SetActive(true);
        reopenButton.gameObject.SetActive(false); // Hide reopen button
    }
}