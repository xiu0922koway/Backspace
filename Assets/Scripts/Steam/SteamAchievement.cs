using System.Collections;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;

public static class SteamAchievement
{
    public static void UnloadAchievement(string achievementID)
    {
        if(!SteamManager.Initialized) 
        {
            Debug.Log("Steam not initialized");
            return;   
        }

        SteamUserStats.SetAchievement(achievementID);
        bool outCheck;
        SteamUserStats.GetAchievement(achievementID,out outCheck);
        if(outCheck)
        {
            Debug.Log("New Achievement Unlock: " + achievementID);
            
        }
        else
        {
            Debug.Log("failed to unlock achievement" + achievementID);
        }
    }
}
