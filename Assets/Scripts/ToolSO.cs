using UnityEngine;

[CreateAssetMenu(fileName = "New Tool", menuName = "ScriptableObjects/ToolSO")]
public class ToolSO : ScriptableObject
{
    public string toolName;
    public ToolType toolType;
    //public Sprite toolVisual;
    public int interactRange = 1; // Mặc định là 1 (cho vùng 3x3)
}
