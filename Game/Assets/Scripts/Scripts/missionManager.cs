using System.Collections.Generic;
using UnityEngine;

public class missionManager : MonoBehaviour
{
    public MissionData currentMission;

    private Dictionary<ItemData, int> teamProgress = new Dictionary<ItemData, int>();

    public void OnItemCollected(ItemData item)
    {
        if (!teamProgress.ContainsKey(item))
            teamProgress[item] = 0;

        teamProgress[item]++;

        CheckMissionComplete();
    }

    void CheckMissionComplete()
    {
        foreach (var req in currentMission.requirements)
        {
            if (!teamProgress.ContainsKey(req.requiredItem) ||
                teamProgress[req.requiredItem] < req.amount)
            {
                return;
            }
        }

        Debug.Log("MISSION COMPLETE");
    }
}