using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;


public class Intro : MonoBehaviour
{
    public class Subtitle
    {
        public float startTime;
        public float endTime;
        public string text;
    }
    public AudioSource wiggleBlorpAudio;
    public Image SubtitleImage;
    public TextMeshProUGUI subtitleText;
    public List<Subtitle> subtitles = new List<Subtitle>();
    public int currentSubtitleIndex = 0;
    private bool allSubtitlesDone;
    public GameObject tutorialScreen1;
    public GameObject tutorialScreen2;
    public GameObject tutorialScreen3;
    public GameObject tutorialScreen4;
    public GameObject tutorialScreen5;
    public GameObject tutorialScreen6;
    public Image wiggleblorpImage;
    public Button tutorialButton;

    // Start is called before the first frame update
    void Start()
    {
        currentSubtitleIndex = 0;
        subtitles.Add(new Subtitle{ startTime = 0.0f, endTime = 4.3f, text = "Ah, greetings, little mortals. I am WiggleBlorp."});
        subtitles.Add(new Subtitle{ startTime = 4.3f, endTime = 9.0f, text = "Perhaps you’ve heard whispers of me—perhaps not. No matter."});
        subtitles.Add(new Subtitle{ startTime = 9.0f, endTime = 15.0f, text = "My presence in Nowhere, Suburbia, is as ancient as the soil beneath your quaint little town."});
        subtitles.Add(new Subtitle{ startTime = 15.0f, endTime = 18.0f, text = "The peace here? Ha! An illusion."});
        subtitles.Add(new Subtitle{ startTime = 18.0f, endTime = 23.0f, text = "Beneath the gardens and smiles lies a power waiting to be claimed. "});
        subtitles.Add(new Subtitle{ startTime = 23.0f, endTime = 28.0f, text = "And now, someone seeks to wake it... to wake me. "});
        subtitles.Add(new Subtitle{ startTime = 28.0f, endTime = 32.0f, text = "Is it you? Are you here to stop them?"});
        subtitles.Add(new Subtitle{ startTime = 32.0f, endTime = 35.0f, text = "Or are you here to help them?"});
        subtitles.Add(new Subtitle{ startTime = 35.0f, endTime = 40.0f, text = "No matter your choice, every step you take brings me closer."});
        subtitles.Add(new Subtitle{ startTime = 40.0f, endTime = 45.0f, text = "Play your part well. I’ll be watching."});
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnSkipButton();
        }
        if(allSubtitlesDone)
        {
            return;
        }
        if (currentSubtitleIndex >= subtitles.Count)
        {
            subtitleText.gameObject.SetActive(false);
            SubtitleImage.gameObject.SetActive(false);
            tutorialButton.gameObject.SetActive(true);
            //Debug.Log("wprlomg");
            allSubtitlesDone = true;
            return;
        }

        // Get the current playback time of the audio
        float currentTime = wiggleBlorpAudio.time;

        if (currentSubtitleIndex < subtitles.Count)
        {
            // Show the current subtitle if the time is right
            if (currentTime >= subtitles[currentSubtitleIndex].startTime &&
                currentTime <= subtitles[currentSubtitleIndex].endTime)
            {
                subtitleText.text = subtitles[currentSubtitleIndex].text;
            }
            // Move to the next subtitle if the current one has ended
            else if (currentTime > subtitles[currentSubtitleIndex].endTime)
            {
                currentSubtitleIndex++;
                subtitleText.text = ""; // Clear the subtitle when none is active
            }
        }

    }

    public void OnSkipButton(){
        SceneManager.LoadScene(sceneBuildIndex:1);
    }
    public void OnTutorialButton(){
        tutorialScreen1.SetActive(true);
        wiggleblorpImage.gameObject.SetActive(false);
        tutorialButton.gameObject.SetActive(false);
        
    }
    public void OnTutorialStep1Button(){
        tutorialScreen2.SetActive(true);
        tutorialScreen1.SetActive(false);
        
    }

    public void OnTutorialStep2Button(){
        tutorialScreen3.SetActive(true);
        tutorialScreen2.SetActive(false);
        
    }
    public void OnTutorialStep3Button(){
        tutorialScreen4.SetActive(true);
        tutorialScreen3.SetActive(false);
        
    }
    public void OnTutorialStep4Button(){
        tutorialScreen5.SetActive(true);
        tutorialScreen4.SetActive(false);
        
    }
    public void OnTutorialStep5Button(){
        tutorialScreen6.SetActive(true);
        tutorialScreen5.SetActive(false);
        
    }
}
