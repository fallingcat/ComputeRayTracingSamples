float Random(float2 uv)
{
	return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453123);
}


// Source
// http://www.gamedev.net/topic/592001-random-number-generation-based-on-time-in-hlsl/
// Supposebly from the NVidia Direct3D10 SDK
// Slightly modified for my purposes
#define RANDOM_IA 16807
#define RANDOM_IM 2147483647
#define RANDOM_AM (1.0f/float(RANDOM_IM))
#define RANDOM_IQ 127773u
#define RANDOM_IR 2836
#define RANDOM_MASK 123459876

struct NumberGenerator {
	int seed; // Used to generate values.

	// Returns the current random float.
	float GetCurrentFloat() {
		Cycle();
		return RANDOM_AM * seed;
	}

	// Returns the current random int.
	int GetCurrentInt() {
		Cycle();
		return seed;
	}

	// Generates the next number in the sequence.
	void Cycle() {
		seed ^= RANDOM_MASK;
		int k = seed / RANDOM_IQ;
		seed = RANDOM_IA * (seed - k * RANDOM_IQ) - RANDOM_IR * k;

		if (seed < 0)
			seed += RANDOM_IM;

		seed ^= RANDOM_MASK;
	}

	// Cycles the generator based on the input count. Useful for generating a thread unique seed.
	// PERFORMANCE - O(N)
	void Cycle(const uint _count) {
		for (uint i = 0; i < _count; ++i)
			Cycle();
	}

	// Returns a random float within the input range.
	float GetRandomFloat(const float low, const float high) {
		float v = GetCurrentFloat();
		return low * (1.0f - v) + high * v;
	}

	// Sets the seed
	void SetSeed(const uint value) {
		seed = int(value);
		Cycle();
	}
};

float3 RandomInUnitSphere(float3 seed)
{
	float2 Rand = float2(Random(seed.xy), Random(seed.xy + seed.z));	
	Rand -= 0.5f;
	Rand *= 2.0f;
	float ang1 = (Rand.x + 1.0) * PI; // [-1..1) -> [0..2*PI)
	float u = Rand.y; // [-1..1), cos and acos(2v-1) cancel each other out, so we arrive at [-1..1)
	float u2 = u * u;
	float sqrt1MinusU2 = sqrt(1.0 - u2);
	float x = sqrt1MinusU2 * cos(ang1);
	float y = sqrt1MinusU2 * sin(ang1);
	float z = u;
	return float3(x, y, z);
	
	/*
	while (true)
	{
		float3 P = float3(Random(seed.xy), Random(seed.xy + seed.z), Random(seed.xy - seed.z));
		P -= 0.5f;
		P *= 2.0f;
		if (dot(P, P) >= 1)
			continue;

		return P;
	}
	return 0;
	*/
}

float3 xRandomInUnitSphere(float3 uv)
{
	float2 Rand;

	NumberGenerator R;
	R.SetSeed(floor(uv.x * 512));	
	Rand.x = R.GetRandomFloat(0.0, 1.0);
	R.SetSeed(floor(uv.y * 512));
	Rand.y = R.GetRandomFloat(0.0, 1.0);	
	Rand -= 0.5f;
	Rand *= 2.0f;
	float ang1 = (Rand.x + 1.0) * PI; // [-1..1) -> [0..2*PI)
	float u = Rand.y; // [-1..1), cos and acos(2v-1) cancel each other out, so we arrive at [-1..1)
	float u2 = u * u;
	float sqrt1MinusU2 = sqrt(1.0 - u2);
	float x = sqrt1MinusU2 * cos(ang1);
	float y = sqrt1MinusU2 * sin(ang1);
	float z = u;
	return float3(x, y, z);
}

float3 RandomHemispherePoint(float3 seed, float3 n) 
{
	/**
	 * Generate random sphere point and swap vector along the normal, if it
	 * points to the wrong of the two hemispheres.
	 * This method provides a uniform distribution over the hemisphere,
	 * provided that the sphere distribution is also uniform.
	 */
	float3 v = RandomInUnitSphere(seed);
	return v * sign(dot(v, n));
}

float3 RandomDiskPoint(float3 seed, float3 n) 
{
	float3 Rand = float3(Random(seed.xy), Random(seed.xy + seed.z), Random(seed.xy - seed.z));
	Rand -= 0.5f;
	Rand *= 2.0f;
	float r = Rand.x * 0.5 + 0.5; // [-1..1) -> [0..1)
	float Angle = (Rand.y + 1.0) * PI; // [-1..1] -> [0..2*PI)
	float sr = sqrt(r);
	float2 P = float2(sr * cos(Angle), sr * sin(Angle));
	/*
	 * Compute some arbitrary tangent space for orienting
	 * our disk towards the normal. We use the camera's up vector
	 * to have some fix reference vector over the whole screen.
	 */
	float3 Tangent = normalize(Rand);
	float3 Bitangent = cross(Tangent, n);
	Tangent = cross(Bitangent, n);

	/* Make our disk orient towards the normal. */
	return Tangent * P.x + Bitangent * P.y;
}

bool NearZero(float3 v)
{
	const float s = 1e-8;
	return (abs(v.x) < s) && (abs(v.y) < s) && (abs(v.z) < s);
}


float3 Reflect(float3 v, float3 n) 
{
	return v - 2 * dot(v, n)*n;
}

float3 Refract(float3 uv, float3 n, float etai_over_etat)
{
	float cos_theta = min(dot(-uv, n), 1.0);
	float3 r_out_perp = etai_over_etat * (uv + cos_theta * n);
	float3 r_out_parallel = -sqrt(abs(1.0 - dot(r_out_perp, r_out_perp))) * n;
	return r_out_perp + r_out_parallel;
}

float Reflectance(float cosine, float ref_idx)
{
	// Use Schlick's approximation for reflectance.
	float r0 = (1 - ref_idx) / (1 + ref_idx);
	r0 = r0 * r0;
	return r0 + (1 - r0) * pow((1 - cosine), 5);
}

/*
float3 RandomInUnitSphere(int s)
{
	NumberGenerator R;
	R.SetSeed(s);

	while (true)
	{
		float3 P = float3(R.GetRandomFloat(0.0, 2.0), R.GetRandomFloat(0.0, 2.0), R.GetRandomFloat(0.0, 2.0));			
		P -= 1.0f;
		if (dot(P, P) >= 1)
			continue;

		return P;
	}
	return 0;
}
*/
