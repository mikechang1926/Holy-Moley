using UnityEngine;
using System.Collections.Generic; 

[System.Serializable]
public class NPCAttributes
{
    public string name;
    public int age;
    public string origin;
    public string hobby;
    public bool isMole;

    public string GetAnswer(string question)
    {
        switch (question)
        {
            case "What's your age?":
                return $"I am {age} years old.";

            case "Where are you from?":
                return $"I am from {origin}.";

            case "What is your favorite hobby?":
                return $"I love {hobby}.";

            case "What have you heard about the mole?":
                return GenerateMoleHint();

            default:
                return "I don't understand that question.";
        }
    }

    private static List<string> usedHints = new List<string>(); // Track used hints

    private string GenerateMoleHint()
    {
        NPCAttributes mole = NPCManager.Instance.GetMole();

        if (mole == null)
        {
            return "Mole not found";
        }

        if (isMole)
        {
            return GetFalseHint(mole); // Mole lies
        }
        else
        {
            return GetUniqueHint(mole); // Honest NPCs provide unique hints
        }
    }

    private string GetUniqueHint(NPCAttributes mole)
    {
        List<string> possibleHints = new List<string>
        {
            $"I heard the mole is from {GetRegion(mole.origin)}.",
            $"I think the mole's hobby is {GetHobbyCategory(mole.hobby)}.",
            mole.age > 30 ? "The mole is over 30 years old." : "The mole is 30 or younger."
        };

        // Remove already used hints
        possibleHints.RemoveAll(hint => usedHints.Contains(hint));

        // If no hints left, return "I don't know anything"
        if (possibleHints.Count == 0)
        {
            return "I don't know anything.";
        }

        // Select a new hint
        string selectedHint = possibleHints[Random.Range(0, possibleHints.Count)];
        usedHints.Add(selectedHint); // Mark hint as used
        return selectedHint;
    }


    private string GetTrueHint(NPCAttributes mole)
    {
        if (mole == null) {
            return "Mole not found";
        }
        // Select a random type of hint
        int hintType = Random.Range(0, 3);

        switch (hintType)
        {
            case 0: // Hint about the mole's origin (continent-based)
                string region = GetRegion(mole.origin);
                return $"I heard the mole is from {region}.";

            case 1: // Hint about the mole's hobby type
                string hobbyCategory = GetHobbyCategory(mole.hobby);
                return $"I think the mole's hobby is {hobbyCategory}.";

            case 2: // Hint about the mole's age
                return mole.age > 30 ? "The mole is over 30 years old." : "The mole is 30 or younger.";

            default:
                return "I don't know much about the mole.";
        }
    }

    private string GetFalseHint(NPCAttributes mole)
    {
        // The mole lies by giving an incorrect hint
        int hintType = Random.Range(0, 3);

        switch (hintType)
        {
            case 0: // False region hint
                string wrongRegion = GetRandomWrongRegion(mole.origin);
                return $"I heard the mole is from {wrongRegion}.";

            case 1: // False hobby hint
                string wrongHobby = GetRandomWrongHobby(mole.hobby);
                return $"I think the mole's hobby is {wrongHobby}.";

            case 2: // False age hint
                return mole.age > 30 ? "The mole is 30 or younger." : "The mole is over 30 years old.";

            default:
                return "I can't say much about the mole...";
        }
    }

    private string GetRegion(string origin)
    {
        // Mapping origins to regions
        if (origin == "Paris" || origin == "Berlin") return "Europe";
        if (origin == "Tokyo") return "Asia";
        if (origin == "New York") return "North America";
        if (origin == "Rio") return "South America";
        return "somewhere far away"; // Default fallback
    }

    private string GetHobbyCategory(string hobby)
    {
        // Categorizing hobbies
        if (hobby == "Hiking" || hobby == "Running") return "an exercise";
        if (hobby == "Painting" || hobby == "Cooking") return "a creative activity";
        return "something unusual";
    }

    private string GetRandomWrongRegion(string correctOrigin)
    {
        List<string> regions = new List<string> { "Europe", "Asia", "Africa", "North America", "South America", "Australia" };
        string correctRegion = GetRegion(correctOrigin);
        regions.Remove(correctRegion); // Remove the correct answer
        return regions[Random.Range(0, regions.Count)];
    }

    private string GetRandomWrongHobby(string correctHobby)
    {
        List<string> hobbies = new List<string> { "an exercise", "a creative activity", "something unusual" };
        string correctCategory = GetHobbyCategory(correctHobby);
        hobbies.Remove(correctCategory); // Remove the correct answer
        return hobbies[Random.Range(0, hobbies.Count)];
    }

}
