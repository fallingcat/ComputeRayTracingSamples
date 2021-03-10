
World _World(int num)
{
	World W;

	W.NumObjects = num;
	
	return W;
}

bool World_Hit(World w, Ray r, float t_min, float t_max, inout HitRecord rec)
{
	HitRecord TempRec = _HitRecord();
	bool bHitAnything = false;
	float ClosestT = t_max;

	for (int i = 0; i < w.NumObjects; i++)
	{
		Sphere S = _Sphere(WorldData[i].Center, WorldData[i].Radius, _Material(WorldData[i].MaterialType, WorldData[i].MaterialAlbedo, WorldData[i].MaterialData.x));
		if (Sphere_Hit(S, r, t_min, ClosestT, TempRec))
		{
			bHitAnything = true;
			ClosestT = TempRec.t;
			rec = TempRec;
		}		
	}
	return bHitAnything;
}
