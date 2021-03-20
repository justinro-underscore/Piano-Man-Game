using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;// Required when using Event data.

public class Patron : MonoBehaviour {
    public string patronName;

    private Collider2D patronCollider;

    void Awake() {
        patronCollider = GetComponent<Collider2D>();
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)){
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);
                
            if(hit.collider != null && hit.collider == patronCollider) {
                Debug.Log(patronName + " was clicked");
            }
        }
    }
}
