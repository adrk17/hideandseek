using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class AgentGroupManager : MonoBehaviour
{
    [Serializable]
    public class AgentEntry
    {
        public AgentMovement agent;
        public Vector3 customStartPosition = Vector3.zero; // Zero = no override, use internal agent start position
    }

    [Tooltip("List of agents with optional custom start positions")]
    public List<AgentEntry> agents = new List<AgentEntry>();

    [Header("Random start position settings")]
    public bool randomPositions = false;

    public List<Rect> randomPositionsSeeker = new List<Rect>();
    public List<Rect> randomPositionsHider = new List<Rect>();

    public float minAgentDistance = 3.1f;   // Minimum distance between agents
    public int maxSpawnAttempts = 10;       // Max attempts to find non-overlapping spawn

    [SerializeField, ReadOnly]
    private bool agentsInitialized = false;
    public bool AgentsInitialized => agentsInitialized;

    public void ResetAllAgents()
    {
        if (!agentsInitialized)
        {
            Debug.LogWarning("AgentGroupManager: Attempted to reset agents before initialization.");
            return;
        }

        foreach (var entry in agents)
        {
            if (entry.agent != null)
            {
                // Use custom start position if set
                if (entry.customStartPosition != Vector3.zero)
                {
                    entry.agent.startPosition = entry.customStartPosition;
                    Debug.Log($"Agent {entry.agent.name} has a custom start position: {entry.customStartPosition}");
                }

                entry.agent.ResetPosition();
            }
        }
    }

    private void Start()
    {
        if (randomPositions)
        {
            PopulateAgentList();
            SetRandomStartPositions();
        }

        agentsInitialized = true;
    }

    private void PopulateAgentList()
    {
        agents.Clear();
        var agentMovements = GetComponentsInChildren<AgentMovement>();
        foreach (var agentMovement in agentMovements)
        {
            agents.Add(new AgentEntry
            {
                agent = agentMovement
            });
        }
    }

    public void SetRandomStartPositions()
    {
        List<Vector3> usedPositions = new List<Vector3>();

        foreach (var entry in agents)
        {
            if (entry.agent == null) continue;

            List<Rect> sourceRects = entry.agent.CompareTag("Seeker") ? randomPositionsSeeker : randomPositionsHider;

            Vector2 random2D = GetRandomNonOverlappingPosition(sourceRects, usedPositions, minAgentDistance, maxSpawnAttempts);
            Vector3 newPos = new Vector3(random2D.x, 2f, random2D.y);

            // Save to customStartPosition so ResetAllAgents uses it
            entry.customStartPosition = newPos;

            usedPositions.Add(newPos);
        }
    }

    private Vector2 GetRandomNonOverlappingPosition(List<Rect> rects, List<Vector3> used, float minDistance, int maxTries)
    {
        for (int attempt = 0; attempt < maxTries; attempt++)
        {
            Vector2 pos = GetRandomPositionFromRects(rects);

            bool tooClose = false;
            foreach (var existing in used)
            {
                float dist = Vector2.Distance(pos, new Vector2(existing.x, existing.z));
                if (dist < minDistance)
                {
                    tooClose = true;
                    break;
                }
            }

            if (!tooClose)
                return pos;
        }

        Debug.LogWarning("Could not find non-overlapping position after max attempts. Agent may overlap.");
        return GetRandomPositionFromRects(rects);
    }

    private static Vector2 GetRandomPositionFromRects(List<Rect> rects)
    {
        if (rects == null || rects.Count == 0)
        {
            Debug.LogWarning("No rects provided for random position generation.");
            return Vector2.zero;
        }

        float totalArea = 0f;
        List<float> cumulativeAreas = new List<float>();

        foreach (Rect rect in rects)
        {
            float area = rect.width * rect.height;
            totalArea += area;
            cumulativeAreas.Add(totalArea);
        }

        float randomPick = UnityEngine.Random.Range(0f, totalArea);

        for (int i = 0; i < rects.Count; i++)
        {
            if (randomPick < cumulativeAreas[i])
            {
                Rect chosen = rects[i];
                float x = UnityEngine.Random.Range(chosen.xMin, chosen.xMax);
                float y = UnityEngine.Random.Range(chosen.yMin, chosen.yMax);
                return new Vector2(x, y);
            }
        }

        return Vector2.zero;
    }
}
