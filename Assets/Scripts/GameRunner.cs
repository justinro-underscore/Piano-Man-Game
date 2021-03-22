using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static DialogueObject;

public class GameRunner : MonoBehaviour {
    public static GameRunner instance = null;
    public int currNight;
    public string currScene = null;

    // Defines the data that should be shared across scenes
    public struct PersistentData {
        public Dictionary<string, string> dialogueIndex; // Link character names to node name
        public Dialogue patronDialogue;
    }
    public PersistentData data = new PersistentData();

    // Each item is one night
    public List<DialogueContainer> dialogues;
    public List<NightInfo> nightInfos;

    public delegate void CloseDialogueHandler(bool completed);
    public event CloseDialogueHandler onCloseDialogue;

    void Awake() {
        if (instance == null) {
            instance = this;
            SetupGameRunner();
        }
        else if (instance != this) {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    private void SetupGameRunner() {
        currScene = "BarScene";
        dialogues = new List<DialogueContainer>();
        nightInfos = nightInfos.OrderBy(night => night.nightNumber).ToList();
        foreach (NightInfo nightInfo in nightInfos) {
            dialogues.Add(new DialogueContainer(nightInfo.twineText));
        }
        StartNight(0); // TODO Move out of here
    }

    private void StartNight(int nightNumber) {
        UnityEngine.Assertions.Assert.IsTrue(nightNumber < dialogues.Count);

        currNight = nightNumber;
        Dictionary<string, Dialogue> characterDialogues = dialogues[nightNumber].dialogues;
        data.dialogueIndex = new Dictionary<string, string>();
        foreach (string character in nightInfos[nightNumber].characters) {
            Node startNode = characterDialogues[character].GetStartNode();
            data.dialogueIndex.Add(character, startNode.title);
        }
    }

    public void OpenDialogueScene(string patronName) {
        data.patronDialogue = dialogues[currNight].dialogues[patronName];
        SceneManager.LoadScene("DialogueScene", LoadSceneMode.Additive);
        currScene = "DialogueScene";
    }

    public void CloseDialogueScene(bool completed) {
        SceneManager.UnloadSceneAsync("DialogueScene");
        currScene = "BarScene";
        onCloseDialogue(completed);
    }
}
