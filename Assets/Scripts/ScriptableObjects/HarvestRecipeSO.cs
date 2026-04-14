using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class DropEntry
{
    public ItemSO item;       //  vật phẩm rớt ra
    public int minAmount = 1; // số lượng tối thiểu
    public int maxAmount = 1; // số lượng tối đa
    [Range(0, 100)]
    public float dropChance = 100f; // Tỷ lệ rớt (%)
}

[CreateAssetMenu(fileName = "New Harvest Recipe", menuName = "ScriptableObjects/Harvest Recipe")]
public class HarvestRecipeSO : ScriptableObject
{
    [Header("Input")]
    public ResourceNodeSO targetNode;

    [Tooltip("Danh sách các loại công cụ có thể thu hoạch được tài nguyên này.")]
    public ToolSO[] validTools; 

    [Header("Output")]
    public List<DropEntry> dropList;

    public bool IsValidTool(ToolSO toolToCheck)
    {
        // Trường hợp 1: Nếu mảng rỗng (không cần công cụ, đập bằng tay không)
        if (validTools == null || validTools.Length == 0)
        {
            return true;
        }

        // Trường hợp 2: Quét xem Tool người chơi cầm có nằm trong danh sách không
        foreach (ToolSO tool in validTools)
        {
            if (tool == toolToCheck)
            {
                return true; // Hợp lệ!
            }
        }

        // Trường hợp 3: Có cầm công cụ, nhưng cầm sai loại (VD: Lấy Cuốc đi chặt Cây)
        return false;
    }
}
