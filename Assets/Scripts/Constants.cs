using System.Collections.Generic;

public class Constants {
    public enum Character {
        BARTENDER_JOHN,
        CHRISTOPHER_FLETCHING,
        JENNIFER,
        DAVY,
        PAUL,
        EUGENE
    };

    static public IDictionary<string, Character> characters = new Dictionary<string, Character>() {
        {"JOHN", Character.BARTENDER_JOHN},
        {"CHRIS", Character.CHRISTOPHER_FLETCHING},
        {"JEN", Character.JENNIFER},
        {"DAVY", Character.DAVY},
        {"PAUL", Character.PAUL},
        {"GENE", Character.EUGENE}
    };

    static public IDictionary<Character, string> characterNames = new Dictionary<Character, string>() {
        {Character.BARTENDER_JOHN, "Bartender John"},
        {Character.CHRISTOPHER_FLETCHING, "Christopher Fletching"},
        {Character.JENNIFER, "Jennifer"},
        {Character.DAVY, "Davy"},
        {Character.PAUL, "Paul"},
        {Character.EUGENE, "Eugene"}
    };

    static public IDictionary<Character, string> characterColor = new Dictionary<Character, string>() {
        {Character.BARTENDER_JOHN, "f00"},
        {Character.CHRISTOPHER_FLETCHING, "ff0"},
        {Character.JENNIFER, "0f0"},
        {Character.DAVY, "0ff"},
        {Character.PAUL, "00f"},
        {Character.EUGENE, "f0f"}
    };

    public enum DialogueElementType {
        ACTION,
        BIO,
        MEMORY
    };

    static public IDictionary<DialogueElementType, string> dialogueElementColor = new Dictionary<DialogueElementType, string>() {
        {DialogueElementType.ACTION, "f00"},
        {DialogueElementType.BIO, "0f0"},
        {DialogueElementType.MEMORY, "00f"}
    };

    public enum ResponseElementType {
        LOCKED,
        KICK
    };
}
