HitRecord _HitRecord()
{
	HitRecord Hit;

	Hit.P = float3(0, 0, 0);
	Hit.Normal = float3(0, 0, 0);
	Hit.t = 0;
	Hit.Material = _Material(0, float3(0, 0, 0), 0);

	return Hit;
}

void HitRecord_SetFaceNormal(inout HitRecord rec, Ray r, float3 outward_normal)
{
	rec.bFrontFace = (dot(r.Dir, outward_normal) < 0);
	rec.Normal = rec.bFrontFace ? outward_normal : -outward_normal;
}


Ray _Ray(float3 o, float3 d)
{
	Ray R;

	R.Orig = o;
	R.Dir = d;

	return R;
}

float3 Ray_At(Ray r, float t)
{
	return (r.Orig + r.Dir * t);
}

float3 xRay_Color(Ray r, World w)
{
	Sphere Sph;
	HitRecord Rec = _HitRecord();

	if (World_Hit(w, r, MINT, INFINITY, Rec))
	{
		return 0.5 * (Rec.Normal + float3(1, 1, 1));
	}

	float3 UnitDir = normalize(r.Dir);
	float t = 0.5 * (UnitDir.y + 1.0f);
	return (1.0 - t) * float3(1.0, 1.0, 1.0) + t * float3(0.5, 0.7, 1.0);
}

float3 Ray_Color(Ray r, World w, float2 uv)
{
	Sphere Sph;
	HitRecord Rec = _HitRecord();
	Ray R = r;
	float3 Factor = float3(1, 1, 1);
	Ray Scattered = _Ray(float3(0, 0, 0), float3(0, 0, 0));
	float3 Attenuation = float3(0, 0, 0);

	for (int i = 0; i < MAX_RAY_RECURSIVE_DEPTH; i++)
	{
		if (World_Hit(w, R, MINT, INFINITY, Rec))
		{			
			float Offset = ((float)i) / (float)(MAX_RAY_RECURSIVE_DEPTH * 2);

			if (Material_Scatter(Rec.Material, R, Rec, Attenuation, Scattered, float3(uv, Offset)))
			{
				R = Scattered;
				Factor *= Attenuation;
			}				
			else
			{
				return float3(0, 0, 0);
			}
		}
		else
		{
			float3 UnitDir = normalize(R.Dir);
			float t = 0.5 * (UnitDir.y + 1.0f);
			return lerp(BKG_COLOR0, BKG_COLOR1, t) * Factor;
		}
	}
	return float3(0, 0, 0);	
}
