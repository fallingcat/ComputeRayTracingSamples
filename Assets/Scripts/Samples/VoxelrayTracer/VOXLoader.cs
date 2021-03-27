using System;
using System.Diagnostics;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

public class VOXLoader
{
    public struct LoadOption
    {
        public bool m_bOptimize;
    }

    public struct VoxelData
    {
        public Vector3 Pos;
        public Vector3 Albedo;
    }

    public static Color[] m_MagicVoxelPalatte = new Color[]
    {
        new Color(1.000000f, 1.000000f, 1.000000f),
        new Color(1.000000f, 1.000000f, 0.800000f),
        new Color(1.000000f, 1.000000f, 0.600000f),
        new Color(1.000000f, 1.000000f, 0.400000f),
        new Color(1.000000f, 1.000000f, 0.200000f),
        new Color(1.000000f, 1.000000f, 0.000000f),
        new Color(1.000000f, 0.800000f, 1.000000f),
        new Color(1.000000f, 0.800000f, 0.800000f),
        new Color(1.000000f, 0.800000f, 0.600000f),
        new Color(1.000000f, 0.800000f, 0.400000f),
        new Color(1.000000f, 0.800000f, 0.200000f),
        new Color(1.000000f, 0.800000f, 0.000000f),
        new Color(1.000000f, 0.600000f, 1.000000f),
        new Color(1.000000f, 0.600000f, 0.800000f),
        new Color(1.000000f, 0.600000f, 0.600000f),
        new Color(1.000000f, 0.600000f, 0.400000f),
        new Color(1.000000f, 0.600000f, 0.200000f),
        new Color(1.000000f, 0.600000f, 0.000000f),
        new Color(1.000000f, 0.400000f, 1.000000f),
        new Color(1.000000f, 0.400000f, 0.800000f),
        new Color(1.000000f, 0.400000f, 0.600000f),
        new Color(1.000000f, 0.400000f, 0.400000f),
        new Color(1.000000f, 0.400000f, 0.200000f),
        new Color(1.000000f, 0.400000f, 0.000000f),
        new Color(1.000000f, 0.200000f, 1.000000f),
        new Color(1.000000f, 0.200000f, 0.800000f),
        new Color(1.000000f, 0.200000f, 0.600000f),
        new Color(1.000000f, 0.200000f, 0.400000f),
        new Color(1.000000f, 0.200000f, 0.200000f),
        new Color(1.000000f, 0.200000f, 0.000000f),
        new Color(1.000000f, 0.000000f, 1.000000f),
        new Color(1.000000f, 0.000000f, 0.800000f),
        new Color(1.000000f, 0.000000f, 0.600000f),
        new Color(1.000000f, 0.000000f, 0.400000f),
        new Color(1.000000f, 0.000000f, 0.200000f),
        new Color(1.000000f, 0.000000f, 0.000000f),
        new Color(0.800000f, 1.000000f, 1.000000f),
        new Color(0.800000f, 1.000000f, 0.800000f),
        new Color(0.800000f, 1.000000f, 0.600000f),
        new Color(0.800000f, 1.000000f, 0.400000f),
        new Color(0.800000f, 1.000000f, 0.200000f),
        new Color(0.800000f, 1.000000f, 0.000000f),
        new Color(0.800000f, 0.800000f, 1.000000f),
        new Color(0.800000f, 0.800000f, 0.800000f),
        new Color(0.800000f, 0.800000f, 0.600000f),
        new Color(0.800000f, 0.800000f, 0.400000f),
        new Color(0.800000f, 0.800000f, 0.200000f),
        new Color(0.800000f, 0.800000f, 0.000000f),
        new Color(0.800000f, 0.600000f, 1.000000f),
        new Color(0.800000f, 0.600000f, 0.800000f),
        new Color(0.800000f, 0.600000f, 0.600000f),
        new Color(0.800000f, 0.600000f, 0.400000f),
        new Color(0.800000f, 0.600000f, 0.200000f),
        new Color(0.800000f, 0.600000f, 0.000000f),
        new Color(0.800000f, 0.400000f, 1.000000f),
        new Color(0.800000f, 0.400000f, 0.800000f),
        new Color(0.800000f, 0.400000f, 0.600000f),
        new Color(0.800000f, 0.400000f, 0.400000f),
        new Color(0.800000f, 0.400000f, 0.200000f),
        new Color(0.800000f, 0.400000f, 0.000000f),
        new Color(0.800000f, 0.200000f, 1.000000f),
        new Color(0.800000f, 0.200000f, 0.800000f),
        new Color(0.800000f, 0.200000f, 0.600000f),
        new Color(0.800000f, 0.200000f, 0.400000f),
        new Color(0.800000f, 0.200000f, 0.200000f),
        new Color(0.800000f, 0.200000f, 0.000000f),
        new Color(0.800000f, 0.000000f, 1.000000f),
        new Color(0.800000f, 0.000000f, 0.800000f),
        new Color(0.800000f, 0.000000f, 0.600000f),
        new Color(0.800000f, 0.000000f, 0.400000f),
        new Color(0.800000f, 0.000000f, 0.200000f),
        new Color(0.800000f, 0.000000f, 0.000000f),
        new Color(0.600000f, 1.000000f, 1.000000f),
        new Color(0.600000f, 1.000000f, 0.800000f),
        new Color(0.600000f, 1.000000f, 0.600000f),
        new Color(0.600000f, 1.000000f, 0.400000f),
        new Color(0.600000f, 1.000000f, 0.200000f),
        new Color(0.600000f, 1.000000f, 0.000000f),
        new Color(0.600000f, 0.800000f, 1.000000f),
        new Color(0.600000f, 0.800000f, 0.800000f),
        new Color(0.600000f, 0.800000f, 0.600000f),
        new Color(0.600000f, 0.800000f, 0.400000f),
        new Color(0.600000f, 0.800000f, 0.200000f),
        new Color(0.600000f, 0.800000f, 0.000000f),
        new Color(0.600000f, 0.600000f, 1.000000f),
        new Color(0.600000f, 0.600000f, 0.800000f),
        new Color(0.600000f, 0.600000f, 0.600000f),
        new Color(0.600000f, 0.600000f, 0.400000f),
        new Color(0.600000f, 0.600000f, 0.200000f),
        new Color(0.600000f, 0.600000f, 0.000000f),
        new Color(0.600000f, 0.400000f, 1.000000f),
        new Color(0.600000f, 0.400000f, 0.800000f),
        new Color(0.600000f, 0.400000f, 0.600000f),
        new Color(0.600000f, 0.400000f, 0.400000f),
        new Color(0.600000f, 0.400000f, 0.200000f),
        new Color(0.600000f, 0.400000f, 0.000000f),
        new Color(0.600000f, 0.200000f, 1.000000f),
        new Color(0.600000f, 0.200000f, 0.800000f),
        new Color(0.600000f, 0.200000f, 0.600000f),
        new Color(0.600000f, 0.200000f, 0.400000f),
        new Color(0.600000f, 0.200000f, 0.200000f),
        new Color(0.600000f, 0.200000f, 0.000000f),
        new Color(0.600000f, 0.000000f, 1.000000f),
        new Color(0.600000f, 0.000000f, 0.800000f),
        new Color(0.600000f, 0.000000f, 0.600000f),
        new Color(0.600000f, 0.000000f, 0.400000f),
        new Color(0.600000f, 0.000000f, 0.200000f),
        new Color(0.600000f, 0.000000f, 0.000000f),
        new Color(0.400000f, 1.000000f, 1.000000f),
        new Color(0.400000f, 1.000000f, 0.800000f),
        new Color(0.400000f, 1.000000f, 0.600000f),
        new Color(0.400000f, 1.000000f, 0.400000f),
        new Color(0.400000f, 1.000000f, 0.200000f),
        new Color(0.400000f, 1.000000f, 0.000000f),
        new Color(0.400000f, 0.800000f, 1.000000f),
        new Color(0.400000f, 0.800000f, 0.800000f),
        new Color(0.400000f, 0.800000f, 0.600000f),
        new Color(0.400000f, 0.800000f, 0.400000f),
        new Color(0.400000f, 0.800000f, 0.200000f),
        new Color(0.400000f, 0.800000f, 0.000000f),
        new Color(0.400000f, 0.600000f, 1.000000f),
        new Color(0.400000f, 0.600000f, 0.800000f),
        new Color(0.400000f, 0.600000f, 0.600000f),
        new Color(0.400000f, 0.600000f, 0.400000f),
        new Color(0.400000f, 0.600000f, 0.200000f),
        new Color(0.400000f, 0.600000f, 0.000000f),
        new Color(0.400000f, 0.400000f, 1.000000f),
        new Color(0.400000f, 0.400000f, 0.800000f),
        new Color(0.400000f, 0.400000f, 0.600000f),
        new Color(0.400000f, 0.400000f, 0.400000f),
        new Color(0.400000f, 0.400000f, 0.200000f),
        new Color(0.400000f, 0.400000f, 0.000000f),
        new Color(0.400000f, 0.200000f, 1.000000f),
        new Color(0.400000f, 0.200000f, 0.800000f),
        new Color(0.400000f, 0.200000f, 0.600000f),
        new Color(0.400000f, 0.200000f, 0.400000f),
        new Color(0.400000f, 0.200000f, 0.200000f),
        new Color(0.400000f, 0.200000f, 0.000000f),
        new Color(0.400000f, 0.000000f, 1.000000f),
        new Color(0.400000f, 0.000000f, 0.800000f),
        new Color(0.400000f, 0.000000f, 0.600000f),
        new Color(0.400000f, 0.000000f, 0.400000f),
        new Color(0.400000f, 0.000000f, 0.200000f),
        new Color(0.400000f, 0.000000f, 0.000000f),
        new Color(0.200000f, 1.000000f, 1.000000f),
        new Color(0.200000f, 1.000000f, 0.800000f),
        new Color(0.200000f, 1.000000f, 0.600000f),
        new Color(0.200000f, 1.000000f, 0.400000f),
        new Color(0.200000f, 1.000000f, 0.200000f),
        new Color(0.200000f, 1.000000f, 0.000000f),
        new Color(0.200000f, 0.800000f, 1.000000f),
        new Color(0.200000f, 0.800000f, 0.800000f),
        new Color(0.200000f, 0.800000f, 0.600000f),
        new Color(0.200000f, 0.800000f, 0.400000f),
        new Color(0.200000f, 0.800000f, 0.200000f),
        new Color(0.200000f, 0.800000f, 0.000000f),
        new Color(0.200000f, 0.600000f, 1.000000f),
        new Color(0.200000f, 0.600000f, 0.800000f),
        new Color(0.200000f, 0.600000f, 0.600000f),
        new Color(0.200000f, 0.600000f, 0.400000f),
        new Color(0.200000f, 0.600000f, 0.200000f),
        new Color(0.200000f, 0.600000f, 0.000000f),
        new Color(0.200000f, 0.400000f, 1.000000f),
        new Color(0.200000f, 0.400000f, 0.800000f),
        new Color(0.200000f, 0.400000f, 0.600000f),
        new Color(0.200000f, 0.400000f, 0.400000f),
        new Color(0.200000f, 0.400000f, 0.200000f),
        new Color(0.200000f, 0.400000f, 0.000000f),
        new Color(0.200000f, 0.200000f, 1.000000f),
        new Color(0.200000f, 0.200000f, 0.800000f),
        new Color(0.200000f, 0.200000f, 0.600000f),
        new Color(0.200000f, 0.200000f, 0.400000f),
        new Color(0.200000f, 0.200000f, 0.200000f),
        new Color(0.200000f, 0.200000f, 0.000000f),
        new Color(0.200000f, 0.000000f, 1.000000f),
        new Color(0.200000f, 0.000000f, 0.800000f),
        new Color(0.200000f, 0.000000f, 0.600000f),
        new Color(0.200000f, 0.000000f, 0.400000f),
        new Color(0.200000f, 0.000000f, 0.200000f),
        new Color(0.200000f, 0.000000f, 0.000000f),
        new Color(0.000000f, 1.000000f, 1.000000f),
        new Color(0.000000f, 1.000000f, 0.800000f),
        new Color(0.000000f, 1.000000f, 0.600000f),
        new Color(0.000000f, 1.000000f, 0.400000f),
        new Color(0.000000f, 1.000000f, 0.200000f),
        new Color(0.000000f, 1.000000f, 0.000000f),
        new Color(0.000000f, 0.800000f, 1.000000f),
        new Color(0.000000f, 0.800000f, 0.800000f),
        new Color(0.000000f, 0.800000f, 0.600000f),
        new Color(0.000000f, 0.800000f, 0.400000f),
        new Color(0.000000f, 0.800000f, 0.200000f),
        new Color(0.000000f, 0.800000f, 0.000000f),
        new Color(0.000000f, 0.600000f, 1.000000f),
        new Color(0.000000f, 0.600000f, 0.800000f),
        new Color(0.000000f, 0.600000f, 0.600000f),
        new Color(0.000000f, 0.600000f, 0.400000f),
        new Color(0.000000f, 0.600000f, 0.200000f),
        new Color(0.000000f, 0.600000f, 0.000000f),
        new Color(0.000000f, 0.400000f, 1.000000f),
        new Color(0.000000f, 0.400000f, 0.800000f),
        new Color(0.000000f, 0.400000f, 0.600000f),
        new Color(0.000000f, 0.400000f, 0.400000f),
        new Color(0.000000f, 0.400000f, 0.200000f),
        new Color(0.000000f, 0.400000f, 0.000000f),
        new Color(0.000000f, 0.200000f, 1.000000f),
        new Color(0.000000f, 0.200000f, 0.800000f),
        new Color(0.000000f, 0.200000f, 0.600000f),
        new Color(0.000000f, 0.200000f, 0.400000f),
        new Color(0.000000f, 0.200000f, 0.200000f),
        new Color(0.000000f, 0.200000f, 0.000000f),
        new Color(0.000000f, 0.000000f, 1.000000f),
        new Color(0.000000f, 0.000000f, 0.800000f),
        new Color(0.000000f, 0.000000f, 0.600000f),
        new Color(0.000000f, 0.000000f, 0.400000f),
        new Color(0.000000f, 0.000000f, 0.200000f),
        new Color(0.933333f, 0.000000f, 0.000000f),
        new Color(0.866667f, 0.000000f, 0.000000f),
        new Color(0.733333f, 0.000000f, 0.000000f),
        new Color(0.666667f, 0.000000f, 0.000000f),
        new Color(0.533333f, 0.000000f, 0.000000f),
        new Color(0.466667f, 0.000000f, 0.000000f),
        new Color(0.333333f, 0.000000f, 0.000000f),
        new Color(0.266667f, 0.000000f, 0.000000f),
        new Color(0.133333f, 0.000000f, 0.000000f),
        new Color(0.066667f, 0.000000f, 0.000000f),
        new Color(0.000000f, 0.933333f, 0.000000f),
        new Color(0.000000f, 0.866667f, 0.000000f),
        new Color(0.000000f, 0.733333f, 0.000000f),
        new Color(0.000000f, 0.666667f, 0.000000f),
        new Color(0.000000f, 0.533333f, 0.000000f),
        new Color(0.000000f, 0.466667f, 0.000000f),
        new Color(0.000000f, 0.333333f, 0.000000f),
        new Color(0.000000f, 0.266667f, 0.000000f),
        new Color(0.000000f, 0.133333f, 0.000000f),
        new Color(0.000000f, 0.066667f, 0.000000f),
        new Color(0.000000f, 0.000000f, 0.933333f),
        new Color(0.000000f, 0.000000f, 0.866667f),
        new Color(0.000000f, 0.000000f, 0.733333f),
        new Color(0.000000f, 0.000000f, 0.666667f),
        new Color(0.000000f, 0.000000f, 0.533333f),
        new Color(0.000000f, 0.000000f, 0.466667f),
        new Color(0.000000f, 0.000000f, 0.333333f),
        new Color(0.000000f, 0.000000f, 0.266667f),
        new Color(0.000000f, 0.000000f, 0.133333f),
        new Color(0.000000f, 0.000000f, 0.066667f),
        new Color(0.933333f, 0.933333f, 0.933333f),
        new Color(0.866667f, 0.866667f, 0.866667f),
        new Color(0.733333f, 0.733333f, 0.733333f),
        new Color(0.666667f, 0.666667f, 0.666667f),
        new Color(0.533333f, 0.533333f, 0.533333f),
        new Color(0.466667f, 0.466667f, 0.466667f),
        new Color(0.333333f, 0.333333f, 0.333333f),
        new Color(0.266667f, 0.266667f, 0.266667f),
        new Color(0.133333f, 0.133333f, 0.133333f),
        new Color(0.066667f, 0.066667f, 0.066667f),
        new Color(0.000000f, 0.000000f, 0.000000f)
    };
    public List<VoxelData> m_VoxelList = new List<VoxelData>();
    public int m_NumVoxels = 0;
    public VoxelData[] m_VoxelArray;
    public long m_OptimizationTime;

    Vector3 m_VoxelPosOffset;
    int[,,] m_VoxelArray3D;

    //----------------------------------------------------------------------------------------
    // 
    //----------------------------------------------------------------------------------------
    void AddVoxel(Vector3 pos, float unit, Color color)
    {
        VoxelData Data = new VoxelData();
        Data.Pos = pos;
        Data.Albedo = new Vector3(color.r, color.g, color.b);
        m_VoxelList.Add(Data);

        m_NumVoxels++;
    }
    //----------------------------------------------------------------------------------------
    // 
    //----------------------------------------------------------------------------------------
    bool FindVoxelAt(Vector3 p)
    {
        return m_VoxelList.Exists(x => x.Pos == p);
    }
    //----------------------------------------------------------------------------------------
    // 
    //----------------------------------------------------------------------------------------
    bool IsOccludedVoxel(int x, int y, int z)
    {
        if ((x + 1) >= m_VoxelArray3D.GetLength(0))
            return false;
        else if ((x - 1) < 0)
            return false;

        if ((y + 1) >= m_VoxelArray3D.GetLength(1))
            return false;
        else if ((y - 1) < 0)
            return false;

        if ((z + 1) >= m_VoxelArray3D.GetLength(2))
            return false;
        else if ((z - 1) < 0)
            return false;


        if (m_VoxelArray3D[x + 1, y, z] == -1)
            return false;

        if (m_VoxelArray3D[x - 1, y, z] == -1)
            return false;

        if (m_VoxelArray3D[x, y + 1, z] == -1)
            return false;

        if (m_VoxelArray3D[x, y - 1, z] == -1)
            return false;

        if (m_VoxelArray3D[x, y, z + 1] == -1)
            return false;

        if (m_VoxelArray3D[x, y, z - 1] == -1)
            return false;

        return true;
    }
    //----------------------------------------------------------------------------------------
    // 
    //----------------------------------------------------------------------------------------
    void Optimize()
    {
        var StopWatch = new Stopwatch();
        StopWatch.Start();

        List<int> RemoveList = new List<int>();

        for (var z = 0; z < m_VoxelArray3D.GetLength(2); z++)
        {
            for (var y = 0; y < m_VoxelArray3D.GetLength(1); y++)
            {
                for (var x = 0; x < m_VoxelArray3D.GetLength(0); x++)
                {
                    if (m_VoxelArray3D[x, y, z] != -1)
                    {
                        if (IsOccludedVoxel(x, y, z))
                        {
                            int Index = RemoveList.BinarySearch(m_VoxelArray3D[x, y, z]);
                            if (Index < 0)
                            {
                                RemoveList.Insert(~Index, m_VoxelArray3D[x, y, z]);
                            }
                        }
                    }
                }
            }
        }

        for (int i = RemoveList.Count - 1; i >=0; i--)
        {
            m_VoxelList.RemoveAt(RemoveList[i]);
        }
        m_NumVoxels = m_VoxelList.Count;

        RemoveList.Clear();

        StopWatch.Stop();
        m_OptimizationTime = StopWatch.ElapsedMilliseconds;
    }
    //-------------------------------------------------------------------------------------------------------
    //
    // 說明:  Load magic voxel file format .VOX.
    // https://github.com/ephtracy/voxel-model/blob/master/MagicaVoxel-file-format-vox.txt
    // https://www.giawa.com/magicavoxel-c-importer/
    //-------------------------------------------------------------------------------------------------------
    public bool Load(string filename, LoadOption option)
    {
        if (!File.Exists(filename))
            return false;

        Stream Stream = File.OpenRead(filename);

        BinaryReader Reader = new BinaryReader(Stream);
        string magic = new string(Reader.ReadChars(4));
        int version = Reader.ReadInt32();

        Vector3 CenterPos = Vector3.zero;
        Vector3 Pos = Vector3.zero;

        if (magic == "VOX ")
        {
            int sizex = 0, sizey = 0, sizez = 0;
            while (Reader.BaseStream.Position < Reader.BaseStream.Length)
            {
                char[] chunkId = Reader.ReadChars(4);
                int chunkSize = Reader.ReadInt32();
                int childChunks = Reader.ReadInt32();
                string chunkName = new string(chunkId);
                if (chunkName == "SIZE")
                {
                    sizex = Reader.ReadInt32();
                    sizez = Reader.ReadInt32();
                    sizey = Reader.ReadInt32();

                    m_VoxelPosOffset = new Vector3(-sizex * 0.5f, sizey * 0.5f, sizez * 0.5f);
                    m_VoxelArray3D = new int[sizex, sizey, sizez];
                    for (var z = 0; z < sizez; z++)
                    {
                        for (var y = 0; y < sizey; y++)
                        {
                            for (var x = 0; x < sizex; x++)
                            {
                                m_VoxelArray3D[x, y, z] = -1;
                            }
                        }
                    }
                    Reader.ReadBytes(chunkSize - 4 * 3);
                }
                else if (chunkName == "XYZI")
                {
                    int numVoxels = Reader.ReadInt32();
                    //m_VoxelArray = new VoxelData[numVoxels];

                    Color VoxColor = Color.white;

                    for (int i = 0; i < numVoxels; i++)
                    {
                        byte x = (byte)(Reader.ReadByte());
                        byte z = (byte)(Reader.ReadByte());
                        byte y = (byte)(Reader.ReadByte());
                        byte Palette = Reader.ReadByte();                        

                        m_VoxelArray3D[x, y, z] = i;
                        AddVoxel(new Vector3(-x, y, z) - m_VoxelPosOffset, 1.0f, new Color(Palette, 0, 0, 0));
                    }
                }
                else if (chunkName == "RGBA")
                {
                    for (int i = 0; i < 256; i++)
                    {
                        byte R = Reader.ReadByte();
                        byte G = Reader.ReadByte();
                        byte B = Reader.ReadByte();
                        byte A = Reader.ReadByte();

                        m_MagicVoxelPalatte[i].r = (float)R / 255.0f;
                        m_MagicVoxelPalatte[i].g = (float)G / 255.0f;
                        m_MagicVoxelPalatte[i].b = (float)B / 255.0f;
                        m_MagicVoxelPalatte[i].a = 1;
                    }

                    for (int i = 0; i < m_VoxelList.Count; i++)
                    {
                        VoxelData T = m_VoxelList[i];
                        int Palette = (int)T.Albedo.x - 1;
                        T.Albedo = new Vector3(m_MagicVoxelPalatte[Palette].r, m_MagicVoxelPalatte[Palette].g, m_MagicVoxelPalatte[Palette].b);
                        m_VoxelList[i] = T;
                    }
                }
                else if (chunkName == "MATT")
                {
                    /*
                    ------------------------------------------------------------------------------
                    # Bytes | Type  | Value
                    ------------------------------------------------------------------------------
                    4       | int   | id[1 - 255]
                    --------+-------+-------------------------------------------------------------
                    4       | int   | material type
                            |       | 0 : diffuse
                            |       | 1 : metal
                            |       | 2 : glass
                            |       | 3 : emissive
                    --------+-------+-------------------------------------------------------------
                    4       | float | material weight
                            |       | diffuse  : 1.0
                            |       | metal    : (0.0 - 1.0] : blend between metal and diffuse material
                            |       | glass    : (0.0 - 1.0] : blend between glass and diffuse material
                            |       | emissive : (0.0 - 1.0] : self - illuminated material
                    --------+-------+-------------------------------------------------------------
                    4       | int   | property bits: set if value is saved in next section
                            |       | bit(0) : Plastic
                            |       | bit(1) : Roughness
                            |       | bit(2) : Specular
                            |       | bit(3) : IOR
                            |       | bit(4) : Attenuation
                            |       | bit(5) : Power
                            |       | bit(6) : Glow
                            |       | bit(7) : isTotalPower(*no value)
                    --------+-------+-------------------------------------------------------------
                    4 * N   | float | normalized property value : (0.0 - 1.0]
                            |       | *need to map to real range
                            |       | *Plastic material only accepts { 0.0, 1.0} for this version
                    ------------------------------------------------------------------------------
                    */
                    int MatID = Reader.ReadInt32();
                    int MatType = Reader.ReadInt32();
                    float Value = Reader.ReadSingle();

                    int NumProps = 0;
                    int MatProp = Reader.ReadInt32();
                    if ((MatProp & 0x1) != 0)
                    {
                        NumProps++;
                    }
                    if ((MatProp & 0x2) != 0)
                    {
                        NumProps++;
                    }
                    if ((MatProp & 0x4) != 0)
                    {
                        NumProps++;
                    }
                    if ((MatProp & 0x8) != 0)
                    {
                        NumProps++;
                    }
                    if ((MatProp & 0x10) != 0)
                    {
                        NumProps++;
                    }
                    if ((MatProp & 0x20) != 0)
                    {
                        NumProps++;
                    }
                    if ((MatProp & 0x40) != 0)
                    {
                        NumProps++;
                    }
                    if ((MatProp & 0x80) != 0)
                    {
                        NumProps++;
                    }
                    Reader.ReadBytes(NumProps * sizeof(float));
                }
                else if (chunkName == "MATL")
                {
                    Dictionary<string, string> MatPropMap = new Dictionary<string, string>();
                    int MatID = Reader.ReadInt32();
                    int NumProps = Reader.ReadInt32();
                    for (int m = 0; m < NumProps; m++)
                    {
                        int StrSize = Reader.ReadInt32();
                        char[] KeyData = Reader.ReadChars(StrSize);
                        string Key = new string(KeyData);
                        StrSize = Reader.ReadInt32();
                        char[] ValueData = Reader.ReadChars(StrSize);
                        string Value = new string(ValueData);
                        MatPropMap[Key] = Value;
                    }
                }
                else
                {
                    Reader.ReadBytes(chunkSize);
                }
            }
        }
        Stream.Close();

        if (option.m_bOptimize)
            Optimize();

        m_VoxelArray = new VoxelData[m_VoxelList.Count];
        m_VoxelArray = m_VoxelList.ToArray();        

        m_VoxelList.Clear();

        return true;
    }
}
