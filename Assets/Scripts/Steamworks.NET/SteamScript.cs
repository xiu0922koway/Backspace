using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;


public class SteamScript : MonoBehaviour 
{
	public static SteamScript s_instance;
	public static SteamScript Instance {
		get {
			if (s_instance == null) {
				return new GameObject("SteamScript").AddComponent<SteamScript>();
			}
			else {
				return s_instance;
			}
		}
	}
    
    void Start() 
    {
		if(SteamManager.Initialized) {
			Debug.Log(SteamFriends.GetPersonaName());
            Debug.Log(SteamUtils.GetAppID());
		}
	}

    public void CheckACH(int index)
    {
        if(SteamManager.Initialized) 
        {
            if(index == 0)
            {
                Steamworks.SteamUserStats.GetAchievement("ACH_END", out bool isAchived);

                if(!isAchived)
                {
                    SteamUserStats.SetAchievement("ACH_END");
                    SteamUserStats.StoreStats();
                }
            }
            if(index == 1)
            {
                Steamworks.SteamUserStats.GetAchievement("ACH_LOVE", out bool isAchived);

                if(!isAchived)
                {
                    SteamUserStats.SetAchievement("ACH_LOVE");
                    SteamUserStats.StoreStats();
                }
            }
            if(index == 2)
            {
                Steamworks.SteamUserStats.GetAchievement("ACH_SLEEP", out bool isAchived);

                if(!isAchived)
                {
                    SteamUserStats.SetAchievement("ACH_SLEEP");
                    SteamUserStats.StoreStats();
                }
            }
		}
    }
}
