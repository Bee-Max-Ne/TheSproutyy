using System;
using UnityEngine;

public class PlayerIndicator : MonoBehaviour
{
    public event EventHandler<OnSelectedResourceNodeChangedEventArgs> OnSelectedResourceNodeChanged;

    public class OnSelectedResourceNodeChangedEventArgs : EventArgs
    {
        public ResourceNode selectedResource;
    }

    [Header("References")]
    [SerializeField] private Player player;

    [Header("Settings")]
    [Tooltip("Offset để xác định tâm của Player trên Grid. Thường là 0.1f.")]
    [SerializeField] private float playerGridOffset = 0.1f;

    [Tooltip("Offset để đưa Indicator vào chính giữa ô Grid. Thường là 0.5f.")]
    [SerializeField] private float indicatorGridOffset = 0.5f;

    [Tooltip("Nếu true, hiện indicator cả khi tay không (None).")]
    [SerializeField] private bool showOnNone = true;

    [Header("Detection Settings")]
    [Tooltip("Chỉ định Layer chứa cây cối/vật thể tương tác (ví dụ: Layer 'Interactable')")]
    [SerializeField] private LayerMask interactableLayerMask;

    private ResourceNode currentSelectedResource;

    private SpriteRenderer spriteRenderer;
    private Camera mainCamera;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        mainCamera = Camera.main; // Cache camera để tối ưu hiệu năng
    }

    // Dùng LateUpdate để Indicator đi theo Player mượt mà nhất, không bị giật
    private void LateUpdate()
    {
        HandleIndicatorLogic();
    }

    private void HandleIndicatorLogic()
    {
        // 1. Kiểm tra trạng thái hiển thị
        ToolType currentType = player.GetEquippedToolType();

        if (currentType == ToolType.None)
        {
            spriteRenderer.enabled = showOnNone;
        }
        else
        {
            spriteRenderer.enabled = true;
        }

        if (!spriteRenderer.enabled) return;

        // 2. Lấy dữ liệu phạm vi từ công cụ
        int range = player.GetEquippedToolRange();

        // 3. Tính toán vị trí chuột trên lưới (Grid)
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int mouseGridPos = new Vector2Int(
            Mathf.FloorToInt(mouseWorldPos.x),
            Mathf.FloorToInt(mouseWorldPos.y)
        );

        // 4. Xác định ô trung tâm của Player
        // playerGridOffset giúp 'neo' tâm vùng chọn vào người Player ổn định hơn
        Vector2Int playerGridPos = new Vector2Int(
            Mathf.FloorToInt(player.transform.position.x + playerGridOffset),
            Mathf.FloorToInt(player.transform.position.y + playerGridOffset)
        );

        // 5. Giới hạn vị trí (Clamp) theo Range của Tool (3x3, 5x5,...)
        int targetX = Mathf.Clamp(mouseGridPos.x, playerGridPos.x - range, playerGridPos.x + range);
        int targetY = Mathf.Clamp(mouseGridPos.y, playerGridPos.y - range, playerGridPos.y + range);

        // 6. Áp dụng vị trí cuối cùng cho Indicator
        // indicatorGridOffset giúp đưa Indicator vào giữa ô
        transform.position = new Vector3(
            targetX + indicatorGridOffset,
            targetY + indicatorGridOffset,
            0f
        );

        // 7. Kiểm tra nếu có ResourceNode nào ở vị trí này
        CheckForResourceAtIndicator();
    }

    private void CheckForResourceAtIndicator()
    {
        // Dùng OverlapPoint tại tâm của Indicator. 
        // Lọc bằng LayerMask để tăng hiệu năng và tránh chạm nhầm mặt đất.
        Collider2D hit = Physics2D.OverlapPoint(transform.position, interactableLayerMask);

        ResourceNode newlySelectedResource = null;

        if (hit != null)
        {
            newlySelectedResource = hit.GetComponent<ResourceNode>();
        }

        // CHỈ gọi event nếu mục tiêu bị thay đổi (tránh spam event mỗi frame)
        if (newlySelectedResource != currentSelectedResource)
        {
            currentSelectedResource = newlySelectedResource;

            // Kích hoạt Event, gửi kèm dữ liệu
            OnSelectedResourceNodeChanged?.Invoke(this, new OnSelectedResourceNodeChangedEventArgs
            {
                selectedResource = currentSelectedResource
            });
        }
    }
}