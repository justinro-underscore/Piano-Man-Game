using System.Collections.Generic;
using UnityEngine;
using static DialogueObject;

/**
 * Adapted from http://www.mrventures.net/all-tutorials/converting-a-twine-story-to-unity
 * https://www.youtube.com/watch?v=cmafUgj1cu8
 */
public class DialogueController : MonoBehaviour {

    Dialogue curDialogue;
    Node curNode;

    public delegate void NodeEnteredHandler( Node node );
    public event NodeEnteredHandler onEnteredNode;

    public Node GetCurrentNode() {
        return curNode;
    }

    public void InitializeDialogue(Dialogue patronDialogue) {
        curDialogue = patronDialogue;
        string startNodeTitle = GameRunner.instance.data.dialogueIndex[patronDialogue.character];
        curNode = patronDialogue.GetNode(startNodeTitle);
        onEnteredNode(curNode);
    }

    public List<Response> GetCurrentResponses() {
        return curNode.responses;
    }

    public void ChooseResponse( int responseIndex ) {
        string nextNodeID = curNode.responses[responseIndex].destinationNode;
        Node nextNode = curDialogue.GetNode(nextNodeID);
        curNode = nextNode;
        onEnteredNode( nextNode );
    }
}
