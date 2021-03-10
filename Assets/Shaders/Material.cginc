Material _Material(int type, float3 albedo, float fuzz)
{
	Material M;

	M.Type = type;
	M.Albedo = albedo;
	M.Fuzz = (fuzz < 1) ? fuzz : 1.0;
	M.IR = fuzz;

	return M;
}

bool Material_Scatter(inout Material m, Ray r, HitRecord rec, inout float3 attenuation, inout Ray scattered, float3 seed)
{
	switch (m.Type)
	{
		case MAT_LAMBERTIAN:
			float3 ScatterDirection = rec.Normal + RandomInUnitSphere(seed);
			if (NearZero(ScatterDirection))
				ScatterDirection = rec.Normal;
			scattered = _Ray(rec.P, ScatterDirection);
			attenuation = m.Albedo;
			return true;
			break;

		case MAT_METAL:
			float3 Reflected = Reflect(normalize(r.Dir), rec.Normal);
			scattered = _Ray(rec.P, Reflected + (m.Fuzz * RandomInUnitSphere(seed)));
			attenuation = m.Albedo;
			return (dot(scattered.Dir, rec.Normal) > 0);
			break;

		case MAT_DIELECTRIC:
			attenuation = float3(1.0, 1.0, 1.0);
			float RefractionRatio = rec.bFrontFace ? (1.0 / m.IR) : m.IR;

			float3 UnitDir = normalize(r.Dir);
			float CosTheta = min(dot(-UnitDir, rec.Normal), 1.0);
			float SinTheta = sqrt(1.0 - CosTheta * CosTheta);

			bool bCannotRefract = RefractionRatio * SinTheta > 1.0;
			float3 Direction;

			if (bCannotRefract || (Reflectance(CosTheta, RefractionRatio) > Random(seed.xy)))
				Direction = Reflect(UnitDir, rec.Normal);
			else
				Direction = Refract(UnitDir, rec.Normal, RefractionRatio);

			scattered = _Ray(rec.P, Direction);
			return true;
			break;

		default:
			return true;
			break;
	}	
}