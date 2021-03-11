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