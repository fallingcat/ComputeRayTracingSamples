using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayQuery : MonoBehaviour
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

    public Material m_QuadMaterial;
    public ComputeShader m_ComputeShader;
    public Vector2Int m_RTSize;
    public Light m_PointLight;

    RenderTexture m_RTTarget;
    ComputeBuffer m_SimpleAccelerationStructureDataBuffer;
    int m_NumSpheres = 0;
    SphereData[] m_SphereArray = new SphereData[512];
    float[] m_SphereTimeOffset = new float[512];

    // Start is called before the first frame update
    void Start()
    {
        //m_RTTarget = new RenderTexture(m_RTSize.x, m_RTSize.y, 0, UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_SRGB);
        m_RTTarget = new RenderTexture(m_RTSize.x, m_RTSize.y, 0, UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_UNorm);
        m_RTTarget.enableRandomWrite = true;
        m_RTTarget.Create();
        m_QuadMaterial.SetTexture("_MainTex", m_RTTarget);

        m_SimpleAccelerationStructureDataBuffer = new ComputeBuffer(512, System.Runtime.InteropServices.Marshal.SizeOf(typeof(SphereData)));

        SphereData Data = new SphereData();

        Data.Center = new Vector3(0, -1000.0f, 0.0f);
        Data.Radius = 1000.0f;
        Data.MaterialType = (int)MaterialType.MAT_LAMBERTIAN;
        Data.MaterialAlbedo = new Vector3(0.5f, 0.5f, 0.5f);
        m_SphereArray[m_NumSpheres] = Data;
        m_SphereTimeOffset[m_NumSpheres] = UnityEngine.Random.Range(0, 100.0f);
        m_NumSpheres++;

        Data.Center = new Vector3(0, 1.0f, 0.0f);
        Data.Radius = 1.0f;
        Data.MaterialType = (int)MaterialType.MAT_DIELECTRIC;
        Data.MaterialAlbedo = new Vector3(0.1f, 0.2f, 0.5f);
        Data.MaterialData = new Vector4(1.5f, 0.0f, 0.0f, 0.0f);
        m_SphereArray[m_NumSpheres] = Data;
        m_SphereTimeOffset[m_NumSpheres] = UnityEngine.Random.Range(0, 100.0f);
        m_NumSpheres++;

        Data.Center = new Vector3(-4.0f, 1.0f, 0.0f);
        Data.Radius = 1.0f;
        Data.MaterialType = (int)MaterialType.MAT_LAMBERTIAN;
        Data.MaterialAlbedo = new Vector3(0.4f, 0.2f, 0.1f);
        Data.MaterialData = new Vector4(1.0f, 0.0f, 0.0f, 0.0f);
        m_SphereArray[m_NumSpheres] = Data;
        m_SphereTimeOffset[m_NumSpheres] = UnityEngine.Random.Range(0, 100.0f);
        m_NumSpheres++;

        Data.Center = new Vector3(4.0f, 1.0f, 0.0f);
        Data.Radius = 1.0f;
        Data.MaterialType = (int)MaterialType.MAT_METAL;
        Data.MaterialAlbedo = new Vector3(0.7f, 0.6f, 0.5f);
        Data.MaterialData = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
        m_SphereArray[m_NumSpheres] = Data;
        m_SphereTimeOffset[m_NumSpheres] = UnityEngine.Random.Range(0, 100.0f);
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
                    m_NumSpheres++;
                }
            }
        }
        m_SimpleAccelerationStructureDataBuffer.SetData(m_SphereArray);
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 4; i < m_NumSpheres; i++)
            m_SphereArray[i].Center.y = 0.2f + (UnityEngine.Mathf.Sin(m_SphereTimeOffset[i] + (Time.time * 2.0f))) + 1.0f;

        int KernelHandle = m_ComputeShader.FindKernel("CSMain");
        m_ComputeShader.SetVector("TargetSize", new Vector4(m_RTSize.x, m_RTSize.y, UnityEngine.Mathf.Sin(Time.time * 10.0f), m_NumSpheres));
        m_ComputeShader.SetVector("PointLightPos", new Vector4(m_PointLight.transform.position.x, m_PointLight.transform.position.y, m_PointLight.transform.position.z, 0.0f));
        m_ComputeShader.SetVector("PointLightColor", new Vector4(m_PointLight.color.r, m_PointLight.color.g, m_PointLight.color.b, m_PointLight.intensity));
        m_ComputeShader.SetTexture(KernelHandle, "Result", m_RTTarget);
        m_SimpleAccelerationStructureDataBuffer.SetData(m_SphereArray);
        m_ComputeShader.SetBuffer(KernelHandle, "SimpleAccelerationStructureData", m_SimpleAccelerationStructureDataBuffer);
        m_ComputeShader.Dispatch(KernelHandle, m_RTSize.x / 8, m_RTSize.y / 8, 1);
    }
}
