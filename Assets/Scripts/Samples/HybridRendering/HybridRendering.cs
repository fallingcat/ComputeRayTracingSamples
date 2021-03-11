using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HybridRendering : MonoBehaviour
{
    public enum MaterialType
    {
        MAT_LAMBERTIAN = 0,
        MAT_METAL = 1,
        MAT_DIELECTRIC = 2,
    }

    public struct SphereData
    {
        public Vector3 Center;
        public float Radius;
        public int MaterialType;
        public Vector3 MaterialAlbedo;
        public Vector4 MaterialData;
    }

    public Material m_Material;
    public Light m_PointLight;

    ComputeBuffer m_SimpleAccelerationStructureDataBuffer;
    int m_NumSpheres = 0;
    SphereData[] m_SphereArray = new SphereData[512];
    float[] m_SphereTimeOffset = new float[512];
    GameObject[] m_SphereGOArray = new GameObject[512];

    // Start is called before the first frame update
    void Start()
    {
        m_SimpleAccelerationStructureDataBuffer = new ComputeBuffer(512, System.Runtime.InteropServices.Marshal.SizeOf(typeof(SphereData)), ComputeBufferType.Default);

        SphereData Data = new SphereData();
        Color[] Colors;
        Mesh SphereMesh;

        Data.Center = new Vector3(0, 1.0f, 0.0f);
        Data.Radius = 1.0f;
        Data.MaterialType = (int)MaterialType.MAT_DIELECTRIC;
        Data.MaterialAlbedo = new Vector3(0.1f, 0.2f, 0.5f);
        Data.MaterialData = new Vector4(1.5f, 0.0f, 0.0f, 0.0f);
        m_SphereArray[m_NumSpheres] = Data;
        m_SphereTimeOffset[m_NumSpheres] = UnityEngine.Random.Range(0, 100.0f);
        m_SphereGOArray[m_NumSpheres] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        m_SphereGOArray[m_NumSpheres].transform.localPosition = Data.Center;
        m_SphereGOArray[m_NumSpheres].transform.localScale = new Vector3(Data.Radius * 2.0f, Data.Radius * 2.0f, Data.Radius * 2.0f);
        m_SphereGOArray[m_NumSpheres].GetComponent<Renderer>().material = m_Material;
        SphereMesh = m_SphereGOArray[m_NumSpheres].GetComponent<MeshFilter>().mesh;
        Colors = new Color[SphereMesh.vertices.Length];
        for (int i = 0; i < SphereMesh.vertices.Length; i++)
            Colors[i] = new Color(Data.MaterialAlbedo.x, Data.MaterialAlbedo.y, Data.MaterialAlbedo.z, 1.0f);
        SphereMesh.colors = Colors;        
        m_NumSpheres++;

        Data.Center = new Vector3(-4.0f, 1.0f, 0.0f);
        Data.Radius = 1.0f;
        Data.MaterialType = (int)MaterialType.MAT_LAMBERTIAN;
        Data.MaterialAlbedo = new Vector3(0.4f, 0.2f, 0.1f);
        Data.MaterialData = new Vector4(1.0f, 0.0f, 0.0f, 0.0f);
        m_SphereArray[m_NumSpheres] = Data;
        m_SphereTimeOffset[m_NumSpheres] = UnityEngine.Random.Range(0, 100.0f);
        m_SphereGOArray[m_NumSpheres] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        m_SphereGOArray[m_NumSpheres].transform.localPosition = Data.Center;
        m_SphereGOArray[m_NumSpheres].transform.localScale = new Vector3(Data.Radius * 2.0f, Data.Radius * 2.0f, Data.Radius * 2.0f);
        m_SphereGOArray[m_NumSpheres].GetComponent<Renderer>().material = m_Material;
        SphereMesh = m_SphereGOArray[m_NumSpheres].GetComponent<MeshFilter>().mesh;
        Colors = new Color[SphereMesh.vertices.Length];
        for (int i = 0; i < SphereMesh.vertices.Length; i++)
            Colors[i] = new Color(Data.MaterialAlbedo.x, Data.MaterialAlbedo.y, Data.MaterialAlbedo.z, 1.0f);
        SphereMesh.colors = Colors;
        m_NumSpheres++;

        Data.Center = new Vector3(4.0f, 1.0f, 0.0f);
        Data.Radius = 1.0f;
        Data.MaterialType = (int)MaterialType.MAT_METAL;
        Data.MaterialAlbedo = new Vector3(0.7f, 0.6f, 0.5f);
        Data.MaterialData = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
        m_SphereArray[m_NumSpheres] = Data;
        m_SphereTimeOffset[m_NumSpheres] = UnityEngine.Random.Range(0, 100.0f);
        m_SphereGOArray[m_NumSpheres] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        m_SphereGOArray[m_NumSpheres].transform.localPosition = Data.Center;
        m_SphereGOArray[m_NumSpheres].transform.localScale = new Vector3(Data.Radius * 2.0f, Data.Radius * 2.0f, Data.Radius * 2.0f);
        m_SphereGOArray[m_NumSpheres].GetComponent<Renderer>().material = m_Material;
        SphereMesh = m_SphereGOArray[m_NumSpheres].GetComponent<MeshFilter>().mesh;
        Colors = new Color[SphereMesh.vertices.Length];
        for (int i = 0; i < SphereMesh.vertices.Length; i++)
            Colors[i] = new Color(Data.MaterialAlbedo.x, Data.MaterialAlbedo.y, Data.MaterialAlbedo.z, 1.0f);
        SphereMesh.colors = Colors;
        m_NumSpheres++;

        for (int a = -4; a < 5; a++)
        {
            for (int b = -4; b < 4; b++)
            {
                float Choose_Mat = UnityEngine.Random.Range(0, 1.0f);
                Vector3 Center = new Vector3(a * 1.5f + 1.5f * UnityEngine.Random.Range(0, 1.0f), 0.2f, b * 1.0f + 1.0f * UnityEngine.Random.Range(0, 1.0f));
                Vector3 Dist = Center - new Vector3(4, 0.2f, 0);
                if (Dist.magnitude > 0.9f)
                {
                    if (Choose_Mat < 0.5f)
                    {
                        // diffuse
                        Vector3 Albedo = new Vector3(UnityEngine.Random.Range(0, 1.0f), UnityEngine.Random.Range(0, 1.0f), UnityEngine.Random.Range(0, 1.0f));
                        Data.Center = Center;
                        Data.Radius = 0.2f;
                        Data.MaterialType = (int)MaterialType.MAT_LAMBERTIAN;
                        Data.MaterialAlbedo = Albedo;
                        Data.MaterialData = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
                    }
                    else if (Choose_Mat < 0.8f)
                    {
                        // metal
                        Vector3 Albedo = new Vector3(UnityEngine.Random.Range(0, 1.0f), UnityEngine.Random.Range(0, 1.0f), UnityEngine.Random.Range(0, 1.0f));
                        float Fuzz = UnityEngine.Mathf.Min(UnityEngine.Random.Range(0, 1.0f), 0.5f);
                        Data.Center = Center;
                        Data.Radius = 0.2f;
                        Data.MaterialType = (int)MaterialType.MAT_METAL;
                        Data.MaterialAlbedo = Albedo;
                        Data.MaterialData = new Vector4(Fuzz, 0.0f, 0.0f, 0.0f);
                    }
                    else
                    {
                        Data.Center = Center;
                        Data.Radius = 0.2f;
                        Data.MaterialType = (int)MaterialType.MAT_DIELECTRIC;
                        Data.MaterialData = new Vector4(1.5f, 0.0f, 0.0f, 0.0f);
                    }
                    m_SphereArray[m_NumSpheres] = Data;
                    m_SphereTimeOffset[m_NumSpheres] = UnityEngine.Random.Range(0, 100.0f);
                    m_SphereGOArray[m_NumSpheres] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    m_SphereGOArray[m_NumSpheres].transform.localPosition = Data.Center;
                    m_SphereGOArray[m_NumSpheres].transform.localScale = new Vector3(Data.Radius * 2.0f, Data.Radius * 2.0f, Data.Radius * 2.0f);
                    m_SphereGOArray[m_NumSpheres].GetComponent<Renderer>().material = m_Material;
                    SphereMesh = m_SphereGOArray[m_NumSpheres].GetComponent<MeshFilter>().mesh;
                    Colors = new Color[SphereMesh.vertices.Length];
                    for (int i = 0; i < SphereMesh.vertices.Length; i++)
                        Colors[i] = new Color(Data.MaterialAlbedo.x, Data.MaterialAlbedo.y, Data.MaterialAlbedo.z, 1.0f);
                    SphereMesh.colors = Colors;
                    m_NumSpheres++;
                }
            }
        }
        m_SimpleAccelerationStructureDataBuffer.SetData(m_SphereArray);

        Camera.main.transform.localPosition = new Vector3(13, 2, -3);
        Camera.main.transform.LookAt(new Vector3(0, 0, 0));
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 4; i < m_NumSpheres; i++)
        {
            m_SphereArray[i].Center.y = 0.2f + (UnityEngine.Mathf.Sin(m_SphereTimeOffset[i] + (Time.time * 2.0f))) + 1.0f;
            m_SphereGOArray[i].transform.localPosition = m_SphereArray[i].Center;
        }        
        m_SimpleAccelerationStructureDataBuffer.SetData(m_SphereArray);

        m_Material.SetVector("TargetSize", new Vector4(0, 0, UnityEngine.Mathf.Sin(Time.time * 10.0f), m_NumSpheres));
        m_Material.SetVector("PointLightPos", new Vector4(m_PointLight.transform.position.x, m_PointLight.transform.position.y, m_PointLight.transform.position.z, 0.0f));
        m_Material.SetVector("PointLightColor", new Vector4(m_PointLight.color.r, m_PointLight.color.g, m_PointLight.color.b, m_PointLight.intensity));        
        m_Material.SetBuffer("SimpleAccelerationStructureData", m_SimpleAccelerationStructureDataBuffer);
    }
}
