using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Video;

public class WinUIManager : MonoBehaviour
{
    public TextMeshProUGUI winMessageText; // Reference to the TextMeshPro for the win message
    public TextMeshProUGUI playerNamesText; // Reference to the TextMeshPro for player names
    public GameObject winningscreen;
    public Image townwins;
    public Image cultwins;
    public Image oldmanwins;


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
        // Start the timer to hide the UI
        StartCoroutine(HideWinUIAfterDelay(15f));
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
}