using UnityEngine;
using System.Collections.Generic;
using TMPro; // Required for the UI text
using UnityEngine.InputSystem; // Required for the New Input System fix

public class MatchManager : MonoBehaviour
{
    [Header("Game Data")]
    [Tooltip("Drag your Archetype ScriptableObjects here!")]
    public List<ProgressionArchetypeSO> allArchetypes;
    private ProgressionArchetypeSO currentArchetype;

    [Header("UI Elements")]
    public TextMeshProUGUI npcTextDisplay;
    public TextMeshProUGUI[] optionTextDisplays; // Make sure this array has exactly 4 elements in the Inspector

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
        // 1. The WarioWare Countdown
        globalTimer -= Time.deltaTime;
        if (globalTimer <= 0)
        {
            Debug.Log("GAME OVER! TIME RAN OUT!");
            this.enabled = false; // Stops the loop
            return;
        }

        // 2. Temporary Keyboard Controls (Using the New Input System)
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
        if (allArchetypes == null || allArchetypes.Count == 0)
        {
            Debug.LogError("You need to add Archetypes to the list in the GameManager!");
            return;
        }

        // Pick a random girl from your list
        currentArchetype = allArchetypes[Random.Range(0, allArchetypes.Count)];

        // Reset the state for the new match
        currentAnger = 0;
        currentStepIndex = 0;

        Debug.Log($"--- SWIPE! New Match: {currentArchetype.archetypeName} ---");
        DisplayCurrentStep();
    }

    public void DisplayCurrentStep()
    {
        // Check if we ran out of dialogue before she got angry enough
        if (currentStepIndex >= currentArchetype.conversationSequence.Count)
        {
            Debug.Log("She got bored and ghosted you! Penalty!");
            globalTimer -= 5f; // Lose 5 seconds
            LoadNextMatch(); // Swipe away
            return;
        }

        // Grab the current step of the conversation
        DialogueStep currentStep = currentArchetype.conversationSequence[currentStepIndex];

        // Pick a random variation of what the NPC says and push it to the UI
        string npcLine = currentStep.npcLineVariations[Random.Range(0, currentStep.npcLineVariations.Length)];
        if (npcTextDisplay != null)
        {
            npcTextDisplay.text = npcLine;
        }

        // Loop through your 4 buttons and update their text on the UI
        for (int i = 0; i < currentStep.options.Length; i++)
        {
            if (i < optionTextDisplays.Length && optionTextDisplays[i] != null)
            {
                optionTextDisplays[i].text = currentStep.options[i].answerText;
            }
        }
    }

    // Brian's UI buttons (and your keyboard shortcuts) call this function
    public void SelectOption(int index)
    {
        DialogueStep currentStep = currentArchetype.conversationSequence[currentStepIndex];

        // Safety check to prevent errors if you press a button that doesn't have an option
        if (index >= currentStep.options.Length) return;

        // Get the response you clicked and add its anger value
        PlayerResponse selectedResponse = currentStep.options[index];
        currentAnger += selectedResponse.angerValue;

        Debug.Log($"You chose option [{index}]. Anger is now {currentAnger}/{currentArchetype.angerThresholdToBlock}");

        // Evaluate the Win/Loss State
        if (currentAnger >= currentArchetype.angerThresholdToBlock)
        {
            Debug.Log("BAM! SHE BLOCKED YOU! +3 Seconds!");
            globalTimer += 3f; // Reward time
            LoadNextMatch(); // Instantly swipe to the next girl
        }
        else
        
        {
            // Not angry enough yet. Move to the next dialogue step!
            currentStepIndex++;
            DisplayCurrentStep();
        }
    }
}