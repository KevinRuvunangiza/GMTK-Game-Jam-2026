using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.InputSystem;

public class MatchManager : MonoBehaviour
{
    [Header("Game Data")]
    [Tooltip("Drop your populated Archetype SO files here")]
    public List<ProgressionArchetypeSO> allArchetypes;
    private ProgressionArchetypeSO currentArchetype;

    [Header("Chat Bubble UI")]
    public Transform chatContainer; // The empty object with the Vertical Layout Group
    public GameObject npcBubblePrefab; // The pink/left bubble prefab
    public GameObject playerBubblePrefab; // The blue/right bubble prefab
    public TextMeshProUGUI[] optionTextDisplays; // The text inside your 4 UI buttons
    public TextMeshProUGUI timerTextDisplay; // The visual clock

    [Header("Game Over UI")]
    public GameObject gameOverCanvas; // The "You've Been Blocked!" screen

    [Header("Match State")]
    public float globalTimer = 60f;
    private int currentAnger = 0;
    private int currentStepIndex = 0;
    private bool isMatchActive = true;

    void Start()
    {
        LoadNextMatch();
    }

    void Update()
    {
        // Pause timer and block inputs when the Game Over screen is active
        if (!isMatchActive) return;

        globalTimer -= Time.deltaTime;

        if (timerTextDisplay != null)
        {
            timerTextDisplay.text = globalTimer.ToString("F1");
        }

        if (globalTimer <= 0)
        {
            Debug.Log("GAME OVER! TIME RAN OUT!");
            this.enabled = false;
            return;
        }

        // Temporary Keyboard Controls using the New Input System
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
        if (allArchetypes == null || allArchetypes.Count == 0) return;

        // Clean up the UI: Destroy all previous chat bubbles
        foreach (Transform child in chatContainer)
        {
            Destroy(child.gameObject);
        }

        // Load the new target
        currentArchetype = allArchetypes[Random.Range(0, allArchetypes.Count)];
        currentAnger = 0;
        currentStepIndex = 0;

        DisplayCurrentStep();
    }

    public void DisplayCurrentStep()
    {
        // Trap them on the final step if they haven't reached the anger threshold
        if (currentStepIndex >= currentArchetype.conversationSequence.Count)
        {
            currentStepIndex = currentArchetype.conversationSequence.Count - 1;
        }

        DialogueStep currentStep = currentArchetype.conversationSequence[currentStepIndex];

        // Pick a random variation of the NPC's current complaint
        string npcLine = currentStep.npcLineVariations[Random.Range(0, currentStep.npcLineVariations.Length)];

        // Spawn the NPC Bubble on the left side of the chat
        GameObject newBubble = Instantiate(npcBubblePrefab, chatContainer);
        newBubble.GetComponentInChildren<TextMeshProUGUI>().text = npcLine;

        // Push the 4 static douchebag options to the UI buttons
        for (int i = 0; i < currentStep.options.Length; i++)
        {
            if (i < optionTextDisplays.Length && optionTextDisplays[i] != null)
            {
                optionTextDisplays[i].text = currentStep.options[i].answerText;
            }
        }
    }

    public void SelectOption(int index)
    {
        DialogueStep currentStep = currentArchetype.conversationSequence[currentStepIndex];
        if (index >= currentStep.options.Length) return;

        PlayerResponse selectedResponse = currentStep.options[index];

        // Spawn the Player Bubble on the right side of the chat
        GameObject playerBubble = Instantiate(playerBubblePrefab, chatContainer);
        playerBubble.GetComponentInChildren<TextMeshProUGUI>().text = selectedResponse.answerText;

        // Apply the anger value
        currentAnger += selectedResponse.angerValue;

        // Check if the threshold is met
        if (currentAnger >= currentArchetype.angerThresholdToBlock)
        {
            isMatchActive = false; // Freeze the game loop

            if (gameOverCanvas != null)
            {
                gameOverCanvas.SetActive(true); // Pop up the Blocked screen
            }
        }
        else
        {
            currentStepIndex++;
            DisplayCurrentStep();
        }
    }

    // Brian hooks the "NEXT" button on the Game Over canvas directly to this function
    public void NextMatchAfterBlock()
    {
        globalTimer += 3f; // Reward time

        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(false); // Hide the blocked screen
        }

        isMatchActive = true; // Unpause the timer
        LoadNextMatch(); // Instantly swipe to the next chat
    }
}