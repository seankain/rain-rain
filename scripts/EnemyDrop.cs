using Godot;
using System;

public partial class EnemyDrop : RigidBody2D
{
	[Export]
	public AnimatedSprite2D anim;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.BodyEntered += HandleCollision;
		this.anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		this.anim.AnimationFinished += HandleAnimationFinished;
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
			((Player)body).Hit();
        }
		anim.Play("splash");
	}
}
