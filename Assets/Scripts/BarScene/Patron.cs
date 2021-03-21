using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Patron : MonoBehaviour {
    public string patronName;

    private Collider2D patronCollider;

    void Awake() {
        patronCollider = GetComponent<Collider2D>();
    }

    void OnMouseEnter()
    {
        Debug.Log("Enter " + patronName);
    }

    void OnMouseExit()
    {
        Debug.Log("Exit " + patronName);
    }

    void OnMouseDown()
    {
        Debug.Log("Test " + patronName);
        SceneManager.LoadScene("DialogueScene", LoadSceneMode.Additive);
    }
}
