using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BarRunner : MonoBehaviour {
    public GameObject patronsContainer;
    public Image overlay;

    private void Start() {
        foreach (Patron patron in patronsContainer.GetComponentsInChildren(typeof(Patron))) {
            patron.onOpenDialogue += OnOpenDialogue;
        }
    }

    private void OnOpenDialogue(string patronName) {
        Debug.Log("Clicked on " + patronName);
        overlay.color = new Color(0, 0, 0, 0.5f);
        PersistentData.data.patronName = patronName;
        SceneManager.LoadScene("DialogueScene", LoadSceneMode.Additive);
    }
}
