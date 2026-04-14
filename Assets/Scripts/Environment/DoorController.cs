using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private Collider2D physicalCollider; // Kéo cái Collider chặn vào đây

    private Animator anim;
    private bool playerInRange = false;
    private const string IS_OPEN = "IsOpen";


    void Awake()
    {
        // Tự động tìm Animator ở object con
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (playerInRange)
        {
            OpenDoor();
        }
        else
        {
            CloseDoor();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Player>())
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Player>())
        {
            playerInRange = false;
        }
    }

    private void OpenDoor()
    {
        anim.SetBool(IS_OPEN, playerInRange);
        if (physicalCollider != null)
            physicalCollider.enabled = false; // Mở đường cho Teemo
    }

    private void CloseDoor()
    {
        anim.SetBool(IS_OPEN, playerInRange);
        if (physicalCollider != null)
            physicalCollider.enabled = true; // Chặn đường lại
    }
}
