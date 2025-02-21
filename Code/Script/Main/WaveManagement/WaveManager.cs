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

		// 🔥 Créer un PathFollow2D unique pour cet ennemi
		PathFollow2D pathFollow = new PathFollow2D();
		pathFollow.Progress = 0;
		pathFollow.Loop = false;

		// 🔥 Instancier l'ennemi
		Node2D newEnemy = (Node2D)EnemyScene.Instantiate();
		enemy enemyInstance = newEnemy as enemy;

		// 🔥 Initialiser l'ennemi avec son PathFollow2D
		enemyInstance.Initialize("Wolf", 100, 10, 50.0f, pathFollow);

		// 🔥 Ajouter l'ennemi dans son propre PathFollow2D
		pathFollow.AddChild(enemyInstance);
		EnemyPath.AddChild(pathFollow);
		_activeEnemies.Add(pathFollow);

		GD.Print("Nouvel ennemi spawné !");
	}
}
