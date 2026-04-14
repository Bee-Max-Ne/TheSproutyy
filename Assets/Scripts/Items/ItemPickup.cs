using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ItemPickup : MonoBehaviour
{
    [Header("Magnet Settings")]
    public float moveSpeed = 8f;     // Tốc độ bay vào người
    public float destroyDistance = 0.2f; // Khoảng cách để biến mất (0.2 để không bị lẹm vào giữa người)

    private bool isMagnetized = false;
    private Transform playerTransform;

    [HideInInspector] public bool canBePickedUp = false;

    private void Update()
    {
        // Item chỉ tốn tài nguyên chạy Update nếu nó ĐÃ BỊ HÚT
        if (isMagnetized && playerTransform != null)
        {
            // 1. Bay từ từ về phía Player
            transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, moveSpeed * Time.deltaTime);

            // 2. Kiểm tra nếu đã bay tới sát Player thì xóa
            if (Vector3.Distance(transform.position, playerTransform.position) < destroyDistance)
            {
                // TODO: Gọi hàm add vào Inventory ở đây sau này

                Destroy(gameObject);
            }
        }
    }

    // Hàm này được gọi tự động khi MagnetZone của Player chạm vào Item
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!canBePickedUp) return;
        // Nếu vòng tròn hút của Player chạm vào vật phẩm
        if (!isMagnetized && collision.gameObject.CompareTag("Magnet"))
        {
            isMagnetized = true;
            // Lưu lại vị trí của Player để đuổi theo
            playerTransform = collision.transform;
        }
    }
}
