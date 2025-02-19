using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

public partial class WaveManager : Node
{
	[Export]
	public PackedScene EnemyScene { get; set; }

	[Export]
	public Path2D EnemyPath { get; set; }
	private List<PathFollow2D> _activeEnemies = new List<PathFollow2D>();

	private int _waveNumber = 0;
	private float _pathLength = 3623.46f;

	public override void _Ready()
	{
		StartWave(5, 1);
	}

	public async void StartWave(int enemyCount, float spawnInterval)
	{
		_waveNumber++;
		GD.Print($"Lancement de la vague {_waveNumber}...");

		for (int i = 0; i < enemyCount; i++)
		{
			SpawnEnemy();
			await Task.Delay((int)(spawnInterval * 1000));
		}
	}

	public override void _Process(double delta)
{
	for (int i = _activeEnemies.Count - 1; i >= 0; i--) // Parcours inversé
	{
		PathFollow2D pathFollow = _activeEnemies[i];

		if (pathFollow != null)
		{
			pathFollow.Progress += (float)(100 * delta); // Avancer l'ennemi

			if (pathFollow.ProgressRatio >= 1.0f) // Vérifier s'il a atteint la fin
			{
				GD.Print("Ennemi arrivé au bout du chemin !");

				_activeEnemies.RemoveAt(i); // Retirer de la liste
				pathFollow.QueueFree(); // Supprimer proprement
			}
		}
	}
}

	private void SpawnEnemy()
	{
		if (EnemyScene == null || EnemyPath == null)
		{
			GD.PrintErr("EnemyScene ou EnemyPath n'est pas défini !");
			return;
		}

		PathFollow2D pathFollow = new PathFollow2D();
		pathFollow.Progress = 0;
		pathFollow.Loop = false;

		Node2D newEnemy = (Node2D)EnemyScene.Instantiate();
		newEnemy.Position = Vector2.Zero;
		AnimatedSprite2D sprite = newEnemy.GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
		if (sprite != null)
		{
			sprite.Play("FrontWalk");
		}

		AnimationPlayer animPlayer = newEnemy.GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
		if (animPlayer != null)
		{
			animPlayer.Play("FrontWalk");
		}
		pathFollow.AddChild(newEnemy);
		EnemyPath.AddChild(pathFollow);
		_activeEnemies.Add(pathFollow);

		GD.Print($"Nouvel ennemi spawné avec animation !");
	}
}
