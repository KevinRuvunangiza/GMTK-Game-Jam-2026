using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class MatchManager : MonoBehaviour
{
    [Header("Game Data")]
    public List<ProgressionArchetypeSO> allArchetypes;
    private ProgressionArchetypeSO currentArchetype;

    [Header("Match State")]
    public float globalTimer = 60f;
    private int currentAnger = 0;
    private int currentStepIndex = 0;

    void Start()
    {
        LoadNextMatch();
    }

    void Update()
    {
        globalTimer -= Time.deltaTime;
        if (globalTimer <= 0)
        {
            Debug.Log("GAME OVER! TIME RAN OUT!");
            this.enabled = false;
        }

        // Temporary test controls: Press 1, 2, 3, or 4 on your keyboard to test
        if (Keyboard.current != null)
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame) SelectOption(0);
            if (Keyboard.current.digit2Key.wasPressedThisFrame) SelectOption(1);
            if (Keyboard.current.digit3Key.wasPressedThisFrame) SelectOption(2);
            if (Keyboard.current.digit4Key.wasPressedThisFrame) SelectOption(3);
        }
    }

    public void LoadNextMatch()
    {
        if (allArchetypes.Count == 0) return;

        currentArchetype = allArchetypes[Random.Range(0, allArchetypes.Count)];
        currentAnger = 0;
        currentStepIndex = 0;

        Debug.Log($"--- SWIPE! New Match: {currentArchetype.archetypeName} ---");
        DisplayCurrentStep();
    }

    public void DisplayCurrentStep()
    {
        if (currentStepIndex >= currentArchetype.conversationSequence.Count)
        {
            Debug.Log("She got bored and ghosted you! Penalty!");
            globalTimer -= 5f;
            LoadNextMatch();
            return;
        }

        DialogueStep currentStep = currentArchetype.conversationSequence[currentStepIndex];
        string npcLine = currentStep.npcLineVariations[Random.Range(0, currentStep.npcLineVariations.Length)];

        Debug.Log($"{currentArchetype.archetypeName} says: {npcLine}");
        for (int i = 0; i < currentStep.options.Length; i++)
        {
            Debug.Log($"Option [{i}]: {currentStep.options[i].answerText}");
        }
    }

    public void SelectOption(int index)
    {
        DialogueStep currentStep = currentArchetype.conversationSequence[currentStepIndex];

        // Prevent errors if you press a number for an option that doesn't exist
        if (index >= currentStep.options.Length) return;

        PlayerResponse selectedResponse = currentStep.options[index];
        currentAnger += selectedResponse.angerValue;

        Debug.Log($"You chose option [{index}]. Anger is now {currentAnger}/{currentArchetype.angerThresholdToBlock}");

        if (currentAnger >= currentArchetype.angerThresholdToBlock)
        {
            Debug.Log("BAM! SHE BLOCKED YOU! +3 Seconds!");
            globalTimer += 3f;
            LoadNextMatch();
        }
        else
        {
            currentStepIndex++;
            DisplayCurrentStep();
        }
    }
}