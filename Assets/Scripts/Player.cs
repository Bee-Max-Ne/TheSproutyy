using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private GameInput gameInput;
    [SerializeField] private float moveSpeed = 5.0f;

    private Vector2 inputVector;
    private bool isMoving;

    void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, inputVector.y, 0);

        float moveDistance = moveSpeed * Time.deltaTime;
        transform.position += moveDir * moveDistance;

        isMoving = inputVector != Vector2.zero;
    }

    public Vector2 GetInputVector()
    {
        return inputVector;
    }

    public bool IsMoving()
    {
        return isMoving;
    }
}
