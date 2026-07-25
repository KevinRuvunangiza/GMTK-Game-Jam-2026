using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewProgressionArchetype", menuName = "AntiDatingApp/ProgressionArchetype")]
public class ProgressionArchetypeSO : ScriptableObject
{
    public string archetypeName;
    public int angerThresholdToBlock = 8;

    [Header("Drop your .tsv file here!")]
    public TextAsset dialogueSpreadsheet;

    public List<DialogueStep> conversationSequence;

    // Right-click the ScriptableObject in Unity's Inspector to run this!
    [ContextMenu("Import Dialogue from TSV")]
    public void ImportFromTSV()
    {
        if (dialogueSpreadsheet == null)
        {
            Debug.LogError("No TSV file attached! Drag your spreadsheet into the slot.");
            return;
        }

        conversationSequence = new List<DialogueStep>();

        // Split the file into rows
        string[] rows = dialogueSpreadsheet.text.Trim().Split('\n');

        // Start at i = 1 to skip the header row in your spreadsheet
        for (int i = 1; i < rows.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(rows[i])) continue;

            // Split the row into columns by Tab
            string[] cols = rows[i].Split('\t');

            // Safety check: Make sure we have exactly 12 columns
            if (cols.Length < 12)
            {
                Debug.LogWarning($"Skipped a row because it didn't have 12 columns! Check row {i + 1} in your spreadsheet.");
                continue;
            }

            DialogueStep newStep = new DialogueStep();

            // Columns 1, 2, and 3 are the NPC variations
            newStep.npcLineVariations = new string[] { cols[1].Trim(), cols[2].Trim(), cols[3].Trim() };

            // Columns 4 through 11 are the buttons and anger values
            newStep.options = new PlayerResponse[4];

            newStep.options[0] = new PlayerResponse { answerText = cols[4].Trim(), angerValue = int.Parse(cols[5].Trim()) };
            newStep.options[1] = new PlayerResponse { answerText = cols[6].Trim(), angerValue = int.Parse(cols[7].Trim()) };
            newStep.options[2] = new PlayerResponse { answerText = cols[8].Trim(), angerValue = int.Parse(cols[9].Trim()) };
            newStep.options[3] = new PlayerResponse { answerText = cols[10].Trim(), angerValue = int.Parse(cols[11].Trim()) };

            conversationSequence.Add(newStep);
        }

        Debug.Log($"Successfully imported {conversationSequence.Count} steps for {archetypeName}!");
    }
}

// --- THESE ARE THE STRUCTS UNITY WAS MISSING ---

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