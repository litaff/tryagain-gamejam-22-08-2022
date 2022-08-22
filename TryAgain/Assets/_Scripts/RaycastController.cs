using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RaycastController : MonoBehaviour
{
	[Header("Raycast variables")]

	#region Public variables

	[Tooltip("Used to determine the target layer for a raycast")]
	public LayerMask collisionMask;

	[Tooltip("The density of raycasts")]
	public float distanceBetweenRays = 0.25f;

	#endregion Public variables

	#region Protected variables

	protected RaycastOrigins RaycastOrigins;
	protected const float SkinWidth = 0.015f;
	protected float HorizontalRaySpacing;
	protected float VerticalRaySpacing;
	protected int HorizontalRayCount;
	protected int VerticalRayCount;

	#endregion Protected variables

	private BoxCollider2D _boxCollider;

	public virtual void Start()
	{
		_boxCollider = GetComponent<BoxCollider2D>();

		CalculateRaySpacing();
	}

	protected void UpdateRaycastOrigins()
	{
		var bounds = _boxCollider.bounds;
		bounds.Expand(SkinWidth * -2);

		RaycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
		RaycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
		RaycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
		RaycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
	}

	private void CalculateRaySpacing()
	{
		var bounds = _boxCollider.bounds;
		bounds.Expand(SkinWidth * -2);

		var boundsWidth = bounds.size.x;
		var boundsHeight = bounds.size.y;

		HorizontalRayCount = Mathf.RoundToInt(boundsHeight / distanceBetweenRays);
		VerticalRayCount = Mathf.RoundToInt(boundsWidth / distanceBetweenRays);

		HorizontalRayCount = Mathf.Clamp(HorizontalRayCount, 2, int.MaxValue);
		VerticalRayCount = Mathf.Clamp(VerticalRayCount, 2, int.MaxValue);

		HorizontalRaySpacing = bounds.size.y / (HorizontalRayCount - 1);
		VerticalRaySpacing = bounds.size.x / (VerticalRayCount - 1);
	}
}

[System.Serializable]
public struct RaycastOrigins
{
	public Vector2 topLeft, topRight;
	public Vector2 bottomLeft, bottomRight;
}