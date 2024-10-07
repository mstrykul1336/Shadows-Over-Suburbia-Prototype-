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

    void InitializeHearts(int maxHearts)
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

    [PunRPC]
    public void UpdateHealthUI()
    {
        for (int i = 0; i < heartImages.Count; i++) // Use Count instead of Length
        {
            if (i < playerController.currentHearts)
            {
                heartImages[i].sprite = fullHeartSprite;  // Full heart if health exists
            }
            else
            {
                heartImages[i].sprite = brokenHeartSprite;  // Broken heart if no health
            }
    }
    }
}
