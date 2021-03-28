//----------------------------------------------------------------------------------------
// 
//----------------------------------------------------------------------------------------
void _AABB(inout AABB aabb, float3 min, float3 max)
{
	aabb.Min = min;
	aabb.Max = max;
}
//----------------------------------------------------------------------------------------
// Reference : 
// https://gamedev.stackexchange.com/questions/18436/most-efficient-aabb-vs-ray-collision-algorithms
//----------------------------------------------------------------------------------------
bool Fast_AABB_Overlap_FullRange(Ray r, float3 aabb_min, float3 aabb_max, float t_min, float t_max)
{
	float MinT;
	float MaxT;

	float3 t0 = (aabb_min - r.Orig) * r.InvDir;
	float3 t1 = (aabb_max - r.Orig) * r.InvDir;

	MinT = max(max(min(t0.x, t1.x), min(t0.y, t1.y)), min(t0.z, t1.z));
	MaxT = min(min(max(t0.x, t1.x), max(t0.y, t1.y)), max(t0.z, t1.z));

	// ray (line) overlaps AABB, but the whole AABB is behind us
	/*if (MaxT < 0)
	{
		return false;
	}
	*/

	// ray doesn't overlap AABB
	if (MinT > MaxT)
	{
		return false;
	}

	if (MinT < t_min || MinT > t_max)
		return false;

	return true;
}
//----------------------------------------------------------------------------------------
// Reference : 
// https://gamedev.stackexchange.com/questions/18436/most-efficient-aabb-vs-ray-collision-algorithms
//----------------------------------------------------------------------------------------
bool Fast_AABB_Overlap(Ray r, float3 aabb_min, float3 aabb_max, float t_max)
{
	float MinT;
	float MaxT;
	
	float3 t0 = (aabb_min - r.Orig) * r.InvDir;
	float3 t1 = (aabb_max - r.Orig) * r.InvDir;

	MinT = max(max(min(t0.x, t1.x), min(t0.y, t1.y)), min(t0.z, t1.z));
	MaxT = min(min(max(t0.x, t1.x), max(t0.y, t1.y)), max(t0.z, t1.z));
	
	// ray doesn't overlap AABB
	if (MinT > MaxT)
	{
		return false;
	}

	// ray doesn't overlap AABB in range
	if (MinT > t_max)	
		return false;

	return true;
}
//----------------------------------------------------------------------------------------
// Slower version ray-AABB overlap detection
//----------------------------------------------------------------------------------------
bool AABB_Overlap(Ray r, float3 aabb_min, float3 aabb_max, float t_min, float t_max)
{
	float Temp;
	float MinT;
	float MaxT;
	float _t0;
	float _t1;

	_t0 = (aabb_min.x - r.Orig.x) * r.InvDir.x;
	_t1 = (aabb_max.x - r.Orig.x) * r.InvDir.x;
	if (_t0 > _t1)
	{
		Temp = _t0;
		_t0 = _t1;
		_t1 = Temp;
	}
	MinT = max(_t0, t_min);
	MaxT = min(_t1, t_max);
	if (MaxT <= MinT)
		return false;

	_t0 = (aabb_min.y - r.Orig.y) * r.InvDir.y;
	_t1 = (aabb_max.y - r.Orig.y) * r.InvDir.y;
	if (_t0 > _t1)
	{
		Temp = _t0;
		_t0 = _t1;
		_t1 = Temp;
	}
	MinT = max(_t0, t_min);
	MaxT = min(_t1, t_max);
	if (MaxT <= MinT)
		return false;

	_t0 = (aabb_min.z - r.Orig.z) * r.InvDir.z;
	_t1 = (aabb_max.z - r.Orig.z) * r.InvDir.z;
	if (_t0 > _t1)
	{
		Temp = _t0;
		_t0 = _t1;
		_t1 = Temp;
	}
	MinT = max(_t0, t_min);
	MaxT = min(_t1, t_max);
	if (MaxT <= MinT)
		return false;

	return true;
}
//----------------------------------------------------------------------------------------
// Slower version ray-rectangle overlap detection
//----------------------------------------------------------------------------------------
bool AABB_Hit_XZ(Ray r, AABB aabb, Material m, float t_min, inout float t_max, inout HitRecord rec)
{
	bool bHit = false;
	float3 OutwardNormal = float3(0, 1, 0);
	float2 P;
	float t;

	if (dot(r.Dir, OutwardNormal) <= 0)
	{
		t = (aabb.Max.y - r.Orig.y) * r.InvDir.y;
		if (t >= t_min && t <= t_max)
		{
			P = r.Orig.xz + t * r.Dir.xz;
			if (P.x >= aabb.Min.x && P.x <= aabb.Max.x && P.y >= aabb.Min.z && P.y <= aabb.Max.z)
			{
				bHit = true;

				t_max = t;
				rec.t = t;
				rec.P = Ray_At(r, rec.t);
				rec.Normal = OutwardNormal;
				//HitRecord_SetFaceNormal(rec, r, OutwardNormal);
				rec.Material = m;
			}
		}
	}

	OutwardNormal = float3(0, -1, 0);
	if (dot(r.Dir, OutwardNormal) <= 0)
	{
		t = (aabb.Min.y - r.Orig.y) * r.InvDir.y;
		if (t >= t_min && t <= t_max)
		{
			P = r.Orig.xz + t * r.Dir.xz;
			if (P.x >= aabb.Min.x && P.x <= aabb.Max.x && P.y >= aabb.Min.z && P.y <= aabb.Max.z)
			{
				bHit = true;

				t_max = t;
				rec.t = t;
				rec.P = Ray_At(r, rec.t);
				rec.Normal = OutwardNormal;
				//HitRecord_SetFaceNormal(rec, r, OutwardNormal);
				rec.Material = m;
			}
		}
	}	

	return bHit;
}
//----------------------------------------------------------------------------------------
// Slower version ray-rectangle overlap detection
//----------------------------------------------------------------------------------------
bool AABB_Hit_YZ(Ray r, AABB aabb, Material m, float t_min, inout float t_max, inout HitRecord rec)
{
	bool bHit = false;
	float3 OutwardNormal = float3(1, 0, 0);
	float2 P;
	float t;

	if (dot(r.Dir, OutwardNormal) <= 0)
	{
		t = (aabb.Max.x - r.Orig.x) * r.InvDir.x;
		if (t >= t_min && t <= t_max)
		{
			P = r.Orig.yz + t * r.Dir.yz;
			if (P.x >= aabb.Min.y && P.x <= aabb.Max.y && P.y >= aabb.Min.z && P.y <= aabb.Max.z)
			{
				bHit = true;

				t_max = t;
				rec.t = t;
				rec.P = Ray_At(r, rec.t);
				rec.Normal = OutwardNormal;
				//HitRecord_SetFaceNormal(rec, r, OutwardNormal);
				rec.Material = m;
			}
		}
	}

	OutwardNormal = float3(-1, 0, 0);
	if (dot(r.Dir, OutwardNormal) <= 0)
	{
		t = (aabb.Min.x - r.Orig.x) * r.InvDir.x;
		if (t >= t_min && t <= t_max)
		{
			P = r.Orig.yz + t * r.Dir.yz;
			if (P.x >= aabb.Min.y && P.x <= aabb.Max.y && P.y >= aabb.Min.z && P.y <= aabb.Max.z)
			{
				bHit = true;

				t_max = t;
				rec.t = t;
				rec.P = Ray_At(r, rec.t);
				rec.Normal = OutwardNormal;
				//HitRecord_SetFaceNormal(rec, r, OutwardNormal);
				rec.Material = m;
			}
		}
	}

	return bHit;
}
//----------------------------------------------------------------------------------------
// Slower version ray-rectangle overlap detection
//----------------------------------------------------------------------------------------
bool AABB_Hit_XY(Ray r, AABB aabb, Material m, float t_min, inout float t_max, inout HitRecord rec)
{
	bool bHit = false;
	float3 OutwardNormal = float3(0, 0, 1);
	float2 P;
	float t;

	if (dot(r.Dir, OutwardNormal) <= 0)
	{
		t = (aabb.Max.z - r.Orig.z) * r.InvDir.z;
		if (t >= t_min && t <= t_max)
		{
			P = r.Orig.xy + t * r.Dir.xy;
			if (P.x >= aabb.Min.x && P.x <= aabb.Max.x && P.y >= aabb.Min.y && P.y <= aabb.Max.y)
			{
				bHit = true;

				t_max = t;
				rec.t = t;
				rec.P = Ray_At(r, rec.t);
				rec.Normal = OutwardNormal;
				//HitRecord_SetFaceNormal(rec, r, OutwardNormal);
				rec.Material = m;
			}
		}
	}

	OutwardNormal = float3(0, 0, -1);
	if (dot(r.Dir, OutwardNormal) <= 0)
	{
		t = (aabb.Min.z - r.Orig.z) * r.InvDir.z;
		if (t >= t_min && t <= t_max)
		{
			P = r.Orig.xy + t * r.Dir.xy;
			if (P.x >= aabb.Min.x && P.x <= aabb.Max.x && P.y >= aabb.Min.y && P.y <= aabb.Max.y)
			{
				bHit = true;
				t_max = t;
				rec.t = t;
				rec.P = Ray_At(r, rec.t);				
				rec.Normal = OutwardNormal;
				//HitRecord_SetFaceNormal(rec, r, OutwardNormal);
				rec.Material = m;
			}
		}
	}

	return bHit;
}
//----------------------------------------------------------------------------------------
// Fast version ray-AABB hit detection
//----------------------------------------------------------------------------------------
bool Fast_AABB_Hit(Ray r, AABB aabb, Material m, float t_min, inout float t_max, inout HitRecord rec)
{
	float MinT;
	float MaxT;
	float T;

	float3 t0 = (aabb.Min - r.Orig) * r.InvDir;
	float3 t1 = (aabb.Max - r.Orig) * r.InvDir;

	MinT = max(max(min(t0.x, t1.x), min(t0.y, t1.y)), min(t0.z, t1.z));
	MaxT = min(min(max(t0.x, t1.x), max(t0.y, t1.y)), max(t0.z, t1.z));

	if (MinT > MaxT || MaxT < 0)
	{
		return false;
	}

	if (MinT < 0) // ray origin is inside AABB so hit position at MaxT
		T = MaxT;
	else
		T = MinT;

	if (T < t_min || T > t_max)
		return false;

	t_max = T;
	rec.t = T;
	rec.P = Ray_At(r, T);
	
	if (T == t0.z && dot(r.Dir, float3(0, 0, 1)) > 0)
		rec.Normal = float3(0, 0, -1);
	else if (T == t1.z && dot(r.Dir, float3(0, 0, -1)) > 0)
		rec.Normal = float3(0, 0, 1);
	else if (T == t0.y && dot(r.Dir, float3(0, 1, 0)) > 0)
		rec.Normal = float3(0, -1, 0);
	else if (T == t1.y && dot(r.Dir, float3(0, -1, 0)) > 0)
		rec.Normal = float3(0, 1, 0);
	else if (T == t0.x && dot(r.Dir, float3(1, 0, 0)) > 0)
		rec.Normal = float3(-1, 0, 0);
	else if (T == t1.x && dot(r.Dir, float3(-1, 0, 0)) > 0)
		rec.Normal = float3(1, 0, 0);

	rec.Material = m;	

	return true;
}
//----------------------------------------------------------------------------------------
// Slower version ray-AABB hit detection
//----------------------------------------------------------------------------------------
bool AABB_Hit(Ray r, AABB aabb, Material m, float t_min, inout float t_max, inout HitRecord rec)
{	
	bool bHitAnything = false;

	if (AABB_Hit_XZ(r, aabb, m, t_min, t_max, rec))
	{
		bHitAnything = true;
	}
	if (AABB_Hit_YZ(r, aabb, m, t_min, t_max, rec))
	{
		bHitAnything = true;
	}
	if (AABB_Hit_XY(r, aabb, m, t_min, t_max, rec))
	{
		bHitAnything = true;
	}
	return bHitAnything;
}
