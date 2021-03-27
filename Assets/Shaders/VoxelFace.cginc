//----------------------------------------------------------------------------------------
// 
//----------------------------------------------------------------------------------------
void _VoxelFace(inout VoxelFace vf, float2 p0, float2 p1, float d, float f, Material m)
{
	vf.P0 = p0;
	vf.P1 = p1;
	vf.D = d;
	vf.F = f;
	vf.Material = m;
}
//----------------------------------------------------------------------------------------
// 
//----------------------------------------------------------------------------------------
bool VoxelFace_Hit_XZ(VoxelFace vf, Ray r, float t_min, inout float t_max, inout HitRecord rec)
{
	float3 OutwardNormal = float3(0, 1, 0) * vf.F;

	if (dot(r.Dir, OutwardNormal) > 0)
		return false;

	float t = (vf.D - r.Orig.y) * r.InvDir.y;
	if (t < t_min || t > t_max)
		return false;

	float2 P = r.Orig.xz + t * r.Dir.xz;	
	if (P.x < vf.P0.x || P.x > vf.P0.y || P.y < vf.P1.x || P.y > vf.P1.y)
		return false;

	t_max = t;
	rec.t = t;
	rec.P = Ray_At(r, rec.t);
	rec.Normal = OutwardNormal;
	//HitRecord_SetFaceNormal(rec, r, OutwardNormal);
	rec.Material = vf.Material;	

	return true;
}
//----------------------------------------------------------------------------------------
// 
//----------------------------------------------------------------------------------------
bool VoxelFace_Hit_YZ(VoxelFace vf, Ray r, float t_min, inout float t_max, inout HitRecord rec)
{
	float3 OutwardNormal = float3(1, 0, 0) * vf.F;

	if (dot(r.Dir, OutwardNormal) > 0)
		return false;

	float t = (vf.D - r.Orig.x) * r.InvDir.x;
	if (t < t_min || t > t_max)
		return false;

	float2 P = r.Orig.yz + t * r.Dir.yz;
	if (P.x < vf.P0.x || P.x > vf.P0.y || P.y < vf.P1.x || P.y > vf.P1.y)
		return false;

	t_max = t;
	rec.t = t;
	rec.P = Ray_At(r, rec.t);
	rec.Normal = OutwardNormal;
	//HitRecord_SetFaceNormal(rec, r, OutwardNormal);
	rec.Material = vf.Material;	

	return true;
}
//----------------------------------------------------------------------------------------
// 
//----------------------------------------------------------------------------------------
bool VoxelFace_Hit_XY(VoxelFace vf, Ray r, float t_min, inout float t_max, inout HitRecord rec)
{	
	float3 OutwardNormal = float3(0, 0, 1) * vf.F;

	if (dot(r.Dir, OutwardNormal) > 0)
		return false;

	float t = (vf.D - r.Orig.z) * r.InvDir.z;
	if (t < t_min || t > t_max)
		return false;

	float2 P = r.Orig.xy + t * r.Dir.xy;
	if (P.x < vf.P0.x || P.x > vf.P0.y || P.y < vf.P1.x || P.y > vf.P1.y)
		return false;

	t_max = t;
	rec.t = t;
	rec.P = Ray_At(r, rec.t);
	rec.Normal = OutwardNormal;
	//HitRecord_SetFaceNormal(rec, r, OutwardNormal);
	rec.Material = vf.Material;	

	return true;
}
