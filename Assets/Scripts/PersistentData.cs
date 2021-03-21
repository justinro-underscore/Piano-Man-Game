using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentData : MonoBehaviour {
    public static PersistentData data = null;

    public string patronName = null;

    void Start() {
        if (data == null) {
            data = this;
        }
        else if (data != this) {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
}
