using UnityEngine;

[CreateAssetMenu(fileName = "MissionData", menuName = "Data/Mission")]
public class MissionData : ScriptableObject
{
    [Header("Info")]
    public string missionName;
    [TextArea] public string description;

    [Header("Multiplayer")]
    public bool requireMultiplayer = true;
    public int requiredPlayerCount = 2;

    [Header("Objectives")]
    public MissionRequirement[] requirements;
}

[System.Serializable]
public class MissionRequirement
{
    public ItemData requiredItem;
    public MonsterData fromMonster;
    public int amount;
}