using Godot;
using System;
using System.Collections.Generic;

public enum GameState
{
	Pause,
	Playing,
	Dead
}
public partial class Game : Node2D
{

	[Export]
	public double cooldown = 5;

	[Export]
	private Hud hud;

	[Export]
	private Timer respawnTimer;

	private double elapsed = 0;

	private uint enemiesSpawned = 0;

	[Export]
	private Player player;

    [Export]
    public PackedScene EnemyScene;

	private List<Node2D> enemyNodes = new();

	private GameState gameState = GameState.Playing;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
    {
		player.Died += HandlePlayerDeath;
		respawnTimer.Timeout += HandleRespawnTimer;
    }

    private void HandlePlayerDeath()
    {
		gameState = GameState.Dead;
		hud.SetMessage("YOU ARE DEAD.", 10);
		respawnTimer.Start();
    }

	private void HandleRespawnTimer()
	{
		gameState = GameState.Playing;
		player.Revive();
		hud.SetMessage("GO!", 2);
	}

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
		if (gameState != GameState.Playing) return;

		elapsed+=delta;
		if(elapsed >= cooldown){
			// todo: handle resolution changes
			SpawnEnemy(new Vector2 { X = Random.Shared.Next(-580, 650), Y = -200 });
			enemiesSpawned++;
			hud.SetScore(enemiesSpawned);
			elapsed = 0;
		}
	}

	private void SpawnEnemy(Vector2 pos){
		var p = GD.Load<PackedScene>(EnemyScene.ResourcePath);
        var enemy = p.Instantiate();
        AddChild(enemy);
        ((Node2D)enemy).GlobalPosition = pos;
        enemyNodes.Add((Node2D)enemy);
	}
}
