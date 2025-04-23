using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public int seekerCount { get; private set; }
    public int hiderCount { get; private set; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Seeker")) seekerCount++;
        if (other.CompareTag("Hider")) hiderCount++;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Seeker")) seekerCount--;
        if (other.CompareTag("Hider")) hiderCount--;
    }
}