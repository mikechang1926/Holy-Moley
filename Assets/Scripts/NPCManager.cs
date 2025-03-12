using UnityEngine;
using System.Collections.Generic;

public class NPCManager : MonoBehaviour
{
    public static NPCManager Instance; // Singleton pattern
    public List<NPCDialogue> npcs; // Assign in Unity Inspector

    private NPCAttributes moleNPC; // The actual mole

    private int[] ages = { 25, 30, 35, 40, 45 };
    private string[] origins = { "New York", "Paris", "Tokyo", "Berlin", "Rio" };
    private string[] hobbies = { "Running", "Hiking", "Painting", "Cooking", "Gaming" };

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        AssignNPCAttributes();
    }
    void AssignNPCAttributes()
    {
        if (npcs == null || npcs.Count == 0)
        {
            Debug.LogError("No NPCs assigned in NPCManager! Make sure you add them in the Inspector.");
            return;
        }

        Debug.Log("Assigning attributes to NPCs...");

        for (int i = 0; i < npcs.Count; i++)
        {
            NPCAttributes attributes = new NPCAttributes
            {
                name = npcs[i].gameObject.name, // Use the GameObject name from the Hierarchy
                age = ages[Random.Range(0, ages.Length)],
                origin = origins[Random.Range(0, origins.Length)],
                hobby = hobbies[Random.Range(0, hobbies.Length)],
                isMole = false // Default false
            };

            npcs[i].SetAttributes(attributes);
            Debug.Log("Assigned attributes to " + npcs[i].gameObject.name + " → Age: " + attributes.age + ", Origin: " + attributes.origin + ", Hobby: " + attributes.hobby);
        }

        // Pick one NPC to be the mole and ensure `isMole` is updated correctly
        int moleIndex = Random.Range(0, npcs.Count);
        moleNPC = npcs[moleIndex].GetAttributes();
        moleNPC.isMole = true;  // ✅ Ensuring the mole attribute is set

        Debug.Log("🕵️ Mole selected: " + moleNPC.name);
    }


    public NPCAttributes GetMole()
    {
        if (moleNPC == null)
        {
            Debug.LogError("❌ GetMole() called, but moleNPC is NULL!");
        }
        return moleNPC;
    }

}
