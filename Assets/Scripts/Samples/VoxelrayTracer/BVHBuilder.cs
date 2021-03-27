using System;
using System.Diagnostics;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

public class BVHBuilder
{
    public enum Axis
    {
        Axis_X = 0,
        Axis_Y = 1,
        Axis_Z = 2,
    }

    public class TreeLeaf
    {
        public List<int> m_RimitiveList;
    }

    public class TreeNode
    {
        public AABB m_AABB;
        public TreeNode m_LNode;
        public TreeNode m_RNode;
        public TreeLeaf m_Leaf;
    }

    public struct TreeNodeData
    {
        public Vector3 Min;
        public Vector3 Max;
        public int LNode;
        public int RNode;
    }

    public struct TreeLeafData
    {
        public Vector3 Min;
        public Vector3 Max;
        public int StartPrimitive;
        public int NumPrimitives;
    }

    public int m_MaxPrimitivesPerLeaf = 64;
    public int m_MaxTreeDepth = 32;
    public int m_NumNodes;
    public int m_NumLeaves;
    public int m_NumPrimitives;
    public int m_TreeDepth;
    public long m_BuidTime;

    TreeNode m_RootNode;
    VOXLoader.VoxelData[] m_VoxelArray;
    bool[] m_ProcessedVoxelArray;

    int m_CNode;
    public TreeNodeData[] m_NodeArray;
    int m_CLeaf;
    public TreeLeafData[] m_LeafArray;
    int m_CPrimitive;
    public VOXLoader.VoxelData[] m_LeafPrimitiveArray;

    //----------------------------------------------------------------------------------------
    // 
    //----------------------------------------------------------------------------------------
    void SplitNode(TreeNode node)
    {
        Axis SelectedAxis;

        Vector3 Size = node.m_AABB.Max - node.m_AABB.Min;

        if (Size.x >= Size.y && Size.x >= Size.z)
        {
            SelectedAxis = Axis.Axis_X;
        }
        else if (Size.y >= Size.x && Size.y >= Size.z)
        {
            SelectedAxis = Axis.Axis_Y;
        }
        else
        {
            SelectedAxis = Axis.Axis_Z;
        }

        AABB LAABB = new AABB(Vector3.zero, Vector3.zero);
        AABB RAABB = new AABB(Vector3.zero, Vector3.zero);

        float Len, LLen, RLen;

        switch (SelectedAxis)
        {
            case Axis.Axis_X:
                Len = (node.m_AABB.Max.x - node.m_AABB.Min.x);
                LLen = UnityEngine.Mathf.Ceil(Len * 0.5f);
                LAABB.Min = node.m_AABB.Min;
                LAABB.Max = new Vector3(LAABB.Min.x + LLen, node.m_AABB.Max.y, node.m_AABB.Max.z);

                RLen = Len - LLen;
                RAABB.Max = node.m_AABB.Max;
                RAABB.Min = new Vector3(RAABB.Max.x - RLen, node.m_AABB.Min.y, node.m_AABB.Min.z);
                break;

            case Axis.Axis_Y:
                Len = (node.m_AABB.Max.y - node.m_AABB.Min.y);
                LLen = UnityEngine.Mathf.Ceil(Len * 0.5f);
                LAABB.Min = node.m_AABB.Min;
                LAABB.Max = new Vector3(node.m_AABB.Max.x, LAABB.Min.y + LLen, node.m_AABB.Max.z);

                RLen = Len - LLen;
                RAABB.Max = node.m_AABB.Max;
                RAABB.Min = new Vector3(node.m_AABB.Min.x, RAABB.Max.y - RLen, node.m_AABB.Min.z);
                break;

            case Axis.Axis_Z:
                Len = (node.m_AABB.Max.z - node.m_AABB.Min.z);
                LLen = UnityEngine.Mathf.Ceil(Len * 0.5f);
                LAABB.Min = node.m_AABB.Min;
                LAABB.Max = new Vector3(node.m_AABB.Max.x, node.m_AABB.Max.y, LAABB.Min.z + LLen);

                RLen = Len - LLen;
                RAABB.Max = node.m_AABB.Max;
                RAABB.Min = new Vector3(node.m_AABB.Min.x, node.m_AABB.Min.y, RAABB.Max.z - RLen);
                break;
        }

        List<int> VoxelList = new List<int>();

        AABB NodeAABB = new AABB(Vector3.zero, Vector3.zero);
        for (int v = 0; v < m_VoxelArray.Length; v++)
        {
            if (!m_ProcessedVoxelArray[v])
            {
                if (m_VoxelArray[v].Pos.x >= LAABB.Min.x && m_VoxelArray[v].Pos.x <= LAABB.Max.x &&
                    m_VoxelArray[v].Pos.y >= LAABB.Min.y && m_VoxelArray[v].Pos.y <= LAABB.Max.y &&
                    m_VoxelArray[v].Pos.z >= LAABB.Min.z && m_VoxelArray[v].Pos.z <= LAABB.Max.z)
                {
                    VoxelList.Add(v);
                    if (VoxelList.Count == 1)
                    {
                        NodeAABB.Min = m_VoxelArray[v].Pos - new Vector3(0.5f, 0.5f, 0.5f);
                        NodeAABB.Max = m_VoxelArray[v].Pos + new Vector3(0.5f, 0.5f, 0.5f);
                    }
                    else
                    {
                        NodeAABB.Add(new AABB(m_VoxelArray[v].Pos, new Vector3(0.5f, 0.5f, 0.5f)));
                    }
                }
            }
        }
        if (VoxelList.Count <= m_MaxPrimitivesPerLeaf) //is leaf
        {
            node.m_LNode = new TreeNode();
            node.m_LNode.m_AABB = new AABB(Vector3.zero, Vector3.zero);
            node.m_LNode.m_AABB.Min = NodeAABB.Min;
            node.m_LNode.m_AABB.Max = NodeAABB.Max;

            node.m_LNode.m_Leaf = new TreeLeaf();
            m_NumLeaves++;
            node.m_LNode.m_Leaf.m_RimitiveList = new List<int>();
            for (int v = 0; v < VoxelList.Count; v++)
            {
                node.m_LNode.m_Leaf.m_RimitiveList.Add(VoxelList[v]);
                m_ProcessedVoxelArray[VoxelList[v]] = true;
            }

            m_NumPrimitives += node.m_LNode.m_Leaf.m_RimitiveList.Count;
        }
        else
        {
            node.m_LNode = new TreeNode();
            m_NumNodes++;
            node.m_LNode.m_AABB = new AABB(Vector3.zero, Vector3.zero);
            node.m_LNode.m_AABB.Min = NodeAABB.Min;
            node.m_LNode.m_AABB.Max = NodeAABB.Max;
            SplitNode(node.m_LNode);
        }

        VoxelList.Clear();
        for (int v = 0; v < m_VoxelArray.Length; v++)
        {
            if (!m_ProcessedVoxelArray[v])
            {
                if (m_VoxelArray[v].Pos.x >= RAABB.Min.x && m_VoxelArray[v].Pos.x <= RAABB.Max.x &&
                    m_VoxelArray[v].Pos.y >= RAABB.Min.y && m_VoxelArray[v].Pos.y <= RAABB.Max.y &&
                    m_VoxelArray[v].Pos.z >= RAABB.Min.z && m_VoxelArray[v].Pos.z <= RAABB.Max.z)
                {
                    VoxelList.Add(v);
                    if (VoxelList.Count == 1)
                    {
                        NodeAABB.Min = m_VoxelArray[v].Pos - new Vector3(0.5f, 0.5f, 0.5f);
                        NodeAABB.Max = m_VoxelArray[v].Pos + new Vector3(0.5f, 0.5f, 0.5f);
                    }
                    else
                    {
                        NodeAABB.Add(new AABB(m_VoxelArray[v].Pos, new Vector3(0.5f, 0.5f, 0.5f)));
                    }
                }
            }
        }
        if (VoxelList.Count <= m_MaxPrimitivesPerLeaf) //is leaf
        {
            node.m_RNode = new TreeNode();
            node.m_RNode.m_AABB = new AABB(Vector3.zero, Vector3.zero);
            node.m_RNode.m_AABB.Min = NodeAABB.Min;
            node.m_RNode.m_AABB.Max = NodeAABB.Max;

            node.m_RNode.m_Leaf = new TreeLeaf();
            m_NumLeaves++;
            node.m_RNode.m_Leaf.m_RimitiveList = new List<int>();
            for (int v = 0; v < VoxelList.Count; v++)
            {
                node.m_RNode.m_Leaf.m_RimitiveList.Add(VoxelList[v]);
                m_ProcessedVoxelArray[VoxelList[v]] = true;
            }

            m_NumPrimitives += node.m_RNode.m_Leaf.m_RimitiveList.Count;
        }
        else
        {
            node.m_RNode = new TreeNode();
            m_NumNodes++;
            node.m_RNode.m_AABB = new AABB(Vector3.zero, Vector3.zero);
            node.m_RNode.m_AABB.Min = NodeAABB.Min;
            node.m_RNode.m_AABB.Max = NodeAABB.Max;
            SplitNode(node.m_RNode);
        }
    }
    //----------------------------------------------------------------------------------------
    // 
    //----------------------------------------------------------------------------------------
    public void Serialize(ref int parent, TreeNode node)
    {
        if (node.m_Leaf == null)
        {
            int Parent = m_CNode;
            m_NodeArray[m_CNode].Min = node.m_AABB.Min;
            m_NodeArray[m_CNode].Max = node.m_AABB.Max;
            m_NodeArray[m_CNode].LNode = -1;
            m_NodeArray[m_CNode].RNode = -1;
            parent = m_CNode;
            m_CNode++;

            Serialize(ref m_NodeArray[Parent].LNode, node.m_LNode);
            Serialize(ref m_NodeArray[Parent].RNode, node.m_RNode);
        }
        else
        {
            m_LeafArray[m_CLeaf].Min = node.m_AABB.Min;
            m_LeafArray[m_CLeaf].Max = node.m_AABB.Max;
            m_LeafArray[m_CLeaf].StartPrimitive = m_CPrimitive;
            m_LeafArray[m_CLeaf].NumPrimitives = node.m_Leaf.m_RimitiveList.Count;
            parent = -m_CLeaf - 1;

            for (int p = 0; p < node.m_Leaf.m_RimitiveList.Count; p++)
            {
                m_LeafPrimitiveArray[m_CPrimitive + p] = m_VoxelArray[node.m_Leaf.m_RimitiveList[p]];
            }
            m_CLeaf++;
            m_CPrimitive += node.m_Leaf.m_RimitiveList.Count;
        }
    }
    //----------------------------------------------------------------------------------------
    // 
    //----------------------------------------------------------------------------------------
    public void Serialize()
    {
        m_CNode = 0;
        m_NodeArray = new TreeNodeData[m_NumNodes];
        m_CLeaf = 0;
        m_LeafArray = new TreeLeafData[m_NumLeaves];
        m_CPrimitive = 0;
        m_LeafPrimitiveArray = new VOXLoader.VoxelData[m_NumPrimitives];

        int Root = 0;
        Serialize(ref Root, m_RootNode);
    }
    //----------------------------------------------------------------------------------------
    // 
    //----------------------------------------------------------------------------------------
    public int ComputeTreeDepth(TreeNode node)
    {
        if (node.m_Leaf != null)
            return 0;

        int LeftDepth = ComputeTreeDepth(node.m_LNode);
        int RightDepth = ComputeTreeDepth(node.m_RNode);

        if (LeftDepth > RightDepth)
            return LeftDepth + 1;
        else
            return RightDepth + 1;
    }
    //----------------------------------------------------------------------------------------
    // 
    //----------------------------------------------------------------------------------------
    public void ComputeTreeDepth()
    {
        m_TreeDepth = ComputeTreeDepth(m_RootNode);
    }
    //----------------------------------------------------------------------------------------
    // 
    //----------------------------------------------------------------------------------------
    public void Build(VOXLoader.VoxelData[] data, int max_primitive = 64, int max_depth = 32)
    {
        var Stopwatch = new Stopwatch();
        Stopwatch.Start();

        m_VoxelArray = data;
        m_MaxPrimitivesPerLeaf = max_primitive;

        m_NumNodes = 0;
        m_NumLeaves = 0;
        m_NumPrimitives = 0;

        AABB RootAABB = new AABB(data[0].Pos, new Vector3(0.5f, 0.5f, 0.5f));
        for (int i = 1; i < data.Length; i++)
        {
            RootAABB.Add(new AABB(data[i].Pos, new Vector3(0.5f, 0.5f, 0.5f)));
        }

        m_RootNode = new TreeNode();
        m_NumNodes++;
        m_RootNode.m_AABB = new AABB(Vector3.zero, Vector3.zero);
        m_RootNode.m_AABB.Min = RootAABB.Min;
        m_RootNode.m_AABB.Max = RootAABB.Max;

        m_ProcessedVoxelArray = new bool[m_VoxelArray.Length];
        for (int i = 0; i < m_ProcessedVoxelArray.Length; i++)
            m_ProcessedVoxelArray[i] = false;

        SplitNode(m_RootNode);

        ComputeTreeDepth();

        Serialize();

        Stopwatch.Stop();
        m_BuidTime = Stopwatch.ElapsedMilliseconds;
    }
    //----------------------------------------------------------------------------------------
    // 
    //----------------------------------------------------------------------------------------
    public void Save(string filename)
    {
        Stream Stream = File.OpenWrite(filename);
        BinaryWriter Writer = new BinaryWriter(Stream);

        Writer.Write(m_NumNodes);
        for (int i = 0; i < m_NodeArray.Length; i++)
        {
            Writer.Write(m_NodeArray[i].Min.x);
            Writer.Write(m_NodeArray[i].Min.y);
            Writer.Write(m_NodeArray[i].Min.z);

            Writer.Write(m_NodeArray[i].Max.x);
            Writer.Write(m_NodeArray[i].Max.y);
            Writer.Write(m_NodeArray[i].Max.z);

            Writer.Write(m_NodeArray[i].LNode);
            Writer.Write(m_NodeArray[i].RNode);
        }

        Writer.Write(m_NumLeaves);
        for (int i = 0; i < m_LeafArray.Length; i++)
        {
            Writer.Write(m_LeafArray[i].Min.x);
            Writer.Write(m_LeafArray[i].Min.y);
            Writer.Write(m_LeafArray[i].Min.z);

            Writer.Write(m_LeafArray[i].Max.x);
            Writer.Write(m_LeafArray[i].Max.y);
            Writer.Write(m_LeafArray[i].Max.z);

            Writer.Write(m_LeafArray[i].StartPrimitive);
            Writer.Write(m_LeafArray[i].NumPrimitives);
        }

        Writer.Write(m_NumPrimitives);
        for (int i = 0; i < m_LeafPrimitiveArray.Length; i++)
        {
            Writer.Write(m_LeafPrimitiveArray[i].Pos.x);
            Writer.Write(m_LeafPrimitiveArray[i].Pos.y);
            Writer.Write(m_LeafPrimitiveArray[i].Pos.z);

            Writer.Write(m_LeafPrimitiveArray[i].Albedo.x);
            Writer.Write(m_LeafPrimitiveArray[i].Albedo.y);
            Writer.Write(m_LeafPrimitiveArray[i].Albedo.z);
        }
        Writer.Write(m_TreeDepth);

        Stream.Close();
    }
    //----------------------------------------------------------------------------------------
    // 
    //----------------------------------------------------------------------------------------
    public bool Load(string filename)
    {
        if (!File.Exists(filename))
            return false;

        Stream Stream = File.OpenRead(filename);
        BinaryReader Reader = new BinaryReader(Stream);

        m_NumNodes = Reader.ReadInt32();
        m_NodeArray = new TreeNodeData[m_NumNodes];

        for (int i = 0; i < m_NodeArray.Length; i++)
        {
            m_NodeArray[i].Min.x = Reader.ReadSingle();
            m_NodeArray[i].Min.y = Reader.ReadSingle();
            m_NodeArray[i].Min.z = Reader.ReadSingle();

            m_NodeArray[i].Max.x = Reader.ReadSingle();
            m_NodeArray[i].Max.y = Reader.ReadSingle();
            m_NodeArray[i].Max.z = Reader.ReadSingle();

            m_NodeArray[i].LNode = Reader.ReadInt32();
            m_NodeArray[i].RNode = Reader.ReadInt32();
        }

        m_NumLeaves = Reader.ReadInt32();
        m_LeafArray = new TreeLeafData[m_NumLeaves];
        for (int i = 0; i < m_LeafArray.Length; i++)
        {
            m_LeafArray[i].Min.x = Reader.ReadSingle();
            m_LeafArray[i].Min.y = Reader.ReadSingle();
            m_LeafArray[i].Min.z = Reader.ReadSingle();

            m_LeafArray[i].Max.x = Reader.ReadSingle();
            m_LeafArray[i].Max.y = Reader.ReadSingle();
            m_LeafArray[i].Max.z = Reader.ReadSingle();

            m_LeafArray[i].StartPrimitive = Reader.ReadInt32();
            m_LeafArray[i].NumPrimitives = Reader.ReadInt32();
        }

        m_NumPrimitives = Reader.ReadInt32();
        m_LeafPrimitiveArray = new VOXLoader.VoxelData[m_NumPrimitives];
        for (int i = 0; i < m_LeafPrimitiveArray.Length; i++)
        {
            m_LeafPrimitiveArray[i].Pos.x = Reader.ReadSingle();
            m_LeafPrimitiveArray[i].Pos.y = Reader.ReadSingle();
            m_LeafPrimitiveArray[i].Pos.z = Reader.ReadSingle();

            m_LeafPrimitiveArray[i].Albedo.x = Reader.ReadSingle();
            m_LeafPrimitiveArray[i].Albedo.y = Reader.ReadSingle();
            m_LeafPrimitiveArray[i].Albedo.z = Reader.ReadSingle();
        }
        m_TreeDepth = Reader.ReadInt32();

        Stream.Close();

        return true;
    }
}