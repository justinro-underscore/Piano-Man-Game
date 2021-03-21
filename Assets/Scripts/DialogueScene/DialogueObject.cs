using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        internal bool IsEndNode() {
            return tags.Contains( kEnd );
        }
    }

    public class Dialogue {
        string title;
        Dictionary<string, Node> nodes;
        string titleOfStartNode;
        public Dialogue( TextAsset twineText ) {
            nodes = new Dictionary<string, Node>();
            ParseTwineText( twineText );
        }

        public Node GetNode( string nodeTitle ) {
            return nodes [ nodeTitle ];
        }

        public Node GetStartNode() {
            UnityEngine.Assertions.Assert.IsNotNull( titleOfStartNode );
            return nodes [ titleOfStartNode ];
        }

        public void ParseTwineText( TextAsset twineText ) {
            string text = twineText.text;
            string[] nodeData = text.Split(new string[] { "::" }, StringSplitOptions.None);

            const int kIndexOfContentStart = 4;
            for ( int i = 0; i<nodeData.Length; i++ ) {
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

                if (!tags.Contains( kEnd )) {
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

                    if ( curNode.tags.Contains( kStart ) ) {
                        UnityEngine.Assertions.Assert.IsTrue( null == titleOfStartNode ); // TODO Remove to make way for multiple stories
                        titleOfStartNode = curNode.title;
                    }

                    // Note: response messages are optional (if no message then destination is the message)
                    // With Message Format: "\n Message[[Response One]]"
                    // Message-less Format: "\n [[Response One]]"
                    curNode.responses = new List<Response>();
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

                nodes [ curNode.title ] = curNode;
            }
        }
    }
}