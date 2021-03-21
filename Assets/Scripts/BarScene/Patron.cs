using System.Collections;
using UnityEngine;

public class Patron : MonoBehaviour {
    public string patronName;

    public delegate void OpenDialogueHandler(Patron patron);
    public event OpenDialogueHandler onOpenDialogue;

    private bool selectable;

    void Awake() {
        selectable = true;
    }

    void OnMouseEnter() {
        if (GameRunner.instance.currScene.Equals("BarScene") && selectable) {
            transform.localScale = new Vector3(1.1f, 1.1f, 1);
        }
    }

    void OnMouseExit() {
        if (GameRunner.instance.currScene.Equals("BarScene") && selectable) {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    void OnMouseDown() {
        if (GameRunner.instance.currScene.Equals("BarScene") && selectable) {
            transform.localScale = new Vector3(1, 1, 1);
            onOpenDialogue(this);
        }
    }

    public void DisablePatron() {
        selectable = false;
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        Color color = renderer.color;
        renderer.color = new Color(Mathf.Max(color.r - 0.5f, 0), Mathf.Max(color.g - 0.5f, 0), Mathf.Max(color.b - 0.5f, 0));
    }
}
