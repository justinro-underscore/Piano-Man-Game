using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/NightInfo", order = 1)]
public class NightInfo : ScriptableObject
{
    public int nightNumber;
    public TextAsset twineText;
    public List<string> characters;
}
