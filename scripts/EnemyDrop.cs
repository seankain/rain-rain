using Godot;
using System;

public partial class EnemyDrop : RigidBody2D
{
	private const int FaceCols = 10;
	private const int FaceRows = 10;
	private const int FaceW = 327;
	private const int FaceH = 252;

	[Export]
	public AnimatedSprite2D anim;
	private Sprite2D faceSprite;

	public override void _Ready()
	{
		this.BodyEntered += HandleCollision;
		this.anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		this.anim.AnimationFinished += HandleAnimationFinished;

		faceSprite = GetNode<Sprite2D>("FaceSprite");
		var texture = GD.Load<Texture2D>("res://assets/drop_faces.png");
		int faceIndex = GD.RandRange(0, FaceCols * FaceRows - 1);
		var atlas = new AtlasTexture();
		atlas.Atlas = texture;
		atlas.Region = new Rect2(
			(faceIndex % FaceCols) * FaceW,
			(faceIndex / FaceCols) * FaceH,
			FaceW, FaceH
		);
		faceSprite.Texture = atlas;
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
