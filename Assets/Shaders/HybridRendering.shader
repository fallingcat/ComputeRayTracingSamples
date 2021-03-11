Shader "Unlit/HybridRendering"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

			#define SHADOW_SAMPLES				8

            #include "UnityCG.cginc"
			#include "Types.cginc"
			#include "Utility.cginc"
			#include "Ray.cginc"
			#include "Material.cginc"
			#include "Sphere.cginc"
			#include "SimpleAS.cginc"
			#include "Camera.cginc"
			#include "RayQuery.cginc"


            struct appdata
            {
                float4 vertex : POSITION;
				float4 normal : NORMAL;                
				float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
				float3 normal : NORMAL;
				float3 pos : TEXCOORD0;
				float4 color : COLOR;
            };

			StructuredBuffer<SphereData> SimpleAccelerationStructureData;

			float4 TargetSize;
			float4 PointLightPos;
			float4 PointLightColor;

            v2f vert (appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
				o.normal = normalize(UnityObjectToWorldDir(v.normal));
				o.pos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.color = v.color;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				float3 Color = float3(0, 0, 0);				
				float3 N = normalize(i.normal);

				SimpleAccelerationStructure SAS = _SimpleAccelerationStructure(int(TargetSize.w), SimpleAccelerationStructureData);

				for (int s = 0; s < SHADOW_SAMPLES; s++)
				{					
					float3 Pos = i.pos + RandomDiskPoint(float3(i.pos.xy, (float)s), N) * 0.05f;
					float3 L = PointLightPos - Pos;
					float MaxT = length(L);
					L = normalize(L);
					RayQuery RQ = _RayQuery(SAS, _Ray(Pos, L), 0.15, MaxT);
					RayQueryProcess(RQ);
					if (RayQueryGetIntersectionType(RQ) == RQ_HIT_NONE)
					{						
						float Diffuse = max(dot(N, L), 0);
						Color += Diffuse * PointLightColor.rgb * PointLightColor.a * i.color;
					}
				}
				Color /= SHADOW_SAMPLES;
				Color += float3(0.1, 0.1, 0.1);
				Color = pow(Color, 1.0 / 1.8);
                fixed4 col = fixed4(Color, 1);
                
                return col;
            }
            ENDCG
        }
    }
}
