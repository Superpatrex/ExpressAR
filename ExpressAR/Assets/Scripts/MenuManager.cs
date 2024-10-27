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
    [SerializeField] private bool canPressButton = true;


    void Start() 
    {
        curSelectedMenu = "mainMenu";
    }

    void Update()
    {
        
    }

    public void OnEnvironmentButtonClicked()
    {
        openAI_TTS._Speak("Environment Mode Selected");
        curSelectedMenu = "enviromentMenu";
        turnOffMainMenu();
        GameManager.instance.StartGame();
    }

    public void OnDiffusionButtonClicked()
    {
        openAI_TTS._Speak("Diffusion Mode Selected");
        curSelectedMenu = "diffusionMenu";
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
        if (!canPressButton) return;

        playButtonClicked();
        if (curSelectedMenu == "enviromentMenu")
        {
            GameManager.instance.EndGame();
            enviromentMenu.SetActive(false);
            mainMenu.SetActive(true);
            openAI_TTS._Speak("Returning to the main menu");
            curSelectedMenu = "mainMenu";
        }
        else if (curSelectedMenu == "diffusionMenu")
        {
            DiffusionModelGame.instance.EndGame();
            diffusionMenu.SetActive(false);
            mainMenu.SetActive(true);
            openAI_TTS._Speak("Returning to the main menu");
            curSelectedMenu = "mainMenu";
        }
        else
        {
            openAI_TTS._Speak("Sorry you cannot go back from the main menu");
        }

        StartCoroutine(WaitToPressButtonAfterDelay(3));
    }

    public IEnumerator WaitToPressButtonAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canPressButton = true;
    }
}
