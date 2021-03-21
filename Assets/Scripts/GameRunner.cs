using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static DialogueObject;

public class GameRunner : MonoBehaviour {
    public static GameRunner instance = null;
    public string currScene = null;

    public struct PersistentData {
        public string patronName;
        public List<Dialogue> dialogues;
    }
    public PersistentData data = new PersistentData();

    public delegate void CloseDialogueHandler(bool completed);
    public event CloseDialogueHandler onCloseDialogue;

    void Awake() {
        if (instance == null) {
            instance = this;
            currScene = "BarScene";
        }
        else if (instance != this) {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    public void OpenDialogueScene(string patronName) {
        data.patronName = patronName;
        SceneManager.LoadScene("DialogueScene", LoadSceneMode.Additive);
        currScene = "DialogueScene";
    }

    public void CloseDialogueScene(bool completed) {
        SceneManager.UnloadSceneAsync("DialogueScene");
        currScene = "BarScene";
        onCloseDialogue(completed);
    }
}
