using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionIndicator : MonoBehaviour
{
    [Header("Floating Settings")]
    public float floatAmplitude = 1f;   
    public float floatFrequency = 2f;     

    [Header("Rotation Settings")]
    public float rotationSpeed = 60f;   

    private Vector3 _startPos;

    void Start()
    {
        _startPos = transform.localPosition;
    }

    void Update()
    {
        float yOffset = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.localPosition = _startPos + new Vector3(0f, yOffset, 0f);
        
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);
    }
}
