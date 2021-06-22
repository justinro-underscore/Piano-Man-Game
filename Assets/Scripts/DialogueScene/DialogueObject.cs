using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

/**
 * Adapted from http://www.mrventures.net/all-tutorials/converting-a-twine-story-to-unity
 * https://www.youtube.com/watch?v=cmafUgj1cu8
 */
public class DialogueObject {
    private const string kNightStart = "NIGHT-START";
    private const string kStart = "START";
    private const string kEnd = "END";
    private static Regex elementRegex = new Regex(@"<([a-z]+)=?([^>]+)?>([^<]*)</([a-z]+)>");
    private static Regex responseRegex = new Regex(@"(.*)\[\[([^\]]+)\]\](.*)");

    public class Response {
        public string displayText;
        public string destinationNode;
        public string lockedKey;
        public bool shouldKick;

        public Response(string input) {
            string display = ParseResponseText(input);
            ParseDisplayText(display);
        }

        private string ParseResponseText(string input) {
            GroupCollection groups = responseRegex.Match(input).Groups;
            UnityEngine.Assertions.Assert.AreEqual(4, groups.Count);
            destinationNode = groups[2].Value;
            return (groups[1].Value + groups[3].Value).Trim();
        }

        private void ParseDisplayText(string rawDisplayText) {
            lockedKey = null;
            shouldKick = false;

            string display = rawDisplayText;
            GroupCollection groups = elementRegex.Match(display).Groups;
            while (groups.Count != 1) {
                UnityEngine.Assertions.Assert.AreEqual(5, groups.Count);
                string elementTypeOpening = groups[1].Value;
                string elementTypeClosing = groups[4].Value;
                UnityEngine.Assertions.Assert.AreEqual(elementTypeOpening, elementTypeClosing);

                Constants.ResponseElementType elementType;
                Enum.TryParse(elementTypeOpening.ToUpper(), out elementType);
                if (elementType == Constants.ResponseElementType.LOCKED) {
                    lockedKey = groups[2].Value.Trim();
                    UnityEngine.Assertions.Assert.IsTrue(lockedKey.Length > 0);
                }
                else if (elementType == Constants.ResponseElementType.KICK) {
                    shouldKick = true;
                }

                display = groups[3].Value.Trim();
                groups = elementRegex.Match(display).Groups;
            }

            displayText = display;
        }
    }

    public struct Element {
        public Constants.DialogueElementType type;
        public string key;

        public Element(string elementText, string key) {
            Enum.TryParse(elementText.ToUpper(), out type);
            this.key = key;
        }
    }

    public class Node {
        public string title;
        public string text;
        public List<string> tags;
        public List<Response> responses;
        public List<Element> elements;

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

            string text = prepText(twineText.text);
            string[] nodeData = text.Split(new string[] {"::"}, StringSplitOptions.None);

            Node endNode = null;
            const int kIndexOfContentStart = 5;
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
                    ? currLineText.Substring( titleEnd + 1, currLineText.IndexOf("]") - titleEnd - 1 )
                    : "";
                curNode.tags = new List<string>( tags.Split( new string [] { " " }, StringSplitOptions.None ) );

                bool isNightStartNode = tags.Contains(kNightStart);
                if (isNightStartNode) {
                    continue;
                }

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
                    foreach (string responseDataText in responseData) {
                        Response response = new Response(responseDataText);
                        curNode.responses.Add(response);
                    }
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

        private string prepText(string text) {
            // Remove curly brackets
            int lastTextLength = text.Length;
            bool noCurlyBrackets = false;
            while (!noCurlyBrackets) {
                text = new Regex(" *{[^{}]*} *").Replace(text, "");
                if (lastTextLength == text.Length) {
                    noCurlyBrackets = true;
                }
                else {
                    lastTextLength = text.Length;
                }
            }

            return text;
        }
    }
}