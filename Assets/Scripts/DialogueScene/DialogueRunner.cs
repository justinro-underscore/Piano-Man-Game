using System.Collections.Generic;
using UnityEngine;
using static DialogueObject;

public class DialogueRunner : MonoBehaviour {
    DialogueController controller;
    DialogueTextRunner textRunner;

    private void Start() {
        controller = GetComponent<DialogueController>();
        textRunner = GetComponent<DialogueTextRunner>();

        controller.onEnteredNode += OnNodeEntered;
        controller.InitializeDialogue(GameRunner.instance.data.patronDialogue);

        textRunner.onSelectedChoice += OnNodeSelected;
        Constants.Character character = Constants.characters[GameRunner.instance.data.patronDialogue.character];
        textRunner.Init(character);
    }

    private void OnNodeSelected( int indexChosen ) {
        controller.ChooseResponse( indexChosen );
    }

    private void OnNodeEntered( Node newNode ) {
        if (newNode.IsEndNode()) {
            ExitDialogue(true);
            return;
        }

        List<string> responses = new List<string>();
        if (newNode.NeedsResponse()) {
            foreach (Response response in newNode.responses) {
                responses.Add(response.displayText);
            }
        }

        textRunner.SetDialogue(newNode.text, responses);
    }

    public void ExitDialogue(bool completed) {
        GameRunner.instance.CloseDialogueScene(completed);
    }
}
