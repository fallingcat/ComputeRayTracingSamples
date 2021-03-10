Camera _Camera(float3 look_from, float3 look_at, float3 vup, 
	           float vfov, float aspect_ratio, 
			   float aperture, float focus_dist)
{
	Camera Cam;

	float Theta = vfov * PI / 180.0f;
	float h = tan(Theta / 2);
	float ViewportHeight = 2.0 * h;
	float ViewportWidth = aspect_ratio * ViewportHeight;

	Cam.w = normalize(look_from - look_at);
	Cam.u = normalize(cross(vup, Cam.w));
	Cam.v = cross(Cam.w, Cam.u);

	Cam.Origin = look_from;
	Cam.Horizontal = focus_dist * ViewportWidth * Cam.u;
	Cam.Vertical = focus_dist * ViewportHeight * Cam.v;
	Cam.LowerLeftCorner = Cam.Origin - Cam.Horizontal / 2 - Cam.Vertical / 2 - focus_dist * Cam.w;

	Cam.LensRadius = aperture / 2.0;

	return Cam;

}

Ray Camera_GetRay(Camera c, float2 uv)
{
	float3 rd = c.LensRadius * RandomDiskPoint(float3(uv, 0.4), c.w);
	float3 Offset = c.u * rd.x + c.v * rd.y;

	return _Ray(c.Origin + Offset, c.LowerLeftCorner + uv.x * c.Horizontal + uv.y * c.Vertical - c.Origin - Offset);	
}