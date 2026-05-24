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
	private double umbrellaElapsed = 0;
	private const double UmbrellaCooldown = 15.0;

	private uint enemiesSpawned = 0;

	[Export]
	private Player player;

	[Export]
	public PackedScene EnemyScene;

	[Export]
	public PackedScene UmbrellaScene;

	private List<Node2D> enemyNodes = new();

	private GameState gameState = GameState.Playing;

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
		enemiesSpawned = 0;
		umbrellaElapsed = 0;
		hud.SetScore(enemiesSpawned);
		player.Revive();
		hud.SetMessage("GO!", 2);
	}

	public override void _Process(double delta)
	{
		if (gameState != GameState.Playing) return;

		elapsed += delta;
		if (elapsed >= cooldown)
		{
			SpawnEnemy(new Vector2 { X = Random.Shared.Next(-580, 650), Y = -200 });
			enemiesSpawned++;
			hud.SetScore(enemiesSpawned);
			elapsed = 0;
		}

		umbrellaElapsed += delta;
		if (umbrellaElapsed >= UmbrellaCooldown)
		{
			SpawnUmbrella(new Vector2 { X = Random.Shared.Next(-500, 500), Y = 200 });
			umbrellaElapsed = 0;
		}
	}

	private void SpawnEnemy(Vector2 pos)
	{
		var p = GD.Load<PackedScene>(EnemyScene.ResourcePath);
		var enemy = p.Instantiate();
		AddChild(enemy);
		((Node2D)enemy).GlobalPosition = pos;
		enemyNodes.Add((Node2D)enemy);
	}

	private void SpawnUmbrella(Vector2 pos)
	{
		if (UmbrellaScene == null) return;
		var p = GD.Load<PackedScene>(UmbrellaScene.ResourcePath);
		var pickup = p.Instantiate();
		AddChild(pickup);
		((Node2D)pickup).GlobalPosition = pos;
	}
}
