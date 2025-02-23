using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

public partial class WaveManager : Node2D
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
		for (int i = _activeEnemies.Count - 1; i >= 0; i--)
		{
			PathFollow2D pathFollow = _activeEnemies[i];

			if (pathFollow != null)
			{
				pathFollow.Progress += (float)(100 * delta);

				if (pathFollow.ProgressRatio >= 1.0f)
				{
					GD.Print("Ennemi arrivé au bout du chemin !");

					_activeEnemies.RemoveAt(i);
					pathFollow.QueueFree();
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
		enemy enemyInstance = newEnemy as enemy;

		enemyInstance.Initialize("Wolf", 100, 10, 50.0f, pathFollow);

		pathFollow.AddChild(enemyInstance);
		EnemyPath.AddChild(pathFollow);
		_activeEnemies.Add(pathFollow);

		GD.Print("Nouvel ennemi spawné !");
	}
}
