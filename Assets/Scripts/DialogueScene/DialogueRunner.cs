﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static DialogueObject;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System;
using System.Runtime.InteropServices;

public class DialogueRunner : MonoBehaviour {
    DialogueController controller;
    DialogueTextRunner textRunner;

    private void Start() {
        controller = GetComponent<DialogueController>();
        textRunner = GetComponent<DialogueTextRunner>();

        controller.onEnteredNode += OnNodeEntered;
        controller.InitializeDialogue();

        textRunner.onSelectedChoice += OnNodeSelected;
    }

    private void OnNodeSelected( int indexChosen ) {
        controller.ChooseResponse( indexChosen );
    }

    private void OnNodeEntered( Node newNode ) {
        if (newNode.IsEndNode()) {
            ExitDialogue();
            return;
        }

        List<string> responses = new List<string>();
        if (newNode.NeedsResponse()) {
            foreach (Response response in newNode.responses) {
                responses.Add(response.displayText);
            }
        }

        textRunner.Init(PersistentData.data.patronName, "f00", newNode.text, responses);
    }

    public void ExitDialogue() {
        SceneManager.UnloadSceneAsync("DialogueScene");
    }
}
