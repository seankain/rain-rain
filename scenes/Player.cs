using Godot;
using System;

public delegate void DeathEventHandler();
public partial class Player : CharacterBody2D
{
	public event DeathEventHandler Died;

	public const float Speed = 600.0f;
	public const float JumpVelocity = -400.0f;

	private bool Dead = false;
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

	public void Hit()
	{
		if (Dead || HasUmbrella) return;
		this.Died?.Invoke();
		Die();
	}

	public void Die()
	{
		Dead = true;
		anim.Rotate(Mathf.DegToRad(90));
	}

	public void Revive()
	{
		Dead = false;
		HasUmbrella = false;
		umbrellaTimeRemaining = 0;
		anim.Rotation = 0;
		anim.Play("idle");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Dead) return;

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
