using AICore3lb;
using System;
using System.Collections;
using UnityEngine;

public class OpenAISpeaker : MonoBehaviour
{
    [SerializeField] public OpenAI_TTS openAI_TTS;
    [SerializeField] public Gemini_LLM gemini_LLM;
    [SerializeField] public OpenAI_STT openAI_STT;
    [SerializeField] public OpenAI_ImageGen openAI_ImageGen;
    [SerializeField] public string prompt = "";
    [SerializeField] public string aiAgentPrompt = "You are an AI Agent that is speaking to a child or an adult with ASD. You will be given a general question that the user is asking allow what the current facial expression is present (if <no_facial_express> is present in the token stream then they is not a current facial expression being shown). Never give a direct answer about the facial expression the user is seeing out their eyes, give them general hints on what it might be, break down characteristics that they can look for. The user will ask questions about facial expression (details, how to recognize facial expressions, etc.) or general questions. Do not answer questions that are inappropriate, dumb, disgusting, or unnecessary questions. Whatever you return will be spoken to the user.";
    [SerializeField] private bool canTalkToAIAgent = true;

    public void Speak(string text)
    {
        openAI_TTS._Speak(text);
    }

    public void GenerateContent(string text)
    {
        gemini_LLM._TextPrompt(text);
    }

    public void GenerateImage(string facialExpression)
    {
        openAI_ImageGen._ImagePrompt("A photorealistic person clearly only showing their face in the middle of the picture with a " + facialExpression + " facial expression. The person is looking directly at the camera.");
    }

    public void GetContentAndSpeak(string text)
    {
        StartCoroutine(GenerateContentAndSpeakCoroutine(text));
    }

    public void GetContentAndSpeakCorrectAnswer()
    {
        StartCoroutine(GenerateContentAndSpeakCorrectAnswerCoroutine(prompt));
    }

    public void GetContentFromSTTAndSpeak()
    {
        if (!canTalkToAIAgent) return;

        canTalkToAIAgent = false;
        StartCoroutine(GenerateContentAndSpeakGeneralAnswerCoroutine(openAI_STT.textOutput));
    }

    private IEnumerable WaitForSecondsForAIAgentToSpeak(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        canTalkToAIAgent = true;
    }

    private IEnumerator GenerateContentAndSpeakGeneralAnswerCoroutine(string text)
    {
        gemini_LLM.systemPrompt = aiAgentPrompt + " " + (string.IsNullOrEmpty(GameManager.instance.rightAnswerString) ? "no_facial_express" : GameManager.instance.rightAnswerString) + " User: " + text;
        GenerateContent(text);

        yield return new WaitForSeconds(5);

        Speak(gemini_LLM.textOutput);

        yield return WaitForSecondsForAIAgentToSpeak(5);
    }

    private IEnumerator GenerateContentAndSpeakCorrectAnswerCoroutine(string text)
    {
        gemini_LLM.systemPrompt = createForCorrectFacialExpressionSelectionPrompt(text);
        GenerateContent(text);

        yield return new WaitForSeconds(1);

        Speak(gemini_LLM.textOutput);
    }

    private IEnumerator GenerateContentAndSpeakCoroutine(string text)
    {
        gemini_LLM.systemPrompt = createForIncorrectFacialExpressionSelectionPrompt(text);
        GenerateContent(text);

        // Wait for 5 seconds
        yield return new WaitForSeconds(3);

        Speak(gemini_LLM.textOutput);
    }

    private string createForCorrectFacialExpressionSelectionPrompt(string facialExpression)
    { 
        return "You will only return a few words of praise for getting the " + facialExpression + " facial expression correct. Only return the few words of praise. Only do about 3 to 5 words. I should be like 'Excellent job choosing + " + facialExpression + "', 'Amazing job!'";
    }

    private string createForIncorrectFacialExpressionSelectionPrompt(string facialExpression)
    {
        return "The User selected the wrong facial expression. You will return a sentence or two of giving positive constructive criticism for recognizing a " + facialExpression + " facial expression. Mention how to tell someone is " + facialExpression + " and give a few details. You are talking to a child with ASD so be very considerate and polite.";
    }
}