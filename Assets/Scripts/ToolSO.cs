using UnityEngine;

[CreateAssetMenu(fileName = "New Tool", menuName = "ScriptableObjects/ToolSO")]
public class ToolSO : ScriptableObject
{
    public string toolName;
    public ToolType toolType;
    //public Sprite toolVisual;
}
