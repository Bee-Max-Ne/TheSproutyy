using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";
    private const string SPEED = "Speed";
    private const string TOOL_TYPE = "ToolType";
    private const string DO_ACTION = "DoAction";

    private Animator animator;
    [SerializeField] private Player player;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        player.OnToolUsed += Player_OnToolUsed;
    }

    private void Player_OnToolUsed(object sender, Player.OnToolUsedEventArgs e)
    {
        UseToolAnimation();
    }

    private void Update()
    {
        Vector2 input = player.GetInputVector();

        // CHỈ cập nhật Horizontal và Vertical khi nhân vật đang di chuyển
        // Điều này giúp Blend Tree "nhớ" hướng nhìn cuối cùng khi đứng yên (Idle)
        if (player.IsMoving())
        {
            animator.SetFloat(HORIZONTAL, input.x);
            animator.SetFloat(VERTICAL, input.y);
        }

        // Cập nhật Speed để Animator biết khi nào chuyển từ Idle sang Walk
        // magnitude giúp lấy độ lớn của Vector (nếu > 0 nghĩa là đang đi)
        animator.SetFloat(SPEED, input.sqrMagnitude);
    }

    public void UseToolAnimation()
    {
        ToolType currentType = player.GetEquippedToolType();

        // Gửi ToolType vào một Parameter "ToolType" (Int) trong Animator
        animator.SetInteger(TOOL_TYPE, (int)currentType);

        // Kích hoạt hành động
        animator.SetTrigger(DO_ACTION);
    }
}
