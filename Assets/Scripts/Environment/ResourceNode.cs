using NUnit.Framework;
using UnityEngine;

public class ResourceNode : MonoBehaviour
{
    [SerializeField] protected HarvestRecipeSO harvestRecipeSO;

    protected int currentHealth;

    protected virtual void Start()
    {
        if (harvestRecipeSO != null && harvestRecipeSO.targetNode != null)
        {
            currentHealth = harvestRecipeSO.targetNode.maxHealth;
        }
    }

    public virtual void TakeDamage(ToolSO playerTool)
    {
        // 1. Kiểm tra xem tool người chơi đang cầm có được phép không
        if (!harvestRecipeSO.IsValidTool(playerTool)) return;

        // 2. Trừ máu dựa trên sức mạnh của Tool
        currentHealth -= playerTool.power;

        // Gợi ý: Bạn có thể gọi Animator chung ở đây 
        // GetComponent<Animator>().SetTrigger("Hit");

        // 3. Kiểm tra phá hủy
        if (currentHealth <= 0)
        {
            DestroySelf();
        }
    }

    public virtual void DestroySelf()
    {
        Player.Instance.ClearTargetResource();

        DropItems();

        Destroy(gameObject);
    }

    protected virtual void DropItems()
    {
        if (harvestRecipeSO == null || harvestRecipeSO.dropList == null || harvestRecipeSO.dropList.Count == 0) return;

        foreach (DropEntry entry in harvestRecipeSO.dropList)
        {
            if (UnityEngine.Random.Range(0f, 100f) <= entry.dropChance)
            {
                int dropAmount = UnityEngine.Random.Range(entry.minAmount, entry.maxAmount + 1);

                for (int i = 0; i < dropAmount; i++)
                {
                    if (entry.item != null && entry.item.prefab != null)
                    {
                        // 1. Vị trí gốc (ngay tại gốc cây)
                        Vector3 startPos = transform.position;

                        // 2. Vị trí đích (Random xung quanh)
                        float randomX = UnityEngine.Random.Range(-0.8f, 0.8f);
                        float randomY = UnityEngine.Random.Range(-0.8f, 0.8f);
                        Vector3 endPos = startPos + new Vector3(randomX, randomY, 0f);

                        // 3. Sinh ra vật phẩm tại VỊ TRÍ GỐC
                        Transform spawnedItem = Instantiate(entry.item.prefab, startPos, Quaternion.identity);

                        // 4. Lấy script hiệu ứng và kích hoạt nó bay tới đích
                        ItemBounceObject bounceAnim = spawnedItem.GetComponent<ItemBounceObject>();
                        if (bounceAnim != null)
                        {
                            bounceAnim.StartBounce(startPos, endPos);
                        }
                    }
                }
            }
        }
    }
}
