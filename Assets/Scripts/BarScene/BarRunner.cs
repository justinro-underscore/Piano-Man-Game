using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BarRunner : MonoBehaviour {
    public GameObject patronsContainer;

    private void Start() {
        foreach (Patron patron in patronsContainer.GetComponentsInChildren(typeof(Patron))) {
            patron.onOpenDialogue += OnOpenDialogue;
        }
    }

    private void OnOpenDialogue(string patronName) {
        Debug.Log("Clicked on " + patronName);
        SceneManager.LoadScene("DialogueScene", LoadSceneMode.Additive);
    }
}
