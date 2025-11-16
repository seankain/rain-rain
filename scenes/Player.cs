using Godot;
using System;

public delegate void DeathEventHandler();
public partial class Player : CharacterBody2D
{
	public event DeathEventHandler Died;

	public const float Speed = 600.0f;
	public const float JumpVelocity = -400.0f;

	private bool Dead = false;

	[Export]
	private AnimatedSprite2D anim;

	public override void _Ready()
	{
	}

	public void Hit()
	{
		if(Dead){ return; }
		this.Died?.Invoke();
		Die();
	}

	public void Die()
	{
		Dead = true;
		anim.Rotate(Mathf.DegToRad(90));
    }
	public override void _PhysicsProcess(double delta)
	{
		if(Dead){ return; }
		Vector2 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}


}
