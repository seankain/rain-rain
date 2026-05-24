using Godot;

public partial class UmbrellaPickup : Area2D
{
	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body.IsInGroup("player"))
		{
			((Player)body).PickupUmbrella();
			QueueFree();
		}
	}
}
