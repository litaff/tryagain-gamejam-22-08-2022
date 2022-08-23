using System;
using _Scripts;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Character : MonoBehaviour
{
	public float maxJumpHeight = 4;
	public float minJumpHeight = 1;
	public float timeToJumpApex = 0.4f;
	public float moveSpeed = 6;
	public float wallSlideSpeedMax = 3;
	public float wallStickTime = 0.25f;
	public Vector2 wallJumpClimb;
	public Vector2 wallJumpOff;
	public Vector2 wallLeap;

	[SerializeField] private float accelerationTimeGrounded = 0.1f;
	[SerializeField] private float accelerationTimeAirborne = 0.2f;
	
	private float _gravity;
	private float _maxJumpVelocity;
	private float _minJumpVelocity;
	private float _velocityXSmoothing;
	private float _timeToWallUnstick;
	private Controller2D _controller;
	private Vector3 _velocity;

	public void Init(float gravity, float maxJumpVelocity, float minJumpVelocity)
	{
		_controller = GetComponent<Controller2D>();
		
		_gravity = gravity;
		_maxJumpVelocity = maxJumpVelocity;
		_minJumpVelocity = minJumpVelocity;
	}

	public void UpdateCharacter(CharacterInput input)
	{
		var wallDirectionX = _controller.collisions.left ? -1 : 1;

		var targetVelocityX = input.basicInput.x * moveSpeed;
		_velocity.x = Mathf.SmoothDamp(
			_velocity.x, targetVelocityX, ref _velocityXSmoothing, (_controller.collisions.below) 
				? accelerationTimeGrounded 
				: accelerationTimeAirborne);

		var wallSliding = false;

		if ((_controller.collisions.left || _controller.collisions.right) 
		    && !_controller.collisions.below && _velocity.y < 0)
		{
			wallSliding = true;

			if (_velocity.y < -wallSlideSpeedMax)
				_velocity.y = -wallSlideSpeedMax;

			if (_timeToWallUnstick > 0)
			{
				_velocityXSmoothing = 0;
				_velocity.x = 0;
				if (Math.Abs(input.basicInput.x - wallDirectionX) > 0.0001f && input.basicInput.x != 0)
				{
					_timeToWallUnstick -= Time.deltaTime;
				}
				else
				{
					_timeToWallUnstick = wallStickTime;
				}
			}
			else
				_timeToWallUnstick = wallStickTime;
		}

		if (input.jumpButtonDown)
		{
			if (wallSliding)
			{
				if (Math.Abs(wallDirectionX - input.basicInput.x) < 0.0001f)
				{
					_velocity.x = -wallDirectionX * wallJumpClimb.x;
					_velocity.y = wallJumpClimb.y;
				}
				else if (input.basicInput.x == 0)
				{
					_velocity.x = -wallDirectionX * wallJumpOff.x;
					_velocity.y = wallJumpOff.y;
				}
				else
				{
					_velocity.x = -wallDirectionX * wallLeap.x;
					_velocity.y = wallLeap.y;
				}
			}

			if (_controller.collisions.below)
				if (_controller.collisions.slidingDownMaxSlope)
				{
					if (Math.Abs(input.basicInput.x - (-Mathf.Sign(_controller.collisions.slopeNormal.x))) > 0.0001f) // not jump on max slope
					{
						_velocity.y = _maxJumpVelocity * _controller.collisions.slopeNormal.y;
						_velocity.x = _maxJumpVelocity * _controller.collisions.slopeNormal.x;
					}
				}
				else
					_velocity.y = _maxJumpVelocity;
		}
		if (input.jumpButtonUp)
		{
			if (_velocity.y > _minJumpVelocity)
				_velocity.y = _minJumpVelocity;
		}

		_velocity.y += _gravity * Time.deltaTime;

		_controller.Move(_velocity * Time.deltaTime, input.basicInput);

		if (_controller.collisions.above || _controller.collisions.below)
			if (_controller.collisions.slidingDownMaxSlope)
				_velocity.y += _controller.collisions.slopeNormal.y * -_gravity * Time.deltaTime;
			else
				_velocity.y = 0;
	}
	
	protected virtual void Start()
	{
		_controller = GetComponent<Controller2D>();

		/*_gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
		_maxJumpVelocity = Mathf.Abs(_gravity) * timeToJumpApex;
		_minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(_gravity) * minJumpHeight);*/
	}

	/*private void Update()
	{
		var input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

		var wallDirectionX = _controller.collisions.left ? -1 : 1;

		var targetVelocityX = input.x * moveSpeed;
		_velocity.x = Mathf.SmoothDamp(
			_velocity.x, targetVelocityX, ref _velocityXSmoothing, (_controller.collisions.below) 
				? accelerationTimeGrounded 
				: accelerationTimeAirborne);

		var wallSliding = false;

		if ((_controller.collisions.left || _controller.collisions.right) 
		    && !_controller.collisions.below && _velocity.y < 0)
		{
			wallSliding = true;

			if (_velocity.y < -wallSlideSpeedMax)
				_velocity.y = -wallSlideSpeedMax;

			if (_timeToWallUnstick > 0)
			{
				_velocityXSmoothing = 0;
				_velocity.x = 0;
				if (Math.Abs(input.x - wallDirectionX) > 0.0001f && input.x != 0)
				{
					_timeToWallUnstick -= Time.deltaTime;
				}
				else
				{
					_timeToWallUnstick = wallStickTime;
				}
			}
			else
				_timeToWallUnstick = wallStickTime;
		}

		if (Input.GetKeyDown(KeyCode.Space))
		{
			if (wallSliding)
			{
				if (Math.Abs(wallDirectionX - input.x) < 0.0001f)
				{
					_velocity.x = -wallDirectionX * wallJumpClimb.x;
					_velocity.y = wallJumpClimb.y;
				}
				else if (input.x == 0)
				{
					_velocity.x = -wallDirectionX * wallJumpOff.x;
					_velocity.y = wallJumpOff.y;
				}
				else
				{
					_velocity.x = -wallDirectionX * wallLeap.x;
					_velocity.y = wallLeap.y;
				}
			}

			if (_controller.collisions.below)
				if (_controller.collisions.slidingDownMaxSlope)
				{
					if (Math.Abs(input.x - (-Mathf.Sign(_controller.collisions.slopeNormal.x))) > 0.0001f) // not jump on max slope
					{
						_velocity.y = _maxJumpVelocity * _controller.collisions.slopeNormal.y;
						_velocity.x = _maxJumpVelocity * _controller.collisions.slopeNormal.x;
					}
				}
				else
					_velocity.y = _maxJumpVelocity;
		}
		if (Input.GetKeyUp(KeyCode.Space))
		{
			if (_velocity.y > _minJumpVelocity)
				_velocity.y = _minJumpVelocity;
		}

		_velocity.y += _gravity * Time.deltaTime;

		_controller.Move(_velocity * Time.deltaTime, input);

		if (_controller.collisions.above || _controller.collisions.below)
			if (_controller.collisions.slidingDownMaxSlope)
				_velocity.y += _controller.collisions.slopeNormal.y * -_gravity * Time.deltaTime;
			else
				_velocity.y = 0;
	}*/
}