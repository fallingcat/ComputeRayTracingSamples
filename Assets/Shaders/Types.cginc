#define MINT					0.001
#define INFINITY				9999999.99999
#define PI						3.14159265359
#define MAX_RAY_RECURSIVE_DEPTH 6
#define AA_SAMPLES				16
#define BKG_COLOR0				float3(1, 1, 1)
#define BKG_COLOR1				float3(0.5, 0.7, 1.0)
#define MAT_LAMBERTIAN			0
#define MAT_METAL				1
#define MAT_DIELECTRIC			2 

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

struct Sphere
{
	float3 Center;
	float Radius;
	Material Material;
};

struct World
{
	int NumObjects;
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
float3 Ray_Color(Ray r, World w);

Sphere _Sphere(float3 c, float r, Material m);
bool Sphere_Hit(Sphere s, Ray r, float t_min, float t_max, inout HitRecord rec);

Material _Material(int type, float3 albedo, float fuzz);
bool Material_Scatter(inout Material m, Ray r, HitRecord rec, inout float3 attenuation, inout Ray scattered, float3 seed);

World _World(int num);
bool World_Hit(World w, Ray r, float t_min, float t_max, inout HitRecord rec);

Camera _Camera(float3 look_from, float3 look_at, float3 vup, float3 vfov, float aspect_ratio, float aperture, float focus_dist);
Ray Camera_GetRay(Camera c, float2 uv);

