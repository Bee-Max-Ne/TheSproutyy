using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Tree : ResourceNode
{
    public override void TakeDamage(ToolSO playerTool)
    {
        base.TakeDamage(playerTool);
        Debug.Log($"Tree took {playerTool.power} damage from {playerTool.toolName}. Remaining health: {currentHealth}");
    }
}
