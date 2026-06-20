using Godot;
using System;

public partial class EnemyDrop : RigidBody2D
{
	[Export]
	public AnimatedSprite2D anim;

	public override void _Ready()
	{
		this.BodyEntered += HandleCollision;
		this.anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		this.anim.AnimationFinished += HandleAnimationFinished;

		int frameCount = this.anim.SpriteFrames.GetFrameCount("default");
		this.anim.Animation = "default";
		this.anim.Frame = GD.RandRange(0, frameCount - 1);
	}

    private void HandleAnimationFinished()
	{
        if(this.anim.Animation == "splash")
        {
			this.Visible = false;
			this.QueueFree();
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}

	public void HandleCollision(Node body){
        if (body.IsInGroup("player"))
        {
			// The drop's fall speed at the moment of impact determines whether
			// this collision is hard enough to count as a crash.
			float magnitude = this.LinearVelocity.Length();
			((Player)body).Hit(magnitude);
        }
		anim.Play("splash");
	}
}
