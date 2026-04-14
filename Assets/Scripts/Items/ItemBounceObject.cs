using System.Collections;
using UnityEngine;

public class ItemBounceObject : MonoBehaviour
{
    [Header("Bounce Settings")]
    public float duration = 0.5f;   // Thời gian bay (giây)
    public float jumpHeight = 1.2f; // Độ cao của cú nảy

    public void StartBounce(Vector3 startPos, Vector3 targetPos)
    {
        StartCoroutine(BounceRoutine(startPos, targetPos));
    }

    private IEnumerator BounceRoutine(Vector3 start, Vector3 end)
    {
        float timePassed = 0f;

        while (timePassed < duration)
        {
            timePassed += Time.deltaTime;
            // Tính toán % thời gian đã trôi qua (từ 0 đến 1)
            float percent = timePassed / duration;

            // 1. Tính vị trí di chuyển ngang dần dần tới đích (Linear interpolation)
            Vector3 currentPos = Vector3.Lerp(start, end, percent);

            // 2. Tạo đường cong nảy lên bằng hàm Sine của Toán học (Mathf.Sin)
            // Mathf.Sin sẽ trả về 0 ở đầu đoạn, lên 1 ở giữa đoạn, và về 0 ở cuối đoạn
            float arcHeight = Mathf.Sin(percent * Mathf.PI) * jumpHeight;

            // Cộng thêm độ cao vào trục Y (để tạo cảm giác vật bay lên trên)
            currentPos.y += arcHeight;

            // Cập nhật vị trí
            transform.position = currentPos;

            yield return null; // Đợi đến frame tiếp theo
        }

        // Đảm bảo khi kết thúc, vật phẩm nằm chính xác ở điểm rơi cuối cùng
        transform.position = end;

        // Nảy xong rồi, cho phép nhặt!
        GetComponent<ItemPickup>().canBePickedUp = true;
    }
}
