RayQuery _RayQuery(SimpleAccelerationStructure sas, Ray r, float min_t, float max_t)
{
	RayQuery RQ;

	RQ.R = r;
	RQ.AS = sas;
	RQ.MinT = min_t;
	RQ.MaxT = max_t;
	RQ.HitType = RQ_HIT_NONE;

	return RQ;
}

bool RayQueryProcess(inout RayQuery rq)
{
	if (SimpleAccelerationStructure_Hit(rq.AS, rq.R, rq.MinT, rq.MaxT, rq.HitRec))
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

int RayQueryGetIntersectionType(RayQuery rq)
{
	return rq.HitType;
}

float RayQueryGetIntersectionT(RayQuery rq)
{
	return rq.HitRec.t;
}