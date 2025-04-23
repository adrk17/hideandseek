using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class RectVisualizer : MonoBehaviour
{
    public AgentGroupManager agentGroupManager;

    public bool drawHiderRects = true;
    public bool drawSeekerRects = true;

    public Color seekerColor = new Color(1f, 0f, 0f, 0.3f);
    public Color hiderColor = new Color(0f, 0f, 1f, 0.3f);

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (agentGroupManager == null)
            return;

        if (drawHiderRects)
        {
            DrawRects(agentGroupManager.randomPositionsHider, hiderColor);
        }

        if (drawSeekerRects)
        {
            DrawRects(agentGroupManager.randomPositionsSeeker, seekerColor);
        }
    }

    private void DrawRects(List<Rect> rects, Color baseColor)
    {
        foreach (Rect rect in rects)
        {
            Vector3 bottomLeft = new Vector3(rect.xMin, 0f, rect.yMin);
            Vector3 bottomRight = new Vector3(rect.xMax, 0f, rect.yMin);
            Vector3 topRight = new Vector3(rect.xMax, 0f, rect.yMax);
            Vector3 topLeft = new Vector3(rect.xMin, 0f, rect.yMax);
            
            Gizmos.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0.3f);
            Vector3 center = (bottomLeft + topRight) / 2f;
            Vector3 size = new Vector3(rect.width, 0.01f, rect.height);
            Gizmos.DrawCube(center, size);
            
            Gizmos.color = new Color(baseColor.r, baseColor.g, baseColor.b, 1f);
            Gizmos.DrawLine(bottomLeft, bottomRight);
            Gizmos.DrawLine(bottomRight, topRight);
            Gizmos.DrawLine(topRight, topLeft);
            Gizmos.DrawLine(topLeft, bottomLeft);
        }
    }
#endif
}