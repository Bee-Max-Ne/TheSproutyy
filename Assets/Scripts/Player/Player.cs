using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static Unity.Cinemachine.CinemachineSplineRoll;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    public event EventHandler<OnToolUsedEventArgs> OnToolUsed;

    public class OnToolUsedEventArgs : EventArgs
    {
        public ToolType toolType;
    }

    [SerializeField] private GameInput gameInput;
    [SerializeField] private PlayerIndicator playerIndicator;
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private ToolSO equippedTool;

    private Rigidbody2D rb;

    private Vector2 inputVector;
    private bool isMoving;
    private ResourceNode targetResource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        gameInput.OnUseToolAction += GameInput_OnUseToolAction;
        playerIndicator.OnSelectedResourceNodeChanged += PlayerIndicator_OnSelectedResourceNodeChanged;
        
    }

    private void PlayerIndicator_OnSelectedResourceNodeChanged(object sender, PlayerIndicator.OnSelectedResourceNodeChangedEventArgs e)
    {
        targetResource = e.selectedResource;
    }

    private void GameInput_OnUseToolAction(object sender, EventArgs e)
    {
        if (!isMoving)
        {
            OnToolUsed?.Invoke(this, new OnToolUsedEventArgs
            {
                toolType = GetEquippedToolType()
            });
        }
    }

    public void PerformToolAction()
    {
        // Kiểm tra xem trước mặt có mục tiêu nào không (cây, đá...)
        if (targetResource != null)
        {
            ToolSO currentTool = equippedTool;

            // Gây sát thương lên cái cây/cục đá
            targetResource.TakeDamage(currentTool);
        }
    }

    void Update()
    {
        inputVector = gameInput.GetMovementVectorNormalized();
        isMoving = inputVector != Vector2.zero;
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        rb.MovePosition(rb.position + inputVector * moveSpeed * Time.fixedDeltaTime);
    }

    public ToolType GetEquippedToolType()
    {
        return equippedTool != null ? equippedTool.toolType : ToolType.None;
    }

    public Vector2 GetInputVector()
    {
        return inputVector;
    }

    public bool IsMoving()
    {
        return isMoving;
    }

    public int GetEquippedToolRange()
    {
        return equippedTool.interactRange;
    }

    public void ClearTargetResource()
    {
        targetResource = null;
    }
}
