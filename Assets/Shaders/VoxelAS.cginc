//----------------------------------------------------------------------------------------
// 
//----------------------------------------------------------------------------------------
VoxelAccelerationStructure _VoxelAccelerationStructure(int num_leaves, StructuredBuffer<TreeNodeData> node, StructuredBuffer<TreeLeafData> leaf, StructuredBuffer<VoxelData> blas, int offset)
{
	VoxelAccelerationStructure AS;

	AS.NumLeaves = num_leaves;
	AS.Node = node;
	AS.Leaf = leaf;
	AS.BLAS = blas;	
	AS.StackOffset = offset;

	TreeStack[offset] = 0;

	return AS;
}
//----------------------------------------------------------------------------------------
// 
//----------------------------------------------------------------------------------------
bool Node_Overlap(Ray r, TreeNodeData node, float t_max)
{	
	return Fast_AABB_Overlap(r, node.Min, node.Max, t_max);
}
//----------------------------------------------------------------------------------------
// 
//----------------------------------------------------------------------------------------
bool Leaf_Overlap(Ray r, TreeLeafData leaf, float t_max)
{
	return Fast_AABB_Overlap(r, leaf.Min, leaf.Max, t_max);
}
//----------------------------------------------------------------------------------------
// 
//----------------------------------------------------------------------------------------
void ClearThreadStack(VoxelAccelerationStructure as)
{
	int O;
	InterlockedExchange(TreeStack[as.StackOffset], 0, O);
}
//----------------------------------------------------------------------------------------
// 
//----------------------------------------------------------------------------------------
void PushNode(VoxelAccelerationStructure as, int v)
{		
	int O;
	InterlockedExchange(TreeStack[as.StackOffset + TreeStack[as.StackOffset] + 1], v, O);
	InterlockedAdd(TreeStack[as.StackOffset], 1);	
}
//----------------------------------------------------------------------------------------
// 
//----------------------------------------------------------------------------------------
int PopNode(VoxelAccelerationStructure as)
{
	InterlockedAdd(TreeStack[as.StackOffset], -1);
	
	return TreeStack[as.StackOffset + TreeStack[as.StackOffset] + 1];
}
//----------------------------------------------------------------------------------------
// BVH based traversal of the voxel scene
//----------------------------------------------------------------------------------------
bool VoxelAccelerationStructure_Hit(VoxelAccelerationStructure as, Ray r, float t_min, float t_max, inout HitRecord rec)
{
	bool bHitAnything = false;
	float ClosestT = t_max;
	AABB Aabb;
	Material Mat = _Material(MAT_LAMBERTIAN, float3(0, 0, 0), 0.0);
	
	ClearThreadStack(as);

	PushNode(as, 0);	
	
	while (true)
	{
		int CurrentNode = PopNode(as);
		if (Node_Overlap(r, as.Node[CurrentNode], ClosestT))
		{
			for (int i = 0; i < 2; i++)
			{
				if (as.Node[CurrentNode].Node[i] >= 0)
				{
					PushNode(as, as.Node[CurrentNode].Node[i]);
				}
				else
				{
					int LeafIndex = -as.Node[CurrentNode].Node[i] - 1;
					if (Leaf_Overlap(r, as.Leaf[LeafIndex], ClosestT))
					{
						for (int v = 0; v < as.Leaf[LeafIndex].NumVoxels; v++)
						{
							_AABB(Aabb, as.BLAS[as.Leaf[LeafIndex].StartVoxel + v].Pos - (float3(0.5f, 0.5f, 0.5f)), as.BLAS[as.Leaf[LeafIndex].StartVoxel + v].Pos + (float3(0.5f, 0.5f, 0.5f)));
							Mat.Albedo = as.BLAS[as.Leaf[LeafIndex].StartVoxel + v].Albedo;
							if (Fast_AABB_Hit(r, Aabb, Mat, t_min, ClosestT, rec))														
							{
								bHitAnything = true;
							}							
						}
					}
				}
			}
		}
		if (TreeStack[as.StackOffset] <= 0)
			return bHitAnything;
	}	
	return bHitAnything;
}
//----------------------------------------------------------------------------------------
// Brute-force traversal of the voxel scene
//----------------------------------------------------------------------------------------
bool PrmitiveList_Hit(VoxelAccelerationStructure as, Ray r, float t_min, float t_max, inout HitRecord rec)
{
	bool bHitAnything = false;
	float ClosestT = t_max;
	AABB Aabb;
	Material Mat = _Material(MAT_LAMBERTIAN, float3(0, 0, 0), 0.0);

	for (int i = 0; i < as.NumLeaves; i++)
	{
		int LeafIndex = i;
		for (int v = 0; v < as.Leaf[LeafIndex].NumVoxels; v++)
		{
			_AABB(Aabb, as.BLAS[as.Leaf[LeafIndex].StartVoxel + v].Pos - float3(0.5f, 0.5f, 0.5f), as.BLAS[as.Leaf[LeafIndex].StartVoxel + v].Pos + float3(0.5f, 0.5f, 0.5f));
			Mat.Albedo = as.BLAS[as.Leaf[LeafIndex].StartVoxel + v].Albedo;
			if (Fast_AABB_Hit(r, Aabb, Mat, t_min, ClosestT, rec))
				bHitAnything = true;
		}
	}
	return bHitAnything;
}
//----------------------------------------------------------------------------------------
// 
//----------------------------------------------------------------------------------------
bool FastPrmitiveList_Hit(VoxelAccelerationStructure as, Ray r, float t_min, float t_max, inout HitRecord rec)
{
	bool bHitAnything = false;
	float ClosestT = t_max;
	AABB Aabb;
	Material Mat = _Material(MAT_LAMBERTIAN, float3(0, 0, 0), 0.0);
	
	for (int i = 0; i < as.NumLeaves; i++)
	{
		if (Leaf_Overlap(r, as.Leaf[i], ClosestT))
		{
			int LeafIndex = i;
			for (int v = 0; v < as.Leaf[LeafIndex].NumVoxels; v++)
			{
				_AABB(Aabb, as.BLAS[as.Leaf[LeafIndex].StartVoxel + v].Pos - float3(0.5f, 0.5f, 0.5f), as.BLAS[as.Leaf[LeafIndex].StartVoxel + v].Pos + float3(0.5f, 0.5f, 0.5f));
				Mat.Albedo = as.BLAS[as.Leaf[LeafIndex].StartVoxel + v].Albedo;
				if (Fast_AABB_Hit(r, Aabb, Mat, t_min, ClosestT, rec))
					bHitAnything = true;
			}			
		}
	}

	return bHitAnything;
}
