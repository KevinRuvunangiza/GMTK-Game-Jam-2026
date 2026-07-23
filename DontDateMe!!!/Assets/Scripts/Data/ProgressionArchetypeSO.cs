using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewProgressionArchetype", menuName = "AntiDatingApp/ProgressionArchetype")]
public class ProgressionArchetypeSO : ScriptableObject
{
    public string archetypeName;
    public int angerThresholdToBlock = 5;

    public List<DialogueStep> conversationSequence;
}

[System.Serializable]
public struct PlayerResponse
{
    public string answerText;
    public int angerValue;
}

[System.Serializable]
public struct DialogueStep
{
    [TextArea]
    public string[] npcLineVariations;
    public PlayerResponse[] options;
}