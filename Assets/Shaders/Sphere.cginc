Sphere _Sphere(float3 c, float r, Material m)
{
	Sphere S;

	S.Center = c;
	S.Radius = r;
	S.Material = m;

	return S;
}

bool Sphere_Hit(Sphere s, Ray r, float t_min, float t_max, inout HitRecord rec)
{
	float3 oc = r.Orig - s.Center;
	float a = dot(r.Dir, r.Dir);
	float half_b = dot(oc, r.Dir);
	float c = dot(oc, oc) - s.Radius * s.Radius;

	float Discriminant = half_b * half_b - a * c;
	if (Discriminant < 0)
	{
		return false;
	}

	float sqrtd = sqrt(Discriminant);

	// Find the nearest root that lies in the acceptable range.
	float root = (-half_b - sqrtd) / a;
	if (root < t_min || t_max < root)
	{
		root = (-half_b + sqrtd) / a;
		if (root < t_min || t_max < root)
		{
			return false;
		}
	}

	rec.t = root;
	rec.P = Ray_At(r, rec.t);
	float3 OutwardNormal = (rec.P - s.Center) / s.Radius;
	HitRecord_SetFaceNormal(rec, r, OutwardNormal);
	rec.Material = s.Material;

	return true;
}
