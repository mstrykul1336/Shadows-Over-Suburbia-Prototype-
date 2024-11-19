using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Video;
using Photon.Pun;
using Photon.Realtime;


public class WinUIManager : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI winMessageText; // Reference to the TextMeshPro for the win message
    public TextMeshProUGUI playerNamesText; // Reference to the TextMeshPro for player names
    public GameObject winningscreen;
    public Image townwins;
    public Image cultwins;
    public Image oldmanwins;

    public Transform roleImagesContainer; 
    public GameObject roleImagePrefab;
    public Sprite mayorSprite;
    public Sprite assistantSprite;
    public Sprite oldManSprite;
    public Sprite clairvoyantSprite;
    public Sprite bakerSprite;
    public Sprite villagerSprite;
    public Sprite DetectiveSprite;
    public Sprite medicSprite;

    public GameObject playerNamePrefab;     // Prefab for player names
    public Transform playerNamesContainer;


    public void ShowWinUI(string winningSide, string[] playerNames)
    {
        // Set the win message
        winMessageText.text = $"{winningSide} has won!";

        // Join player names into a single string
        playerNamesText.text = "" + string.Join(", ", playerNames);

        // Activate the win UI
        winningscreen.SetActive(true);
        if (winningSide == "Helpful")
        {
            PlayWinVideo(townwins);
        }
        else if (winningSide == "Destructive")
        {
            PlayWinVideo(cultwins);
        }
        else if (winningSide == "Neutral")
        {
            PlayWinVideo(oldmanwins);
        }

        foreach (string playerName in playerNames)
        {
            string playerRole = GetPlayerRoleFromProperties(playerName);
            if (!string.IsNullOrEmpty(playerRole))
            {
                Debug.Log($"Player: {playerName}, Role: {playerRole}");
                Sprite roleSprite = GetRoleSprite(playerRole);
                if (roleSprite != null)
                {
                    Debug.Log($"Found sprite for role: {playerRole}");
                    GameObject roleImageGO = Instantiate(roleImagePrefab, roleImagesContainer);
                    Image roleImage = roleImageGO.GetComponent<Image>();
                    roleImage.sprite = roleSprite;
                }

                GameObject playerNameGO = Instantiate(playerNamePrefab, playerNamesContainer);
                TMP_Text playerNameText = playerNameGO.GetComponent<TMP_Text>();
                playerNameText.text = playerName;
            }
        }


        // Start the timer to hide the UI
        StartCoroutine(HideWinUIAfterDelay(30f));
    }

    private System.Collections.IEnumerator HideWinUIAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        winningscreen.SetActive(false);
    }

    private void PlayWinVideo(Image image)
    {
        // Stop any currently playing video
        townwins.gameObject.SetActive(false);
        cultwins.gameObject.SetActive(false);
        oldmanwins.gameObject.SetActive(false);

        // Play the selected video
        image.gameObject.SetActive(true);
    }

    private string GetPlayerRoleFromProperties(string playerName)
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.NickName == playerName && player.CustomProperties.ContainsKey("Role"))
            {
                return player.CustomProperties["Role"] as string;
            }
        }
        return null;
    }

    private Sprite GetRoleSprite(string role)
    {
        // Match roles to sprites
        switch (role)
        {
            case "Mayor": return mayorSprite;
            case "Assistant": return assistantSprite;
            case "Old Man": return oldManSprite;
            case "Clairvoyant": return clairvoyantSprite;
            case "Baker": return bakerSprite;
            case "Villager": return villagerSprite;
            case "Detective": return DetectiveSprite;
            case "Medic": return medicSprite;
            default: return null;
        }
    }
}