using System.Collections.Generic;
using UnityEngine;

public class AgentGroupManager : MonoBehaviour
{
    [System.Serializable]
    public class AgentEntry
    {
        public AgentMovement agent;
        public Vector3 customStartPosition = Vector3.zero; // zero = no override, the default start position from AgentMovement will be used
    }

    [Tooltip("Agent list with custom start positions")]
    public List<AgentEntry> agents = new List<AgentEntry>();

    public void ResetAllAgents()
    {
        foreach (var entry in agents)
        {
            if (entry.agent != null)
            {
                // If the custom start position is not zero, use it
                if (entry.customStartPosition != Vector3.zero)
                {
                    entry.agent.startPosition = entry.customStartPosition;
                }

                entry.agent.ResetPosition();
            }
        }
    }
}