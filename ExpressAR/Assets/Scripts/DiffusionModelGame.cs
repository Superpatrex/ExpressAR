using System.Collections.Generic;
using AICore3lb;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
public class DiffusionModelGame : MonoBehaviour
{
    public static DiffusionModelGame instance;
    public ImageItem [] imagesOfFacialExpressions;
    private Dictionary<string, Sprite> facialExpressionsDictionary;
    public static bool isReadyForGame = false;

    [SerializeField] public Image [] optionsMenus;

    [SerializeField] public GameObject [] buttons;
    [SerializeField] public OpenAI_TTS openAI_TTS;

    [SerializeField] public int correctOptionIndex = -1;
    [SerializeField] public GameObject totalMenu;

    private bool hasFaceBeenSelected = false;

    [SerializeField] public AudioClip successSound;
    [SerializeField] public AudioClip failureSound;
    [SerializeField] public AudioSource audioSource;

    [SerializeField] public string rightAnswerString = null;
    [SerializeField] public OpenAISpeaker openAI_speaker;
    private bool isSelectionInProgress = false;
    [SerializeField] public MenuManager menuManager;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (openAI_speaker == null)
        {
            throw new System.Exception("openAI_speaker does not exist");
        }
    }

    private void Start()
    {
        this.facialExpressionsDictionary = new Dictionary<string, Sprite>();

        foreach (ImageItem item in imagesOfFacialExpressions)
        {
            this.facialExpressionsDictionary.Add(item.name, item.image);
        }
    }

    public void Update()
    {
        if (isReadyForGame && !hasFaceBeenSelected)
        {
            totalMenu.SetActive(true);
            List<string> expressions = new List<string>(this.facialExpressionsDictionary.Keys);
            string expression = expressions[UnityEngine.Random.Range(0, expressions.Count)];

            if (this.facialExpressionsDictionary.ContainsKey(expression))
            {
                openAI_speaker.GenerateImage(expression);
                openAI_TTS._Speak("Beginning the game. Please be patient as the diffusion model generates the images");
                ShowButtons();
                ShowGameOptions(expression);
            }
            else
            {
                HideButtons();
            }
        }
    }

    public void ShowButtons()
    {
        foreach (GameObject button in buttons)
        {
            button.SetActive(true);
        }
    }

    public void HideButtons()
    {
        foreach (GameObject button in buttons)
        {
            button.SetActive(false);
        }
    }

    public void ShowGameOptions(string expression)
    {
        string correctOption = expression;
        rightAnswerString = correctOption;

        // Select a random option that is not the correct one
        string randomOption;
        List<string> keys = new List<string>(facialExpressionsDictionary.Keys);
        do
        {
            randomOption = keys[UnityEngine.Random.Range(0, keys.Count)];

        } while (randomOption == correctOption);

        int [] optionIndexSelection = giveMeZeroOrOne();

        optionsMenus[optionIndexSelection[0]].GetComponent<Image>().sprite = facialExpressionsDictionary[correctOption];
        optionsMenus[optionIndexSelection[1]].GetComponent<Image>().sprite = facialExpressionsDictionary[randomOption];

        buttons[optionIndexSelection[0]].GetComponentInChildren<TMP_Text>().text = char.ToUpper(correctOption[0]) + correctOption.Substring(1);
        buttons[optionIndexSelection[1]].GetComponentInChildren<TMP_Text>().text = char.ToUpper(randomOption[0]) + randomOption.Substring(1);

        correctOptionIndex = optionIndexSelection[0];
        Debug.Log("Correct Option Index: " + correctOptionIndex);
        ShowImages();
        hasFaceBeenSelected = true;
    }

    public void HideGameOptions()
    {
        foreach (Image option in optionsMenus)
        {
            option.gameObject.SetActive(false);
        }
    }

    public void ShowImages()
    {
        foreach (Image option in optionsMenus)
        {
            option.gameObject.SetActive(true);
        }
    }

    private int[] giveMeZeroOrOne()
    {
        int[] result = new int[2];
        result[0] = UnityEngine.Random.Range(0, 2);
        
        if (result[0] == 0)
        {
            result[1] = 1;
        }
        else
        {
            result[1] = 0;
        }

        return result;
    }

    public void StartGame()
    {
        isReadyForGame = true;
        totalMenu.SetActive(true);
    }

    public void EndGame()
    {
        isReadyForGame = false;
        HideButtons();
        HideGameOptions();
        totalMenu.SetActive(false);
    }

    public void TrySelection(TMP_Text text)
    {
        if (isSelectionInProgress) return;

        isSelectionInProgress = true;

        if (text.text.ToLower() == rightAnswerString.ToLower())
        {
            menuManager.turnOffDalleImage();
            audioSource.PlayOneShot(successSound);
            openAI_speaker.GetContentAndSpeakCorrectAnswer();
            HideButtons();
            HideGameOptions();
            correctOptionIndex = -1;
            rightAnswerString = null;
            StartCoroutine(ResetHasFaceBeenSelectedAfterDelay(5));
        }
        else
        {
            audioSource.PlayOneShot(failureSound);
            openAI_speaker.GetContentAndSpeak(rightAnswerString);
        }
        
        StartCoroutine(ResetSelectionFlagAfterDelay(15));
    }

    private IEnumerator ResetSelectionFlagAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isSelectionInProgress = false;
    }

    private IEnumerator ResetHasFaceBeenSelectedAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        hasFaceBeenSelected = false;
    }
}