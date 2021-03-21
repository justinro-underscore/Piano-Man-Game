using System.Collections;
using UnityEngine;

public class Patron : MonoBehaviour {
    public string patronName;

    private Collider2D patronCollider;

    public delegate void OpenDialogueHandler( string patronName );
    public event OpenDialogueHandler onOpenDialogue;

    void Awake() {
        patronCollider = GetComponent<Collider2D>();
    }

    void OnMouseEnter() {
        Debug.Log("Enter " + patronName);
    }

    void OnMouseExit() {
        Debug.Log("Exit " + patronName);
    }

    void OnMouseDown() {
        onOpenDialogue(patronName);
    }
}
