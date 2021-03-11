#define MINT					0.001
#define INFINITY				9999999.99999
#define PI						3.14159265359

#define BKG_COLOR0				float3(1, 1, 1)
#define BKG_COLOR1				float3(0.5, 0.7, 1.0)

#define MAT_LAMBERTIAN			0
#define MAT_METAL				1
#define MAT_DIELECTRIC			2 

#define RQ_HIT_NONE				0
#define RQ_HIT_OBJECT			1

struct SphereData
{
	float3 Center;
	float Radius;
	int MaterialType;
	float3 MaterialAlbedo;
	float4 MaterialData;
};

struct SimpleAccelerationStructure
{
	int NumObjects;
	StructuredBuffer<SphereData> ASData;
};

struct Material
{
	int Type;
	float3 Albedo;
	float Fuzz;
	float IR;
};

struct HitRecord
{	
	float3 P;
	float3 Normal;
	float t;
	Material Material;
	bool bFrontFace;
};

struct Ray
{
	float3 Orig;
	float3 Dir;
};

struct RayQuery
{
	SimpleAccelerationStructure AS;
	Ray R;	
	float MinT;
	float MaxT;
	int HitType;
	HitRecord HitRec;
};

struct Sphere
{
	float3 Center;
	float Radius;
	Material Material;
};

struct Camera
{
	float3 Origin;
	float3 LowerLeftCorner;
	float3 Horizontal;
	float3 Vertical;
	float3 u, v, w;
	float LensRadius;
};

HitRecord _HitRecord();

Ray _Ray(float3 o, float3 d);
float3 Ray_At(Ray r, float t);
float3 Ray_Color(Ray r, SimpleAccelerationStructure sas);

Sphere _Sphere(float3 c, float r, Material m);
bool Sphere_Hit(Sphere s, Ray r, float t_min, float t_max, inout HitRecord rec);

Material _Material(int type, float3 albedo, float fuzz);
bool Material_Scatter(inout Material m, Ray r, HitRecord rec, inout float3 attenuation, inout Ray scattered, float3 seed);

SimpleAccelerationStructure _SimpleAccelerationStructure(int num, StructuredBuffer<SphereData> data);
bool SimpleAccelerationStructure_Hit(SimpleAccelerationStructure sas, Ray r, float t_min, float t_max, inout HitRecord rec);

Camera _Camera(float3 look_from, float3 look_at, float3 vup, float3 vfov, float aspect_ratio, float aperture, float focus_dist);
Ray Camera_GetRay(Camera c, float2 uv);

RayQuery _RayQuery(SimpleAccelerationStructure sas, Ray r, float min_t, float max_t);
bool RayQueryProcess(inout RayQuery rq);
int RayQueryGetIntersectionType(RayQuery rq);
float RayQueryGetIntersectionT(RayQuery rq);