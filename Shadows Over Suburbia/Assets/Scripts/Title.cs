using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    public GameObject rolescanvas;
    public GameObject controlscanvas;
    public GameObject howtocanvas;
    public GameObject maincanvas;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPlayButton(){
        SceneManager.LoadScene(sceneBuildIndex:1);
    }

    public void OnRolesButton()
    {
     
        rolescanvas.SetActive(true);
        maincanvas.SetActive(false);

        
    }

    public void OnControlsButton()
    {
        controlscanvas.SetActive(true);
        maincanvas.SetActive(false);
    }

    public void OnExitControlButton()
    {
        controlscanvas.SetActive(false);
        maincanvas.SetActive(true);
    }

    public void OnExitRolesButton()
    {
        rolescanvas.SetActive(false);
        maincanvas.SetActive(true);
    }

    public void OnHowToButton()
    {
     
        howtocanvas.SetActive(true);
        maincanvas.SetActive(false);
        
    }

    public void OnExitHowtoButton()
    {
        howtocanvas.SetActive(false);
        maincanvas.SetActive(true);
    }
}
