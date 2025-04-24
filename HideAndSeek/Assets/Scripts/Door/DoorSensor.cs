using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

[RequireComponent(typeof(DoorController))]
public class DoorSensor : MonoBehaviour
{
    public float doorWidth = 10f;

    [Header("Observations")]
    [ReadOnly] public bool isOpen;
    [ReadOnly] public int seekerCount;
    [ReadOnly] public int hiderCount;
    [ReadOnly] public int hiderSeekerBalance;
    [ReadOnly] public Vector2 doorLeftCornerXZ;
    [ReadOnly] public Vector2 doorRightCornerXZ;
    [ReadOnly] public Vector2 doorCenterXZ;

    private DoorController doorController;

    void Awake()
    {
        doorController = GetComponent<DoorController>();
        if (doorController == null)
        {
            Debug.LogError("DoorController is missing!");
        }
    }

    void Start()
    {
        Vector3 left = transform.position - (transform.right * doorWidth / 2f);
        Vector3 right = transform.position + (transform.right * doorWidth / 2f);

        doorLeftCornerXZ = new Vector2(left.x, left.z);
        doorRightCornerXZ = new Vector2(right.x, right.z);
        doorCenterXZ = new Vector2(transform.position.x, transform.position.z);
    }
    void Update()
    {
        isOpen = doorController.IsOpen;
        seekerCount = doorController.plate.seekerCount;
        hiderCount = doorController.plate.hiderCount;
        hiderSeekerBalance = seekerCount - hiderCount;
    }
}

