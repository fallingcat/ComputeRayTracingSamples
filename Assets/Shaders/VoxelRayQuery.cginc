//----------------------------------------------------------------------------------------
// 
//----------------------------------------------------------------------------------------
VoxelRayQuery _VoxelRayQuery(VoxelAccelerationStructure as, Ray r, float min_t, float max_t)
{
	VoxelRayQuery RQ;

	RQ.R = r;
	RQ.AS = as;
	RQ.MinT = min_t;
	RQ.MaxT = max_t;
	RQ.HitType = RQ_HIT_NONE;
	RQ.HitRec = _HitRecord();

	return RQ;
}
//----------------------------------------------------------------------------------------
// 
//----------------------------------------------------------------------------------------
bool VoxelRayQueryProcess(inout VoxelRayQuery rq)
{
	if (VoxelAccelerationStructure_Hit(rq.AS, rq.R, rq.MinT, rq.MaxT, rq.HitRec))
	{
		rq.HitType = RQ_HIT_OBJECT;
		return true;
	}
	else
	{
		rq.HitType = RQ_HIT_NONE;
		return true;
	}
}
//----------------------------------------------------------------------------------------
// Brute-force ray query
//----------------------------------------------------------------------------------------
bool VoxelRayQueryProcessNoAS(inout VoxelRayQuery rq)
{
	if (FastPrmitiveList_Hit(rq.AS, rq.R, rq.MinT, rq.MaxT, rq.HitRec))
	{
		rq.HitType = RQ_HIT_OBJECT;
		return true;
	}
	else
	{
		rq.HitType = RQ_HIT_NONE;
		return true;
	}
}
//----------------------------------------------------------------------------------------
// 
//----------------------------------------------------------------------------------------
int VoxelRayQueryGetIntersectionType(VoxelRayQuery rq)
{
	return rq.HitType;
}
//----------------------------------------------------------------------------------------
// 
//----------------------------------------------------------------------------------------
float VoxelRayQueryGetIntersectionT(VoxelRayQuery rq)
{
	return rq.HitRec.t;
}