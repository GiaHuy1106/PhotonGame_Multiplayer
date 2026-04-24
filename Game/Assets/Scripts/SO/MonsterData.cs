using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData", menuName = "Data/Monster")]
public class MonsterData : ScriptableObject
{
    public string monsterID;
    public string monsterName;

    public DropTable dropTable;
}