using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
//No está completamente desarrollado.
[System.Serializable]
public class GameAttempt
{
    public int attemptId;
    public int molesHit;
    public int molesMissed;
    public int totalMoles;
    public float spawnInterval;
    public float moleLifetime;
}

[System.Serializable]
public class DateEntry
{
    public string date;
    public List<GameAttempt> attempts = new List<GameAttempt>();
}

[System.Serializable]
public class PlayerStats
{
    public List<DateEntry> dateEntries = new List<DateEntry>();
    private const string fileName = "playerStats.json";
    private int currentAttemptId;
    private DateEntry currentDateEntry;
    private GameAttempt currentAttempt;

    // Guarda datos en JSON
    public void SaveStats()
    {
        string json = JsonUtility.ToJson(this, true);
        Debug.Log("Saving stats to JSON: " + json); 
        string path = Path.Combine(Application.persistentDataPath, fileName);
        File.WriteAllText(path, json);
    }

    // Carga datos desde JSON
    public static PlayerStats LoadStats()
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            Debug.Log("Loading stats from JSON: " + json); 
            PlayerStats loadedStats = JsonUtility.FromJson<PlayerStats>(json);
            return loadedStats;
        }
        else
        {
            Debug.Log("No stats file found, creating new stats.");
            return new PlayerStats(); 
        }
    }

    // Inicia un nuevo intento
    public void StartNewAttempt()
    {
        DateTime now = DateTime.Now;
        string todayDate = now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

        currentDateEntry = dateEntries.Find(entry => entry.date == todayDate);
        if (currentDateEntry == null)
        {
            currentDateEntry = new DateEntry { date = todayDate };
            dateEntries.Add(currentDateEntry);
        }

        currentAttemptId = currentDateEntry.attempts.Count + 1;

        currentAttempt = new GameAttempt
        {
            attemptId = currentAttemptId,
            molesHit = 0,
            molesMissed = 0,
            totalMoles = 0,
            spawnInterval = 0,
            moleLifetime = 0
        };
    }

    // Actualiza el intento actual
    public void UpdateCurrentAttempt(int molesHit, int molesMissed, int totalMoles, float spawnInterval, float moleLifetime)
    {
        if (currentAttempt != null)
        {
            currentAttempt.molesHit = molesHit;
            currentAttempt.molesMissed = molesMissed;
            currentAttempt.totalMoles = totalMoles;
            currentAttempt.spawnInterval = spawnInterval;
            currentAttempt.moleLifetime = moleLifetime;
        }
    }

    // Añade el intento actual a la lista y prepara uno nuevo
    public void FinalizeCurrentAttempt()
    {
        if (currentDateEntry != null && currentAttempt != null)
        {
            currentDateEntry.attempts.Add(currentAttempt);
        }

        StartNewAttempt();
    }
}
