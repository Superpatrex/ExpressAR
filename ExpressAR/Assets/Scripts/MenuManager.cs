using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICore3lb;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject enviromentMenu;
    [SerializeField] private GameObject diffusionMenu;
    [SerializeField] private GameObject dalleImage;
    [SerializeField] private GameObject microphoneButton;
    [SerializeField] private AudioClip buttonClickedSound;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private string curSelectedMenu;
    [SerializeField] private OpenAI_TTS openAI_TTS;
    [SerializeField] private AudioClip incorrectSound;


    void Start() 
    {
        curSelectedMenu = "mainMenu";
    }

    void Update()
    {
        
    }

    public void OnEnvironmentButtonClicked()
    {
        curSelectedMenu = "enviromentMenu";
        Debug.LogWarning("Environmnet Button Clicked");
        turnOffMainMenu();
        GameManager.instance.StartGame();
    }

    public void OnDiffusionButtonClicked()
    {
        curSelectedMenu = "diffusionMenu";
        Debug.LogWarning("Diffusion Button Clicked");
        turnOffMainMenu();
        DiffusionModelGame.instance.StartGame();
    }

    public void OnMicrophoneButtonClicked()
    {
    }

    public void playButtonClicked()
    {
        audioSource.PlayOneShot(buttonClickedSound);
    }

    private void turnOffMainMenu()
    {
        mainMenu.SetActive(false);
    }

    public void turnOffDalleImage()
    {
        dalleImage.SetActive(false);
    }

    public void turnOnDalleImage()
    {
        dalleImage.SetActive(true);
    }

    public void goBack()
    {
        if (curSelectedMenu == "enviromentMenu")
        {
            enviromentMenu.SetActive(false);
            mainMenu.SetActive(true);
            openAI_TTS._Speak("Returning to the main menu");
        }
        else if (curSelectedMenu == "diffusionMenu")
        {
            diffusionMenu.SetActive(false);
            mainMenu.SetActive(true);
            openAI_TTS._Speak("Returning to the main menu");
        }
        else
        {
            audioSource.PlayOneShot(incorrectSound);
        }
    }
}
