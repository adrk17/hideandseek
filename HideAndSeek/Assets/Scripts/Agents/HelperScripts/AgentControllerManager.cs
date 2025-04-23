using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentControllerManager : MonoBehaviour
{
    [Header("Input Settings")]
    public int selectAgentMouseButtonIndex = 0; // 0 = LPM, 1 = PPM
    public int deselectAgentMouseButtonIndex = 1;
    public KeyCode upKey = KeyCode.W;
    public KeyCode downKey = KeyCode.S;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;

    [Header("Selection Indicator")]
    public GameObject selectionIndicatorPrefab;

    private AgentMovement _activeAgent;
    private GameObject _currentIndicator;

    void FixedUpdate()
    {
        if (_activeAgent != null)
        {
            Vector2 input = Vector2.zero;

            if (Input.GetKey(upKey)) input.x -= 1f;
            if (Input.GetKey(downKey)) input.x += 1f;
            if (Input.GetKey(rightKey)) input.y += 1f;
            if (Input.GetKey(leftKey)) input.y -= 1f;

            _activeAgent.Move(input);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(deselectAgentMouseButtonIndex))
        {
            DeselectActiveAgent();
        }
        
        if (Input.GetMouseButtonDown(selectAgentMouseButtonIndex))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                AgentMovement clickedAgent = hit.collider.GetComponent<AgentMovement>();
                if (clickedAgent != null)
                {
                    SetActiveAgent(clickedAgent);
                }
            }
        }
    }

    void SetActiveAgent(AgentMovement newAgent)
    {
        _activeAgent = newAgent;

        if (_currentIndicator != null)
        {
            Destroy(_currentIndicator);
        }

        if (selectionIndicatorPrefab != null)
        {
            _currentIndicator = Instantiate(selectionIndicatorPrefab, _activeAgent.transform, true);
            _currentIndicator.transform.localPosition = new Vector3(0, 3f, 0);
        }
    }
    
    void DeselectActiveAgent()
    {
        if (_currentIndicator != null)
        {
            Destroy(_currentIndicator);
            _currentIndicator = null;
        }

        _activeAgent = null;
    }
}
