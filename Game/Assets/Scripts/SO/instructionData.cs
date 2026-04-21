using UnityEngine;

[CreateAssetMenu(fileName = "InstructionData", menuName = "Data/Instruction Data")]
public class InstructionData : ScriptableObject
{
    [Header("UI")]
    [TextArea]
    public string text = "Press E"; // nội dung hướng dẫn

    public bool showText = true; // có hiển thị hướng dẫn hay không

    [Header("Input")]
    public KeyCode key = KeyCode.E; // phím cần nhấn
}
