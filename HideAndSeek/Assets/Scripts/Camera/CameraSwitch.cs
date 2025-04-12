using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    public List<Camera> cameras;
    public int currentCameraIndex = 0;
    public KeyCode switchKey = KeyCode.Tab;
    void Start()
    {
        for (int i = 0; i < cameras.Count; i++)
        {
            cameras[i].gameObject.SetActive(i == currentCameraIndex);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(switchKey))
        {
            cameras[currentCameraIndex].gameObject.SetActive(false);
            currentCameraIndex = (currentCameraIndex + 1) % cameras.Count;
            cameras[currentCameraIndex].gameObject.SetActive(true);
        }
    }
}
