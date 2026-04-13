using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "InstructionData", menuName = "Data/Instruction Data")]
public class InstructionData : ScriptableObject
{
    [Header("UI")]
    public Sprite icon;           // icon nút (E, F, ...)
    public string text;           // optional (nếu muốn hiển thị chữ)

    [Header("Input")]
    public KeyCode key = KeyCode.E;

    [Header("Settings")]
    public bool showText = false; // bật/tắt text
}