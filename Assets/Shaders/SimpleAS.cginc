
SimpleAccelerationStructure _SimpleAccelerationStructure(int num)
{
	SimpleAccelerationStructure SAS;

	SAS.NumObjects = num;
	
	return SAS;
}

bool SimpleAccelerationStructure_Hit(SimpleAccelerationStructure sas, Ray r, float t_min, float t_max, inout HitRecord rec)
{
	HitRecord TempRec = _HitRecord();
	bool bHitAnything = false;
	float ClosestT = t_max;

	for (int i = 0; i < sas.NumObjects; i++)
	{
		Sphere S = _Sphere(SimpleAccelerationStructureData[i].Center, SimpleAccelerationStructureData[i].Radius, _Material(SimpleAccelerationStructureData[i].MaterialType, SimpleAccelerationStructureData[i].MaterialAlbedo, SimpleAccelerationStructureData[i].MaterialData.x));
		if (Sphere_Hit(S, r, t_min, ClosestT, TempRec))
		{
			bHitAnything = true;
			ClosestT = TempRec.t;
			rec = TempRec;
		}		
	}
	return bHitAnything;
}
