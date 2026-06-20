using Godot;
using System;

public delegate void DeathEventHandler();
public delegate void CrashEventHandler();
public partial class Player : CharacterBody2D
{
	public event DeathEventHandler Died;
	public event CrashEventHandler Crashed;

	public const float Speed = 600.0f;
	public const float JumpVelocity = -400.0f;

	// Minimum collision impact magnitude (impact speed) that counts as a crash.
	[Export]
	public float CrashMagnitude = 700.0f;

	// How long the player is locked out of controlling the board and camera
	// after a crash, before the respawn flow is triggered.
	[Export]
	public double BailDuration = 1.5;

	private bool Dead = false;
	private bool Crashing = false;
	private double bailTimeRemaining = 0;
	private bool HasUmbrella = false;
	private double umbrellaTimeRemaining = 0;

	[Export]
	private AnimatedSprite2D anim;

	public override void _Ready()
	{
	}

	public void PickupUmbrella()
	{
		HasUmbrella = true;
		umbrellaTimeRemaining = 10.0;
	}

	public void Hit(float magnitude = 0.0f)
	{
		if (Dead || Crashing || HasUmbrella) return;

		// A sufficiently hard collision is a crash: the player bails and
		// temporarily loses control before the respawn flow kicks in.
		if (magnitude >= CrashMagnitude)
		{
			Crash();
			return;
		}

		this.Died?.Invoke();
		Die();
	}

	private void Crash()
	{
		Crashing = true;
		bailTimeRemaining = BailDuration;
		Velocity = Vector2.Zero;
		anim.Stop();
		anim.Rotate(Mathf.DegToRad(90));
		this.Crashed?.Invoke();
	}

	public void Die()
	{
		Dead = true;
		anim.Rotate(Mathf.DegToRad(90));
	}

	public void Revive()
	{
		Dead = false;
		Crashing = false;
		bailTimeRemaining = 0;
		HasUmbrella = false;
		umbrellaTimeRemaining = 0;
		anim.Rotation = 0;
		anim.Play("idle");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Dead) return;

		// While bailing from a crash the player can't steer the board (or the
		// camera). Let the board settle under gravity, ignore all input, then
		// hand off to the death/respawn flow once the bail duration elapses.
		if (Crashing)
		{
			bailTimeRemaining -= delta;

			Vector2 crashVelocity = Velocity;
			if (!IsOnFloor())
			{
				crashVelocity += GetGravity() * (float)delta;
			}
			crashVelocity.X = Mathf.MoveToward(crashVelocity.X, 0, Speed);
			Velocity = crashVelocity;
			MoveAndSlide();

			if (bailTimeRemaining <= 0)
			{
				// Already in the wipeout pose from the crash; just mark dead
				// and kick off the respawn flow.
				Crashing = false;
				Dead = true;
				this.Died?.Invoke();
			}
			return;
		}

		if (HasUmbrella)
		{
			umbrellaTimeRemaining -= delta;
			if (umbrellaTimeRemaining <= 0)
			{
				HasUmbrella = false;
				umbrellaTimeRemaining = 0;
			}
		}

		Vector2 velocity = Velocity;

		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		if (velocity.IsZeroApprox())
		{
			anim.Play(HasUmbrella ? "idle_umbrella" : "idle");
		}
		else
		{
			anim.Play(HasUmbrella ? "walk_umbrella" : "walk");
		}

		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * Speed;
			if (direction.X != 0)
			{
				anim.FlipH = direction.X < 0;
			}
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();

		WrapToScreen();
	}

	private void WrapToScreen()
	{
		Camera2D camera = GetViewport().GetCamera2D();
		if (camera == null) return;

		Vector2 viewSize = GetViewport().GetVisibleRect().Size / camera.Zoom;
		float halfWidth = viewSize.X / 2.0f;

		Vector2 center = camera.GetScreenCenterPosition();
		float left = center.X - halfWidth;
		float right = center.X + halfWidth;

		Vector2 pos = GlobalPosition;
		if (pos.X < left)
		{
			pos.X = right;
		}
		else if (pos.X > right)
		{
			pos.X = left;
		}
		GlobalPosition = pos;
	}
}
