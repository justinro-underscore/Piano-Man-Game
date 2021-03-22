using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Adapted from http://www.mrventures.net/all-tutorials/converting-a-twine-story-to-unity
 * https://www.youtube.com/watch?v=cmafUgj1cu8
 */
public class DialogueObject {
    private const string kStart = "START";
    private const string kEnd = "END";

    public struct Response {
        public string displayText;
        public string destinationNode;

        public Response( string display, string destination ) {
            displayText = display;
            destinationNode = destination;
        }
    }

    public class Node {
        public string title;
        public string text;
        public List<string> tags;
        public List<Response> responses;

        internal bool NeedsResponse() {
            if (responses.Count != 1) {
                return true;
            }
            Response response = responses[0];
            return !response.destinationNode.Equals(response.displayText);
        }

        internal bool IsEndNode() {
            return tags.Contains( kEnd );
        }
    }

    public class Dialogue {
        public string character;
        public Dictionary<string, Node> nodes;
        public string titleOfStartNode;

        public Dialogue(string characterName) {
            character = characterName;
            nodes = new Dictionary<string, Node>();
        }

        public Node GetNode(string nodeTitle) {
            return nodes[nodeTitle];
        }

        public Node GetStartNode() {
            UnityEngine.Assertions.Assert.IsNotNull(titleOfStartNode);
            return nodes[titleOfStartNode];
        }
    }

    public class DialogueContainer {
        public Dictionary<string, Dialogue> dialogues;

        public DialogueContainer(TextAsset twineText) {
            dialogues = new Dictionary<string, Dialogue>();
            ParseTwineText(twineText);
        }

        public void ParseTwineText(TextAsset twineText) {
            List<string> validNames = new List<string>(Constants.characters.Keys);

            string text = twineText.text;
            string[] nodeData = text.Split(new string[] {"::"}, StringSplitOptions.None);

            Node endNode = null;
            const int kIndexOfContentStart = 4;
            for ( int i = 0; i < nodeData.Length; i++ ) {
                if ( i < kIndexOfContentStart )
                    continue;
                Node curNode = new Node();

                // Note: tags are optional
                // Normal Format: "NodeTitle [Tags, comma, seperated] \n Message Text \n [[Response One]] \n [[Response Two]]"
                // No-Tag Format: "NodeTitle \n Message Text \n [[Response One]] \n [[Response Two]]"
                string currLineText = nodeData[i];
                bool tagsPresent = currLineText.IndexOf( "[" ) < currLineText.IndexOf( "\n" );
                int endOfFirstLine = currLineText.IndexOf( "\n" );

                // Extract Title
                int titleStart = 0;
                int titleEnd = tagsPresent
                    ? currLineText.IndexOf( "[" )
                    : endOfFirstLine;
                string title = currLineText.Substring(titleStart, titleEnd).Trim();
                curNode.title = title;

                // Extract Tags (if any)
                string tags = tagsPresent
                    ? currLineText.Substring( titleEnd + 1, (endOfFirstLine - titleEnd)-2)
                    : "";
                curNode.tags = new List<string>( tags.Split( new string [] { " " }, StringSplitOptions.None ) );

                curNode.responses = new List<Response>();

                string character = null;
                string titleOfStartNode = null;
                bool isEndNode = tags.Contains(kEnd);

                if (!isEndNode) {
                    string[] titleSegments = title.Split(new string[]{" - "}, StringSplitOptions.None);
                    UnityEngine.Assertions.Assert.IsTrue(titleSegments.Length == 2);
                    UnityEngine.Assertions.Assert.IsTrue(validNames.Contains(titleSegments[0]));
                    character = titleSegments[0];

                    int startOfResponses = -1;
                    int startOfResponseDestinations = currLineText.IndexOf( "[[" );
                    bool lastNode = (startOfResponseDestinations == -1);
                    UnityEngine.Assertions.Assert.IsTrue( startOfResponseDestinations != -1 ); // We should always have responses

                    // Last new line before "[["
                    startOfResponses = currLineText.Substring( 0, startOfResponseDestinations ).LastIndexOf( "\n" );

                    // Extract Message Text & Responses
                    string messsageText = currLineText.Substring( endOfFirstLine, startOfResponses - endOfFirstLine).Trim();
                    string responseText = !lastNode ? currLineText.Substring( startOfResponses ).Trim() : "";

                    curNode.text = messsageText;

                    if (curNode.tags.Contains(kStart)) {
                        titleOfStartNode = curNode.title;
                    }

                    // Note: response messages are optional (if no message then destination is the message)
                    // With Message Format: "\n Message[[Response One]]"
                    // Message-less Format: "\n [[Response One]]"
                    List<string> responseData = new List<string>(responseText.Split( new string [] { "\n" }, StringSplitOptions.None ));
                    for ( int k = responseData.Count-1; k >= 0; k-- ) { // Go backwards to remove potential duds
                        string curResponseData = responseData[k];

                        if ( string.IsNullOrEmpty( curResponseData ) ) {
                            responseData.RemoveAt( k );
                            continue;
                        }

                        // If message-less, then destination is the message
                        Response curResponse = new Response();
                        int destinationStart = curResponseData.IndexOf( "[[");
                        int destinationEnd = curResponseData.IndexOf( "]]");
                        string destination = curResponseData.Substring(destinationStart + 2, (destinationEnd - destinationStart)-2);
                        curResponse.destinationNode = destination;
                        if ( destinationStart == 0 )
                            curResponse.displayText = destination;
                        else
                            curResponse.displayText = curResponseData.Substring( 0, destinationStart );
                        curNode.responses.Add( curResponse );
                    }
                    curNode.responses.Reverse();
                }
                else {
                    endNode = curNode;
                }

                // string tt = curNode.title + "\n" + curNode.text + "\n";
                // foreach (string t in curNode.tags) {
                //     tt += t + "\n";
                // }
                // foreach (Response r in curNode.responses) {
                //     tt += r.displayText + " : " + r.destinationNode + "\n";
                // }
                // Debug.Log(tt);

                if (character != null) {
                    if (!dialogues.ContainsKey(character)) {
                        dialogues.Add(character, new Dialogue(character));
                    }
                    dialogues[character].nodes[curNode.title] = curNode;
                    if (titleOfStartNode != null) {
                        dialogues[character].titleOfStartNode = titleOfStartNode;
                    }
                }
            }

            foreach (string character in dialogues.Keys) {
                dialogues[character].nodes[endNode.title] = endNode;
            }
        }
    }
}