using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants {
    public enum Characters {
        BARTENDER_JOHN,
        CHRISTOPHER_FLETCHING,
        JENNIFER,
        DAVY,
        PAUL,
        EUGENE
    };

    static public IDictionary<string, Characters> characters = new Dictionary<string, Characters>() {
        {"JOHN", Characters.BARTENDER_JOHN},
        {"CHRIS", Characters.CHRISTOPHER_FLETCHING},
        {"JEN", Characters.JENNIFER},
        {"DAVY", Characters.DAVY},
        {"PAUL", Characters.PAUL},
        {"GENE", Characters.EUGENE}
    };
}
