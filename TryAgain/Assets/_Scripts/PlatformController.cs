using System;
using UnityEngine;
using System.Collections.Generic;

public class PlatformController : RaycastController
{
	[Header("Platform variables")]

	#region Public variables

	[Tooltip("Layer which the platform will carry")]
	public LayerMask passengerMask;

	[Tooltip("Waypoints for the platform")]
	public Vector3[] localWaypoints = new Vector3[1];

	[Tooltip("The speed of the platform")]
	public float speed = 1;

	[Tooltip("Should the platform go to the first waypoint after the last one")]
	public bool cyclic;

	[Tooltip("How much time the platform will wait at each waypoint")]
	public float waitTime;

	[Range(0f, 2f)]
	[Tooltip("How much the platform should ease in on a waypoint")]
	public float easeAmount;

	#endregion Public variables

	#region Private variables

	private int _fromWaypointIndex;
	private float _percentBetweenWaypoints;
	private float _nextMoveTime;
	private Vector3[] _globalWaypoints;
	private List<PassengerMovement> _passengerMovements = new List<PassengerMovement>();
	private readonly Dictionary<Transform, Controller2D> _passengerDictionary =
		new Dictionary<Transform, Controller2D>();

	#endregion Private variables

	public override void Start()
	{
		base.Start();

		_globalWaypoints = new Vector3[localWaypoints.Length];

		for (var i = 0; i < localWaypoints.Length; i++)
		{
			_globalWaypoints[i] = localWaypoints[i] + transform.position;
		}
	}

	private void Update()
	{
		UpdateRaycastOrigins();

		var velocity = CalculatePlatformMovement();

		CalculatePassengerMovement(velocity);

		MovePassengers(true);

		transform.Translate(velocity);
		Physics2D.SyncTransforms();

		MovePassengers(false);
	}

	private void MovePassengers(bool beforeMovePlatform)
	{
		foreach (var passenger in _passengerMovements)
		{
			if (!_passengerDictionary.ContainsKey(passenger.transform))
				_passengerDictionary.Add(passenger.transform, passenger.transform.GetComponent<Controller2D>());
			if (passenger.moveBeforePlatform == beforeMovePlatform)
				_passengerDictionary[passenger.transform].Move(passenger.velocity, passenger.standingOnPlatform);
		}
	}

	private Vector3 CalculatePlatformMovement()
	{
		if (Time.time < _nextMoveTime)
		{
			return Vector3.zero;
		}
		_fromWaypointIndex %= _globalWaypoints.Length;
		var toWaypointIndex = (_fromWaypointIndex + 1) % _globalWaypoints.Length;
		var distanceBetweenWaypoints =
			Vector3.Distance(_globalWaypoints[_fromWaypointIndex], _globalWaypoints[toWaypointIndex]);
		_percentBetweenWaypoints += Time.deltaTime * speed / distanceBetweenWaypoints;
		_percentBetweenWaypoints = Mathf.Clamp01(_percentBetweenWaypoints);
		var easedPercentBetweenWaypoints = Ease(_percentBetweenWaypoints);

		var newPosition = Vector3.Lerp(_globalWaypoints[_fromWaypointIndex], _globalWaypoints[toWaypointIndex],
			easedPercentBetweenWaypoints);

		if (_percentBetweenWaypoints >= 1)
		{
			_percentBetweenWaypoints = 0;
			_fromWaypointIndex++;
			if (!cyclic)
			{
				if (_fromWaypointIndex >= _globalWaypoints.Length - 1)
				{
					_fromWaypointIndex = 0;
					Array.Reverse(_globalWaypoints);
				}
			}
			_nextMoveTime = Time.time + waitTime;
		}

		return newPosition - transform.position;
	}

	private void CalculatePassengerMovement(Vector3 velocity)
	{
		var movedPassengers = new HashSet<Transform>();
		_passengerMovements = new List<PassengerMovement>();

		var directionX = Mathf.Sign(velocity.x);
		var directionY = Mathf.Sign(velocity.y);

		// Vertically moving platform
		if (velocity.y != 0)
		{
			var rayLength = Mathf.Abs(velocity.y) + SkinWidth;

			for (var i = 0; i < VerticalRayCount; i++)
			{
				var rayOrigin = (Math.Abs(directionY - (-1)) < 0.0001f)
					? RaycastOrigins.bottomLeft
					: RaycastOrigins.topLeft;
				rayOrigin += Vector2.right * (VerticalRaySpacing * i);
				var hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, passengerMask);

				if (!hit || hit.distance == 0) continue;

				if (movedPassengers.Contains(hit.transform)) continue;

				movedPassengers.Add(hit.transform);
				var pushX = (Math.Abs(directionY - 1) < 0.0001f) ? velocity.x : 0;
				var pushY = velocity.y - (hit.distance - SkinWidth) * directionY;

				_passengerMovements.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY),
					Math.Abs(directionY - 1) < 0.0001f, true));
			}
		}

		// Horizontally moving platform
		if (velocity.x != 0)
		{
			var rayLength = Mathf.Abs(velocity.x) + SkinWidth;

			for (var i = 0; i < HorizontalRayCount; i++)
			{
				var rayOrigin = (Math.Abs(directionX - (-1)) < 0.0001f)
					? RaycastOrigins.bottomLeft
					: RaycastOrigins.bottomRight;
				rayOrigin += Vector2.up * (HorizontalRaySpacing * i);
				var hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, passengerMask);

				if (!hit || hit.distance == 0) continue;

				if (movedPassengers.Contains(hit.transform)) continue;

				movedPassengers.Add(hit.transform);
				var pushX = velocity.x - (hit.distance - SkinWidth) * directionX;
				const float pushY = -SkinWidth;

				_passengerMovements.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY),
					false, true));
			}
		}

		// Passenger on top of a horizontally or downward moving platform
		if (Math.Abs(directionY - (-1)) < 0.0001f || velocity.y == 0 && velocity.x != 0)
		{
			const float rayLength = SkinWidth * 2;

			for (var i = 0; i < VerticalRayCount; i++)
			{
				var rayOrigin = RaycastOrigins.topLeft + Vector2.right * (VerticalRaySpacing * i);
				var hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, passengerMask);

				if (!hit || hit.distance == 0) continue;

				if (movedPassengers.Contains(hit.transform)) continue;

				movedPassengers.Add(hit.transform);
				var pushX = velocity.x;
				var pushY = velocity.y;

				_passengerMovements.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY),
					true, false));
			}
		}
	}

	private float Ease(float x)
	{
		var a = easeAmount + 1;
		return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
	}

	private void OnDrawGizmos()
	{
		if (localWaypoints != null)
		{
			Gizmos.color = Color.red;
			const float size = 0.3f;

			for (var i = 0; i < localWaypoints.Length; i++)
			{
				var globalWaypointPos = (Application.isPlaying) 
					? _globalWaypoints[i] 
					: localWaypoints[i] + transform.position;
				Gizmos.DrawLine(globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size);
				Gizmos.DrawLine(globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);
			}
		}
	}
}

[Serializable]
public struct PassengerMovement
{
	public Transform transform;
	public Vector3 velocity;
	public bool standingOnPlatform;
	public bool moveBeforePlatform;

	public PassengerMovement(Transform transform, Vector3 velocity, bool standingOnPlatform, bool moveBeforePlatform)
	{
		this.transform = transform;
		this.velocity = velocity;
		this.standingOnPlatform = standingOnPlatform;
		this.moveBeforePlatform = moveBeforePlatform;
	}
}