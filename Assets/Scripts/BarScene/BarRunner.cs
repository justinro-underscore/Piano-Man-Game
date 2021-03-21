using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BarRunner : MonoBehaviour {
    public GameObject patronsContainer;
    public Image overlay;

    private Patron selectedPatron = null;

    private void Start() {
        GameRunner.instance.onCloseDialogue += OnCloseDialogue;
        foreach (Patron patron in patronsContainer.GetComponentsInChildren(typeof(Patron))) {
            patron.onOpenDialogue += OnOpenDialogue;
        }
    }

    private void OnOpenDialogue(Patron patron) {
        selectedPatron = patron;
        overlay.color = new Color(0, 0, 0, 0.5f);
        GameRunner.instance.OpenDialogueScene(patron.patronName);
    }

    private void OnCloseDialogue(bool completed) {
        overlay.color = new Color(0, 0, 0, 0);
        if (completed) {
            selectedPatron.DisablePatron();
        }
        selectedPatron = null;
    }
}
