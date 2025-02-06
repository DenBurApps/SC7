using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class PlayerBalanceController
{
    private static readonly string SaveFilePath = Path.Combine(Application.persistentDataPath, "PlayerData");
    private const int StartBalance = 1000;

    static PlayerBalanceController()
    {
        LoadBalanceData();
    }

    public static event Action<int> OnBalanceChanged;

    public static int CurrentBalance { get; private set; }


    public static void IncreaseBalance(int amount)
    {
        CurrentBalance += amount;
        SaveBalanceData();
        OnBalanceChanged?.Invoke(CurrentBalance);
    }

    public static void DecreaseBalance(int amount)
    {
        if (CurrentBalance >= amount)
        {
            CurrentBalance -= amount;
            OnBalanceChanged?.Invoke(CurrentBalance);
        }

        SaveBalanceData();
    }

    private static void LoadBalanceData()
    {
        if (!File.Exists(SaveFilePath))
        {
            ResetToDefault();
            return;
        }
        try
        {
            string json = File.ReadAllText(SaveFilePath);
            var data = JsonUtility.FromJson<PlayerData>(json);

            CurrentBalance = data.Balance;
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to load balance: " + e.Message);
            ResetToDefault();
        }
    }

    private static void SaveBalanceData()
    {
        try
        {
            var data = new PlayerData(CurrentBalance);
            string json = JsonUtility.ToJson(data);
            File.WriteAllText(SaveFilePath, json);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save balance: " + e.Message);
        }
    }

    private static void ResetToDefault()
    {
        CurrentBalance = StartBalance;
        SaveBalanceData();
    }

    [Serializable]
    private class PlayerData
    {
        public int Balance;

        public PlayerData(int balance)
        {
            Balance = balance;
        }
    }

    public static bool HasEnoughBalance(int betAmount)
    {
        return betAmount <= CurrentBalance;
    }
}