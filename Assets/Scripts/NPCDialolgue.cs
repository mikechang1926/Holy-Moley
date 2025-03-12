using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections.Generic; // 🔥 Add this to fix List<> error
public class NPCDialogue : MonoBehaviour
{
    public string npcName = "NPC"; // Default name (will be set dynamically)
    private NPCAttributes attributes;

    private static GameObject dialogueUI;
    private static TextMeshProUGUI dialogueText;
    private static TextMeshProUGUI npcNameText;
    private static Button closeButton;
    private static Button accuseButton;
    private static Button[] questionButtons; // Array for question buttons
    private Animator animator;
    private static Button restartButton; 
    void Start()
    {
        Debug.Log("NPCDialogue Start() running for: " + gameObject.name);
        animator = GetComponent<Animator>(); // Ensure NPC has an Animator component

        if (dialogueUI == null) 
        {
            dialogueUI = GameObject.Find("DialogueUI");
            if (dialogueUI == null)
            {
                Debug.LogError("❌ DialogueUI not found! Make sure it's named correctly in the Hierarchy.");
                return;
            }

            dialogueText = GameObject.Find("DialogueText")?.GetComponent<TextMeshProUGUI>();
            npcNameText = GameObject.Find("NPCNameText")?.GetComponent<TextMeshProUGUI>();

            if (dialogueText == null) Debug.LogError("❌ DialogueText not found! Check name.");
            if (npcNameText == null) Debug.LogError("❌ NPCNameText not found! Check name.");

            // 🔹 Handle close button separately
            closeButton = GameObject.Find("CloseButton")?.GetComponent<Button>();
            restartButton = GameObject.Find("RestartButton")?.GetComponent<Button>();
            if (restartButton != null) {
                restartButton.gameObject.SetActive(false);
                restartButton.GetComponent<Button>().onClick.AddListener(RestartGame);
            }
            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(CloseDialogue);
                Debug.Log("✅ Close button setup complete.");
            }
            else
            {
                Debug.LogError("❌ CloseButton not found! Check name.");
            }

            // 🔹 Handle accuse button separately
            accuseButton = GameObject.Find("AccuseButton")?.GetComponent<Button>();
            if (accuseButton != null)
            {
                accuseButton.onClick.RemoveAllListeners();
                accuseButton.onClick.AddListener(AccuseNPC);
                Debug.Log("✅ Accuse button setup complete.");
            }
            else
            {
                Debug.LogError("❌ AccuseButton not found! Check name.");
            }

            // 🔹 Only find actual question buttons
            List<Button> questionButtonList = new List<Button>();
            foreach (Button button in dialogueUI.GetComponentsInChildren<Button>())
            {
                if (button != closeButton && button != accuseButton) // Exclude close & accuse buttons
                {
                    questionButtonList.Add(button);
                }
            }
            questionButtons = questionButtonList.ToArray();

            Debug.Log("✅ Found " + questionButtons.Length + " question buttons inside DialogueUI.");

            // 🔹 Setup question button listeners
            foreach (Button button in questionButtons)
            {
                TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    string questionText = buttonText.text;
                    Debug.Log("✅ Setting up question button: " + questionText);
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() => AskQuestion(questionText));
                }
            }

            dialogueUI.SetActive(false); // Ensure it starts hidden
        }
    }


    void OnMouseDown()
    {
        // Ignore click if the mouse is over a UI element that blocks input
        if (IsPointerOverBlockingUI())
        {
            Debug.Log("❌ Click ignored: Mouse is over UI.");
            return;
        }

        Debug.Log("✅ " + gameObject.name + " clicked! Showing dialogue.");
        ShowDialogue();
    }

    // 🛠 Helper Function: Detects if the mouse is over a UI button or blocking element
    private bool IsPointerOverBlockingUI()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.CompareTag("UIBlocker")) // ✅ Only block clicks over specific UI elements
            {
                return true;
            }
        }
        return false;
    }

    public void SetAttributes(NPCAttributes newAttributes)
    {
        if (newAttributes == null)
        {
            Debug.LogError("SetAttributes() received null attributes for " + gameObject.name);
            return;
        }

        attributes = newAttributes;
        npcName = attributes.name; // Update NPC name
        Debug.Log("Attributes assigned to " + gameObject.name + " → Age: " + attributes.age + ", Origin: " + attributes.origin + ", Hobby: " + attributes.hobby);
    }


    public NPCAttributes GetAttributes()
    {
        return attributes;
    }

    public void ShowDialogue()
    {
        if (dialogueUI == null)
        {
            Debug.LogError("❌ ShowDialogue() called but dialogueUI is null!");
            return;
        }

        Debug.Log("✅ Showing dialogue for: " + npcName);
        npcNameText.text = npcName;
        dialogueText.text = "Hello, traveler! What brings you here?";
        dialogueUI.SetActive(true);

        // 🛠 FIX: Capture the current NPC correctly
        NPCDialogue currentNPC = this;

        foreach (Button button in questionButtons)
        {
            button.onClick.RemoveAllListeners(); // Clear previous listeners
            TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();

            if (buttonText != null)
            {
                string questionText = buttonText.text;
                button.onClick.AddListener(() => currentNPC.AskQuestion(questionText)); // Correct NPC reference
                Debug.Log("✅ Setting up button: " + questionText + " for " + currentNPC.npcName);
            }
        }

        // 🛠 FIX: Ensure CloseButton closes dialogue for the correct NPC
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(() => currentNPC.CloseDialogue());
        Debug.Log("✅ Close button now closes dialogue for " + currentNPC.npcName);

        // 🛠 FIX: Ensure AccuseButton accuses the correct NPC
        accuseButton.onClick.RemoveAllListeners();
        accuseButton.onClick.AddListener(() => currentNPC.AccuseNPC()); // 🔥 Correctly references current NPC
        Debug.Log("✅ Accuse button now accuses " + currentNPC.npcName);

    }





    public void AskQuestion(string question)
    {
        if (attributes == null)
        {
            Debug.LogError("❌ AskQuestion() called but attributes are NULL for " + gameObject.name + "! Was SetAttributes() called?");
            return;
        }

        // Debug log: Print out all attributes before answering
        Debug.Log("🔍 AskQuestion() called for: " + gameObject.name);
        Debug.Log("   → Name: " + attributes.name);
        Debug.Log("   → Age: " + attributes.age);
        Debug.Log("   → Origin: " + attributes.origin);
        Debug.Log("   → Hobby: " + attributes.hobby);
        Debug.Log("   → Is Mole: " + attributes.isMole);

        // Get answer from attributes
        string response = attributes.GetAnswer(question);

        // Debug log: Print the output of GetAnswer()
        Debug.Log("✅ GetAnswer('" + question + "') Output: " + response);

        // Display response in UI
        dialogueText.text = response;
    }


    void AccuseNPC()
    {
        NPCAttributes mole = NPCManager.Instance.GetMole(); // Get the actual mole

        if (attributes == null || mole == null)
        {
            Debug.LogError("❌ AccuseNPC() error: NPC attributes or mole is NULL!");
            dialogueText.text = "Something went wrong... Try again.";
            return;
        }

        // 🔍 Debugging: Print out what we are comparing
        Debug.Log($"🔍 Comparing accused NPC: {attributes.name} with mole NPC: {mole.name}");

        if (attributes.name == mole.name) // ✅ Compare by name instead of reference
        {
            dialogueText.text = "You got it! I am the mole!";
            Debug.Log($"✅ Correct! {attributes.name} is the mole.");
            
            animator.SetBool("found", true); 
            if (restartButton != null) 
            {
                // CloseDialogue();
                restartButton.gameObject.SetActive(true);
                accuseButton.gameObject.SetActive(false);
                closeButton.gameObject.SetActive(false);
     
                // 🔹 Setup question button listeners
                foreach (Button button in questionButtons)
                {
                    button.gameObject.SetActive(false);
                }



            } 
            else {
                Debug.Log("Restart button not found.");
            }
        }
        else
        {
            dialogueText.text = $"I'm not the mole! Try again. ";
        }
    }



    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // ✅ Reloads the scene
    }
    void CloseDialogue()
    {
        dialogueUI.SetActive(false);
    }
}
