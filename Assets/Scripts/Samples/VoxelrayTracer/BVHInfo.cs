using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BVHInfo : MonoBehaviour
{
    public VoxelRayTracer m_VoxelRayTracer;
    public Color m_TextColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    float deltaTime = 0.0f;

    void Start()
    {
    }

    public void Update()
    {
        
    }

    void OnGUI()
    {
        if (m_VoxelRayTracer.m_bUseBVH)
        {
            int w = Screen.width, h = Screen.height;
            GUIStyle style = new GUIStyle();
            Rect rect = new Rect(0, h * 0.1f, w * 0.1f, h * 0.3f);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = h * 1 / 30;
            style.normal.textColor = m_TextColor;
            string text = string.Format("Voxels: {0:0}\n\nBVH Info\n===========\nPrimitives: {1:0}\nNodes: {2:0}\nLeaves: {3:0}\nDepth: {4:0}\nOptimization Time: {5:0} ms\nBuild Time: {6:0} ms",
                m_VoxelRayTracer.m_VOXLoader.m_NumVoxels,
                m_VoxelRayTracer.m_Builder.m_NumPrimitives,
                m_VoxelRayTracer.m_Builder.m_NumNodes,
                m_VoxelRayTracer.m_Builder.m_NumLeaves,
                m_VoxelRayTracer.m_Builder.m_TreeDepth,
                m_VoxelRayTracer.m_VOXLoader.m_OptimizationTime,
                m_VoxelRayTracer.m_Builder.m_BuidTime);
            GUI.Label(rect, text, style);
        }
        else
        {
            int w = Screen.width, h = Screen.height;
            GUIStyle style = new GUIStyle();
            Rect rect = new Rect(0, h * 0.1f, w * 0.1f, h * 0.3f);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = h * 1 / 30;
            style.normal.textColor = m_TextColor;
            string text = string.Format("Voxels: {0:0}", m_VoxelRayTracer.m_VOXLoader.m_NumVoxels);
            GUI.Label(rect, text, style);

        }
    }
}
