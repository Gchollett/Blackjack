using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    public GameObject dialogueBox;
    public GameObject continueButton;
    public HoverScript HoverController;
    private TextMeshProUGUI dialogueText;
    private Queue<string> sentences;
    public bool ended{get;set;}
    void Awake()
    {
        Instance = this;
        sentences = new Queue<string>();
        dialogueText = dialogueBox.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    public void StartDialogue(string[] dialogues,bool withButton=true){
        ended = false;
        dialogueBox.SetActive(true);
        continueButton.SetActive(withButton);
        foreach (string sentence in dialogues){
            sentences.Enqueue(sentence);
        }
        DisplayNextSentence();
    }

    public void DisplayNextSentence(){
        if(sentences.Count == 0){
            EndDialogue();
            return;
        }
        string next = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(WriteDialogue(next));
    }

    void EndDialogue(){
        ended = true;
        dialogueText.text = "";
        dialogueBox.SetActive(false);
        continueButton.SetActive(false);
        HoverController.EndHover();
    }

    IEnumerator WriteDialogue(string sentence){
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray()){
            dialogueText.text += letter;
            yield return null;
        }
    }

    public IEnumerator GoThroughDialogue(){
        while(sentences.Count > 0){
            DisplayNextSentence();
            yield return new WaitForSeconds(2);
        }
    }
}
