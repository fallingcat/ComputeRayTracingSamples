using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

public class VoxelRayTracer : MonoBehaviour
{    
    public enum MaterialType
    {
        MAT_LAMBERTIAN = 0,
        MAT_METAL = 1,
        MAT_DIELECTRIC = 2,
    }    

    public struct AABBTreeNode
    {
        public AABB m_AABB;
        public List<int> m_VoxelList;
    }

    public struct AABBTreeNodeData
    {
        public Vector3 Min;
        public Vector3 Max;
        public int NumVoxels;
        public int StartVoxel;
    }

    public Material m_QuadMaterial;
    public ComputeShader m_ComputeShader;
    public Vector2Int m_RTSize;
    public Color m_AmbientColor;
    public Light m_PointLight;
    public bool m_bOptimizeVoxelData = false;
    public bool m_bRebuildBVH = false;
    public int m_BVHMaxPrimitivesPerLeaf = 64;    
    public bool m_bUseBVH = false;    

    public VOXLoader m_VOXLoader = new VOXLoader();
    RenderTexture m_RTTarget;
    public BVHBuilder m_Builder = new BVHBuilder();

    string m_VOXFilename = "vox/monument/monu9.vox";
    //string m_VOXFilename = "vox/ruin.vox";
    //string m_VOXFilename = "vox/monument/monu8-without-water.vox";    
    //string m_VOXFilename = "vox/monument/monu10.vox";
    //string m_VOXFilename = "vox/monument/monu7.vox";
    //string m_VOXFilename = "vox/monument/monu16.vox";
    int m_KernelHandle;  
    ComputeBuffer m_TreeNodeBuffer;
    ComputeBuffer m_TreeLeafBuffer;
    ComputeBuffer m_TreeLeafVoxelBuffer;

    //----------------------------------------------------------------------------------------
    // 
    //----------------------------------------------------------------------------------------
    public void OnUseBVH(bool v)
    {
        m_bUseBVH = v;
    }
    //----------------------------------------------------------------------------------------
    // 
    //----------------------------------------------------------------------------------------
    void Start()
    {
        m_RTTarget = new RenderTexture(m_RTSize.x, m_RTSize.y, 0, UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_UNorm);
        m_RTTarget.enableRandomWrite = true;
        m_RTTarget.Create();
        m_QuadMaterial.SetTexture("_MainTex", m_RTTarget);

        if (m_bRebuildBVH || !m_Builder.Load(System.IO.Path.Combine(Application.streamingAssetsPath, m_VOXFilename + ".bvh")))
        {
            VOXLoader.LoadOption Option;
            Option.m_bOptimize = m_bOptimizeVoxelData;
            m_VOXLoader.Load(System.IO.Path.Combine(Application.streamingAssetsPath, m_VOXFilename), Option);            
            m_Builder.Build(m_VOXLoader.m_VoxelArray, m_BVHMaxPrimitivesPerLeaf);
            m_Builder.Save(System.IO.Path.Combine(Application.streamingAssetsPath, m_VOXFilename + ".bvh"));
        }

        m_TreeNodeBuffer = new ComputeBuffer(m_Builder.m_NumNodes, System.Runtime.InteropServices.Marshal.SizeOf<BVHBuilder.TreeNodeData>());
        m_TreeNodeBuffer.SetData(m_Builder.m_NodeArray);

        m_TreeLeafBuffer = new ComputeBuffer(m_Builder.m_NumLeaves, System.Runtime.InteropServices.Marshal.SizeOf<BVHBuilder.TreeLeafData>());
        m_TreeLeafBuffer.SetData(m_Builder.m_LeafArray);

        m_TreeLeafVoxelBuffer = new ComputeBuffer(m_Builder.m_NumPrimitives, System.Runtime.InteropServices.Marshal.SizeOf<VOXLoader.VoxelData>());
        m_TreeLeafVoxelBuffer.SetData(m_Builder.m_LeafPrimitiveArray);
        
        m_KernelHandle = m_ComputeShader.FindKernel("CSMain");

        m_ComputeShader.SetBuffer(m_KernelHandle, "Node", m_TreeNodeBuffer);
        m_ComputeShader.SetBuffer(m_KernelHandle, "Leaf", m_TreeLeafBuffer);
        m_ComputeShader.SetBuffer(m_KernelHandle, "BLAS", m_TreeLeafVoxelBuffer);
        m_ComputeShader.SetTexture(m_KernelHandle, "Result", m_RTTarget);
    }
    //----------------------------------------------------------------------------------------
    // 
    //----------------------------------------------------------------------------------------
    void Update()
    {
        m_ComputeShader.SetVector("TargetSize", new Vector4(m_RTSize.x, m_RTSize.y, UnityEngine.Mathf.Sin(Time.time * 10.0f), Time.time));
        m_ComputeShader.SetVector("TreeSize", new Vector4(m_Builder.m_NumNodes, m_Builder.m_NumLeaves, 0, 0));
        m_ComputeShader.SetVector("PointLightPos", new Vector4(m_PointLight.transform.position.x, m_PointLight.transform.position.y, m_PointLight.transform.position.z, 0.0f));
        m_ComputeShader.SetVector("PointLightColor", new Vector4(m_PointLight.color.r, m_PointLight.color.g, m_PointLight.color.b, m_PointLight.intensity));
        m_ComputeShader.SetVector("AmbientColor", m_AmbientColor);
        m_ComputeShader.SetBool("bUseBVH", m_bUseBVH);        
        m_ComputeShader.Dispatch(m_KernelHandle, m_RTSize.x / 8, m_RTSize.y / 8, 1);
    }
}
