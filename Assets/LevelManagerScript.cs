using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LevelManagerScript : MonoBehaviour
{
    public PlayerNovels noli;
    public PlayerNovels elFili;
    [SerializeField] private bool resetOnStart = false;

    private void Awake()
    {
        // Initialize novels
        //noli = new PlayerNovels(Novels.NoliMeTangere);
        //elFili = new PlayerNovels(Novels.ElFilibusterismo);
        // Save the player novels if needed
        //print(PlayerPrefs.GetInt("FinishedTutorial", 0) == 1);
        if (string.IsNullOrEmpty(MyData.Load().user_id))
        {
            Debug.LogWarning("No User Data, Return to Login Page");
            SceneManager.LoadScene("LoginScene");
            return;
        }
        if (resetOnStart) PlayerPrefs.SetInt("HasPlayerLevelData", 0);
        if (!PlayerLevelsData.HasData()) // no saved data found
        {
            print("No data found, creating new data...");
            PlayerLevelsManager.Save(noli);
            PlayerLevelsManager.Save(elFili);
            PlayerLevelsData.SaveData(PlayerLevelsData.LevelsData());
            print("New data has been saved");
            PlayerPrefs.SetInt("FinishedTutorial", 0);
            SceneManager.LoadScene("Guide"); // Go to tutorial first time
        }
        else
        {
            //if (string.IsNullOrEmpty(MyData.Load().user_id)) return;
            int totalStars = PlayerLevelsData.GetAllStars();
            int finishedTutorial = PlayerPrefs.GetInt("FinishedTutorial", 0); // 0 = not done, 1 = done

            // 🟡 Condition 1: No stars, no tutorial → Go to tutorial
            if (totalStars <= 0 && finishedTutorial == 0)
            {
                print("No stars and no tutorial — going to tutorial.");
                SceneManager.LoadScene("Guide");
            }
            // 🟡 Condition 2: No stars, but tutorial done → Do nothing
            else if (totalStars <= 0 && finishedTutorial == 1)
            {
                print("No stars but tutorial already done — staying here.");
            }
            // 🟡 Condition 3: Has stars, no tutorial → Save tutorial progress
            else if (totalStars > 0 && finishedTutorial == 0)
            {
                print("Has stars but tutorial not marked — saving tutorial as finished.");
                PlayerPrefs.SetInt("FinishedTutorial", 1);
                PlayerPrefs.Save();
            }
            else
            {
                print("All good — game continues normally.");
            }
        }
    }

    private void Start()
    {

        //PlayerNovels noli = PlayerLevelsManager.Load(Novels.NoliMeTangere);
        //PlayerLevels level = noli.FindNextLevel("Level1");
        //noli.LockLevel(level.Levelname);
        //PlayerLevelsManager.Save(noli);
        //print(level.Levelname);
        //print(level.Lock());
        // Example of loading and logging
        //var loadedNoli = PlayerLevelsManager.Load(Novels.NoliMeTangere);
        //Debug.Log(JsonUtility.ToJson(loadedNoli));
        //print(PlayerLevelsData.LevelsData().JsonGetAll());
        //print("hehe");
    }
}

[System.Serializable]
public class PlayerLevelsData
{
    public string noli;
    public string elfili;

    public string JsonGetAll() => JsonUtility.ToJson(this);
    public static PlayerLevelsData JsonConvertAll(string json) => JsonUtility.FromJson<PlayerLevelsData>(json);

    public static PlayerLevelsData LevelsData()
    {
        return new PlayerLevelsData
        {
            noli = PlayerLevelsManager.GetJsonString(Novels.NoliMeTangere),
            elfili = PlayerLevelsManager.GetJsonString(Novels.ElFilibusterismo)
        };
    }

    public static void SaveData(PlayerLevelsData data)
    {
        PlayerLevelsManager.Save(PlayerLevelsManager.GetFromJson(data.noli));
        PlayerLevelsManager.Save(PlayerLevelsManager.GetFromJson(data.elfili));
        PlayerPrefs.SetInt("HasPlayerLevelData", 1);
    }

    public static bool HasData()
    {
        return PlayerPrefs.GetInt("HasPlayerLevelData", 0) == 1;
    }

    public static int GetAllStars()
    {
        int stars = 0;
        foreach (PlayerLevels levels in PlayerLevelsManager.Load(Novels.NoliMeTangere).playerLevels)
        {
            stars += levels.stars;
        }
        foreach (PlayerLevels levels in PlayerLevelsManager.Load(Novels.ElFilibusterismo).playerLevels)
        {
            stars += levels.stars;
        }

        return stars;
    }

    public static int GetMaxStars()
    {
        string grade = MyData.Load().grade_lvl;
        int stars = 0;

        foreach (PlayerLevels levels in PlayerLevelsManager.Load(Novels.NoliMeTangere).playerLevels)
        {
            stars += 3;
        }

        if(grade == "Grade - 10")
        {
            foreach (PlayerLevels levels in PlayerLevelsManager.Load(Novels.ElFilibusterismo).playerLevels)
            {
                stars += 3;
            }
        }

        return stars;
    }

    public static int GetMaxScrolls()
    {
        string grade = MyData.Load().grade_lvl;

        List<PlayerLevels> noliM = PlayerLevelsManager.Load(Novels.NoliMeTangere).playerLevels;
        List<PlayerLevels> elM = PlayerLevelsManager.Load(Novels.ElFilibusterismo).playerLevels;

        int stars = (noliM.Count() * 10);
        if(grade == "Grade - 10")
        {
            stars += (elM.Count() * 10);
        }

        return stars;
    }

    public bool IsCompleted()
    {
        bool noliComplete = PlayerLevelsManager.Load(Novels.NoliMeTangere).IsFinished();
        bool elFiliComplete = PlayerLevelsManager.Load(Novels.ElFilibusterismo).IsFinished();

        return noliComplete && elFiliComplete;
    }

}

// Make this static so it can be used anywhere
public static class PlayerLevelsManager
{
    public static void Save(PlayerNovels playerNovel)
    {
        string novelKey = EnumHelper.GetNovel(playerNovel.novel);
        string playerNovelsString = JsonUtility.ToJson(playerNovel);
        PlayerPrefs.SetString(novelKey, playerNovelsString);
        ProgressData.Altered();
        PlayerPrefs.Save();
    }

    public static PlayerNovels Load(Novels novel)
    {
        string novelKey = EnumHelper.GetNovel(novel);
        string playerNovelsString = PlayerPrefs.GetString(novelKey, "");

        if (string.IsNullOrEmpty(playerNovelsString))
            return null;

        return JsonUtility.FromJson<PlayerNovels>(playerNovelsString);
    }

    public static string GetJsonString(Novels novel)
    {
        string novelKey = EnumHelper.GetNovel(novel);
        return PlayerPrefs.GetString(novelKey, "");
    }

    public static PlayerNovels GetFromJson(string json) => JsonUtility.FromJson<PlayerNovels>(json);
}

[System.Serializable]
public class PlayerNovels
{
    public Novels novel;
    public List<PlayerLevels> playerLevels = new List<PlayerLevels>();

    // Constructor
    public PlayerNovels(Novels novel)
    {
        this.novel = novel;
    }

    // 🔎 Find a level by name
    public PlayerLevels FindLevel(string levelName)
    {
        return playerLevels.FirstOrDefault(level => level.Levelname == levelName);
    }

    // 🔎 Find the next level after the current one
    public PlayerLevels FindNextLevel(string currentLevel)
    {
        for (int i = 0; i < playerLevels.Count - 1; i++) // -1 prevents out-of-bounds
        {
            if (currentLevel == playerLevels[i].Levelname)
            {
                return playerLevels[i + 1];
            }
        }
        return null;
    }

    // 📌 Get the index of a level
    public int GetLevelIndex(string levelName)
    {
        for (int i = 0; i < playerLevels.Count; i++)
        {
            if (playerLevels[i].Levelname == levelName)
                return i;
        }
        return -1; // not found
    }

    // 🔓 Unlock the next level and return it
    public PlayerLevels UnlockNextLevel(string currentLevel)
    {
        var next = FindNextLevel(currentLevel);
        if (next != null)
        {
            next.Unlock();
            PlayerLevelsManager.Save(this);
            return next;
        }
        return null; // no next level available
    }

    public PlayerLevels LockLevel(string currentLevel)
    {
        var current = FindLevel(currentLevel);
        if (current != null)
        {
            current.Lock();
            PlayerLevelsManager.Save(this);
            return current;
        }
        return null; 
    }

    // Forcefully set stars (no condition)
    public PlayerLevels SetStars(string currentLevel, int star)
    {
        var current = FindLevel(currentLevel);
        if (current != null)
        {
            current.SetStars(star); // always sets
            PlayerLevelsManager.Save(this);
            return current;
        }
        return null;
    }

    // Update stars only if higher than current
    public PlayerLevels UpdateStars(string currentLevel, int star)
    {
        var current = FindLevel(currentLevel);
        if (current != null && star > current.stars)
        {
            current.SetStars(star);
            PlayerLevelsManager.Save(this);
            return current;
        }
        return null;
    }

    // Reset stars to 0
    public PlayerLevels ResetStars(string currentLevel)
    {
        var current = FindLevel(currentLevel);
        if (current != null)
        {
            current.ResetStar();
            PlayerLevelsManager.Save(this);
            return current;
        }
        return null;
    }

    public bool IsFinished() => playerLevels[^1].stars > 0;

    // 📂 Serialization helpers
    public string ToJson() => JsonUtility.ToJson(this, true);

    public static PlayerNovels FromJson(string json) =>
        JsonUtility.FromJson<PlayerNovels>(json);
}

[System.Serializable]
public class PlayerLevels
{
    public string Levelname;
    public bool isLocked = true;
    public int stars = 0;

    public bool SetStars(int star)
    {
        if (star > stars && star > 0 && star <= 3)
        {
            stars = star;
            ProgressData.Altered();
            return true;
        }
        return false;
    }

    public bool ResetStar()
    {
        stars = 0;
        ProgressData.Altered();
        return true;
    }
    public bool Unlock()
    {
        isLocked = false;
        ProgressData.Altered();
        return true;
    }

    public bool Lock()
    {
        isLocked = true;
        ProgressData.Altered();
        return true;
    }
}