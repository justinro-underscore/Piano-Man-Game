using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueTextRunner : MonoBehaviour 
{
    public TMP_Text nameText;
    public TMP_Text choicesText;
    public TMP_Text dialogueText;

    public Camera mainCamera;
    public GameObject choicesBox;
    public GameObject forwardText;

    public delegate void NodeSelectedHandler( int index );
    public event NodeSelectedHandler onSelectedChoice;

    private string patronName;
    private string patronNameColor;
    private List<string> choices;
    private string dialogue;

    [Range(0, 0.5f)]
    [SerializeField]
    private float dialogueTypingSpeed = 0.1f;
    [Range(0, 0.5f)]
    [SerializeField]
    private float choicesTypingSpeed = 0.02f;
    [Range(0, 1)]
    [SerializeField]
    private float blinkSpeed = 0.5f;

    private bool skip = false;
    private bool ready = false;

    private struct TextSnippet {
        public string text;
        public bool tag;

        public TextSnippet(string text, bool tag) {
            this.text = text;
            this.tag = tag;
        }
    }
    private List<TextSnippet> dialogueSnippets = new List<TextSnippet>();
    private List<List<TextSnippet>> choicesSnippets = new List<List<TextSnippet>>();

    public void Init(Constants.Character character) {
        SetPatronName(character);
    }

    private void SetPatronName(Constants.Character character) {
        string patronName = Constants.characterNames[character];
        string patronColor = Constants.characterColor[character];
        nameText.text = "<color=#" + patronColor + ">" + patronName + "</color>";
    }

    public void SetDialogue(string dialogue, List<string> choices) {
        Clear(false);

        this.choices = choices;
        this.dialogue = dialogue;

        string[] dialogueSnippetsArr = Regex.Split(dialogue, @"(</?[^>]+>)");
        foreach (string snippet in dialogueSnippetsArr) {
            dialogueSnippets.Add(new TextSnippet(snippet, snippet.IndexOf("<") == 0));
        }

        foreach (string choice in choices) {
            List<TextSnippet> snippets = new List<TextSnippet>();
            string[] choicesSnippetsArr = Regex.Split(choice, @"(</?[^<>]+>)");
            snippets.Add(new TextSnippet("<link>", true));
            snippets.Add(new TextSnippet(">", false));
            foreach (string snippet in choicesSnippetsArr) {
                snippets.Add(new TextSnippet(snippet, snippet.IndexOf("<") == 0));
            }
            snippets.Add(new TextSnippet("</link>", true));
            choicesSnippets.Add(snippets);
        }

        ready = false;
        skip = false;
        StartCoroutine(TypeDialogue());
    }

    private IEnumerator TypeDialogue() {
        // I tried to abstract this out so we didn't have duplicate code, but I don't understand IEnumerators well enough
        foreach (TextSnippet snippet in  dialogueSnippets) {
            if (snippet.tag) {
                dialogueText.text += snippet.text;
            }
            else {
                foreach (char letter in snippet.text.ToCharArray()) {
                    dialogueText.text += letter;
                    if (!skip && letter != ' ') {
                        yield return new WaitForSeconds(dialogueTypingSpeed);
                    }
                }
            }
        }

        skip = false;
        StartCoroutine(TypeChoices());
    }

    private IEnumerator TypeChoices() {
        bool hasChoices = choicesSnippets.Count > 0;
        if (hasChoices) {
            choicesBox.SetActive(true);
            foreach (List<TextSnippet> choiceSnippet in choicesSnippets) {
                foreach (TextSnippet snippet in  choiceSnippet) {
                    if (snippet.tag) {
                        choicesText.text += snippet.text;
                    }
                    else {
                        foreach (char letter in snippet.text.ToCharArray()) {
                            choicesText.text += letter;
                            if (!skip && letter != ' ') {
                                yield return new WaitForSeconds(choicesTypingSpeed);
                            }
                        }
                    }
                }
                choicesText.text += "\n";
            }
        }

        ready = true;
        if (!hasChoices) {
            StartCoroutine(BlinkForward());
        }
    }

    private IEnumerator BlinkForward() {
        while (ready) {
            forwardText.SetActive(!forwardText.activeSelf);
            yield return new WaitForSeconds(blinkSpeed);
        }
    }
    
    void Update() {
        if (Input.GetMouseButtonDown(0)){
            skip = true;

            if (ready) {
                if (choicesSnippets.Count > 0) {
                    int index = TMP_TextUtilities.FindIntersectingLink(choicesText, Input.mousePosition, mainCamera);
                    if (index > -1) {
                        onSelectedChoice(index);
                    }
                }
                else {
                    onSelectedChoice(0);
                }
            }
        }
    }

    private void Clear(bool clearName) {
        if (clearName) {
            nameText.text = "";
        }
        choicesText.text = "";
        dialogueText.text = "";

        dialogueSnippets = new List<TextSnippet>();
        choicesSnippets = new List<List<TextSnippet>>();

        choicesBox.SetActive(false);
        forwardText.SetActive(false);
    }
}
