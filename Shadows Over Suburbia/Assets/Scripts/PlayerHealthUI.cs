using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlayerHealthUI : MonoBehaviourPun
{
    public GameObject heartPrefab;            
    public Transform heartContainer;          
    public Sprite fullHeartSprite;           
    public Sprite brokenHeartSprite;  
    public Sprite quarterHeartSprite;
    public Sprite threeQuarterHeartSprite; 
    public Sprite halfHeartSprite;     
    private PlayerController playerController;      
    private List<Image> heartImages = new List<Image>(); 

    void Start()
    {
        playerController = GetComponentInParent<PlayerController>();
        if (playerController != null && photonView.IsMine)
        {
            InitializeHearts(playerController.maxHearts); // Initialize hearts based on max health
            UpdateHealthUI(); // Update UI to match current health
        }
        // else
        // {
        //     //gameObject.SetActive(false);
        //    // Debug.Log("PlayerHealthUI disabled for non-local player.");
        // }
    }

    void InitializeHearts(float maxHearts)
    {
        // Clear any existing hearts in the heart container
        foreach (Transform child in heartContainer)
        {
            Destroy(child.gameObject);
        }
        heartImages.Clear();

        // Instantiate heart images based on maxHearts
        for (int i = 0; i < maxHearts; i++)
        {
            GameObject heart = Instantiate(heartPrefab, heartContainer);  // Create a new heart image
            Image heartImage = heart.GetComponent<Image>();               // Get the Image component
            heartImage.sprite = fullHeartSprite;                          // Set it to full heart initially
            heartImages.Add(heartImage);                                  // Add the image to the list
        }
    }

    // [PunRPC]
    // public void UpdateHealthUI()
    // {
    //     for (int i = 0; i < heartImages.Count; i++)
    //     {
    //         // Determine the current health relative to this heart index
    //         float heartStatus = playerController.currentHearts - i;

    //         if (heartStatus >= 1)
    //         {
    //             // Full heart
    //             heartImages[i].sprite = fullHeartSprite;
    //         }
    //         else if (heartStatus >= 0.75)
    //         {
    //             // 3/4th heart
    //             heartImages[i].sprite = threefourthheartSprite;
    //         }
    //         else if (heartStatus >= 0.5)
    //         {
    //             // 1/2th heart
    //             heartImages[i].sprite = halfheartSprite;
    //         }
    //         else if (heartStatus >= 0.25)
    //         {
    //             // 1/4th heart
    //             heartImages[i].sprite = onefourthheartSprite;
    //         }
    //         else
    //         {
    //             // Broken heart
    //             heartImages[i].sprite = brokenHeartSprite;
    //         }
    //     }
    // }

    [PunRPC]
    public void UpdateHealthUI()
    {
        if (!photonView.IsMine)
        {
            return;  // Only update the health UI for the local player
        }
        // Ensure we have the correct number of heart images
        while (heartImages.Count < playerController.maxHearts)
        {
            // Instantiate more heart images if needed
            GameObject newHeart = Instantiate(heartPrefab, heartContainer); 
            heartImages.Add(newHeart.GetComponent<Image>());
        }

        for (int i = 0; i < heartImages.Count; i++)
        {
            if (i < Mathf.FloorToInt(playerController.currentHearts))
            {
                heartImages[i].sprite = fullHeartSprite; // Full heart if health exists
            }
            else if (i == Mathf.FloorToInt(playerController.currentHearts))
            {
                heartImages[i].sprite = GetFractionalHeartSprite(playerController.currentHearts); // Handle fractional hearts
            }
            else
            {
                heartImages[i].sprite = brokenHeartSprite; // Broken heart if no health
            }
        }
    }

    // Function to handle fractional hearts
    private Sprite GetFractionalHeartSprite(float currentHearts)
    {
        float remainder = currentHearts - Mathf.Floor(currentHearts);

        if (remainder >= 0.75f)
            return threeQuarterHeartSprite;
        else if (remainder >= 0.5f)
            return halfHeartSprite;
        else if (remainder >= 0.25f)
            return quarterHeartSprite;
        
        return brokenHeartSprite; // Default to broken if none match
    }
}
