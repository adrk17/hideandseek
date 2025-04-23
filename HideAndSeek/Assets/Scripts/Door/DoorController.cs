using UnityEngine;

public class DoorController : MonoBehaviour
{
    public PressurePlate plate;
    public Transform doorTransform;
    public Collider doorCollider; 

    public Vector3 openOffset = new Vector3(0, 20f, 0);
    public float openSpeed = 6f;

    private Vector3 closedPos;
    private Vector3 targetPos;

    private bool isOpen = false;

    void Start()
    {
        closedPos = doorTransform.position;
        targetPos = closedPos;
    }

    void FixedUpdate()
    {
        UpdateDoorLogic();
        doorTransform.position = Vector3.Lerp(doorTransform.position, targetPos, Time.fixedDeltaTime * openSpeed);
    }

    void UpdateDoorLogic()
    {
        bool shouldOpen = plate.seekerCount > plate.hiderCount;
        
        // Start opening the door
        if (shouldOpen && !isOpen)
        {
            targetPos = closedPos + openOffset;
            if (doorCollider != null)
                doorCollider.enabled = false;

            isOpen = true;
        }
        // Start closing the door
        else if (!shouldOpen && isOpen)
        {
            targetPos = closedPos;
            if (doorCollider != null)
                doorCollider.enabled = true;

            isOpen = false;
        }
    }
}