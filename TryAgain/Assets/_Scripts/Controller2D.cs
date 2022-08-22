using System;
using UnityEngine;

public class Controller2D : RaycastController
{
	[Header("Controller variables")]
	public float maxSlopeAngle = 80;

	public CollisionInfo collisions;

	private Vector2 _characterInput;

	public override void Start()
	{
		base.Start();
		collisions.faceDirection = 1;
	}

	public void Move(Vector3 velocity, bool standingOnPlatform)
	{
		Move(velocity, Vector2.zero, standingOnPlatform);
	}

	public void Move(Vector3 velocity, Vector2 input, bool standingOnPlatform = false)
	{
		UpdateRaycastOrigins();

		collisions.Reset();

		collisions.velocityOld = velocity;

		_characterInput = input;

		if (velocity.y < 0)
			velocity = DescendSlope(velocity);

		if (velocity.x != 0)
			collisions.faceDirection = (int)Mathf.Sign(velocity.x);

		velocity = HorizontalCollisions(velocity);

		if (velocity.y != 0)
			velocity = VerticalCollisions(velocity);

		transform.Translate(velocity);
		Physics2D.SyncTransforms();

		if (!standingOnPlatform) return;

		collisions.below = true;
	}

	private Vector3 VerticalCollisions(Vector3 velocity)
	{
		var directionY = Mathf.Sign(velocity.y);
		var rayLength = Mathf.Abs(velocity.y) + SkinWidth;

		for (var i = 0; i < VerticalRayCount; i++)
		{
			var rayOrigin = (Math.Abs(directionY - (-1)) < 0.0001f) 
				? RaycastOrigins.bottomLeft 
				: RaycastOrigins.topLeft;
			rayOrigin += Vector2.right * (VerticalRaySpacing * i + velocity.x);
			var hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

			Debug.DrawRay(rayOrigin, Vector2.up * (directionY * rayLength), Color.red);

			if (!hit) continue;

			if (hit.collider.CompareTag("Through"))
			{
				if (Math.Abs(directionY - 1) < 0.0001f || hit.distance == 0) continue;
				if (collisions.fallingThroughPlatform) continue;
				if (Math.Abs(_characterInput.y - (-1)) < 0.0001f)
				{
					collisions.fallingThroughPlatform = true;
					Invoke(nameof(ResetFallingThroughPlatform), 0.5f);
					continue;
				}
			}

			velocity.y = (hit.distance - SkinWidth) * directionY;
			rayLength = hit.distance;

			if (collisions.climbingSlope)
				velocity.x = velocity.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad * Mathf.Sign(velocity.x));

			collisions.below = Math.Abs(directionY - (-1)) < 0.0001f;
			collisions.above = Math.Abs(directionY - 1) < 0.0001f;
		}

		if (collisions.climbingSlope)
		{
			var directionX = Mathf.Sign(velocity.x);
			rayLength = Mathf.Abs(velocity.x) + SkinWidth;
			var rayOrigin = ((Math.Abs(directionX - (-1)) < 0.0001f) 
				? RaycastOrigins.bottomLeft 
				: RaycastOrigins.bottomRight) + Vector2.up * velocity.y;
			var hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

			if (!hit) return velocity;

			var slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

			if (Math.Abs(slopeAngle - collisions.slopeAngle) < 0.0001f) return velocity;

			velocity.x = (hit.distance - SkinWidth) * directionX;
			collisions.slopeAngle = slopeAngle;
			collisions.slopeNormal = hit.normal;
		}

		return velocity;
	}

	private Vector3 HorizontalCollisions(Vector3 velocity)
	{
		float directionX = collisions.faceDirection;
		var rayLength = Mathf.Abs(velocity.x) + SkinWidth;

		if (Mathf.Abs(velocity.x) < SkinWidth)
			rayLength = 2 * SkinWidth;

		for (var i = 0; i < HorizontalRayCount; i++)
		{
			var rayOrigin = (Math.Abs(directionX - (-1)) < 0.0001f) 
				? RaycastOrigins.bottomLeft 
				: RaycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (HorizontalRaySpacing * i);
			var hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

			Debug.DrawRay(rayOrigin, Vector2.right * (directionX * rayLength), Color.red);

			if (!hit) continue;

			if (hit.distance == 0) continue;

			var slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
			if (i == 0 && slopeAngle <= maxSlopeAngle)
			{
				if (collisions.descendingSlope)
				{
					collisions.descendingSlope = false;
					velocity = collisions.velocityOld;
				}
				float distanceToSlopeStart = 0;
				if (Math.Abs(slopeAngle - collisions.slopeAngleOld) > 0.0001f)
				{
					distanceToSlopeStart = hit.distance - SkinWidth;
					velocity.x -= distanceToSlopeStart * directionX;
				}
				velocity = ClimbSlope(velocity, slopeAngle, hit.normal);
				velocity.x += distanceToSlopeStart * directionX;
			}

			if (!collisions.climbingSlope || slopeAngle > maxSlopeAngle)
			{
				velocity.x = (hit.distance - SkinWidth) * directionX;
				rayLength = hit.distance;

				collisions.left = Math.Abs(directionX - (-1)) < 0.0001f;
				collisions.right = Math.Abs(directionX - 1) < 0.0001f;

				if (!collisions.climbingSlope) return velocity;

				velocity.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad * Mathf.Abs(velocity.x));
			}
		}

		return velocity;
	}

	private Vector3 ClimbSlope(Vector3 velocity, float angle, Vector2 slopeNormal)
	{
		var moveDistance = Mathf.Abs(velocity.x);
		var climbVelocityY = Mathf.Sin(angle * Mathf.Deg2Rad) * moveDistance;

		if (velocity.y > climbVelocityY) return velocity;

		velocity.y = climbVelocityY;
		velocity.x = Mathf.Cos(angle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
		collisions.below = true;
		collisions.climbingSlope = true;
		collisions.slopeAngle = angle;
		collisions.slopeNormal = slopeNormal;
		return velocity;
	}

	private Vector3 DescendSlope(Vector3 velocity)
	{
		var maxSlopeHitLeft = Physics2D.Raycast(RaycastOrigins.bottomLeft, Vector2.down,
			Mathf.Abs(velocity.y) + SkinWidth, collisionMask);
		var maxSlopeHitRight = Physics2D.Raycast(RaycastOrigins.bottomRight, Vector2.down,
			Mathf.Abs(velocity.y) + SkinWidth, collisionMask);
		if (maxSlopeHitLeft ^ maxSlopeHitRight)
		{
			velocity = SlideDownMaxSlope(maxSlopeHitRight, velocity);
			velocity = SlideDownMaxSlope(maxSlopeHitLeft, velocity);
		}

		if (collisions.slidingDownMaxSlope) return velocity;

		var directionX = Mathf.Sign(velocity.x);
		var rayOrigin = (Math.Abs(directionX - (-1)) < 0.0001f) 
			? RaycastOrigins.bottomRight 
			: RaycastOrigins.bottomLeft;
		var hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

		if (!hit) return velocity;

		var slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

		if (slopeAngle == 0 || slopeAngle > maxSlopeAngle) return velocity;

		if (Math.Abs(Mathf.Sign(hit.normal.x) - directionX) > 0.0001f) return velocity;

		if (hit.distance - SkinWidth > Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x)) return velocity;

		var moveDistance = Mathf.Abs(velocity.x);
		var descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

		velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
		velocity.y -= descendVelocityY;

		collisions.slopeAngle = slopeAngle;
		collisions.descendingSlope = true;
		collisions.below = true;
		collisions.slopeNormal = hit.normal;

		return velocity;
	}

	private Vector3 SlideDownMaxSlope(RaycastHit2D hit, Vector3 velocity)
	{
		if (!hit) return velocity;

		var slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

		if (slopeAngle <= maxSlopeAngle) return velocity;

		velocity.x = hit.normal.x * (Mathf.Abs(velocity.y) - hit.distance) / Mathf.Tan(slopeAngle * Mathf.Deg2Rad);

		collisions.slopeAngle = slopeAngle;
		collisions.slidingDownMaxSlope = true;
		collisions.slopeNormal = hit.normal;

		return velocity;
	}

	private void ResetFallingThroughPlatform()
	{
		collisions.fallingThroughPlatform = false;
	}
}

[Serializable]
public struct CollisionInfo
{
	public bool above, below;
	public bool left, right;
	public bool climbingSlope;
	public bool descendingSlope;
	public bool fallingThroughPlatform;
	public bool slidingDownMaxSlope;
	public float slopeAngle, slopeAngleOld;
	public int faceDirection;
	public Vector2 slopeNormal;
	public Vector3 velocityOld;

	public void Reset()
	{
		above = below = false;
		left = right = false;
		climbingSlope = false;
		descendingSlope = false;
		slidingDownMaxSlope = false;
		slopeNormal = Vector2.zero;

		slopeAngleOld = slopeAngle;
		slopeAngle = 0;
	}
}