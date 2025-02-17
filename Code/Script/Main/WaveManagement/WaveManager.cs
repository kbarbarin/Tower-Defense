using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

public partial class WaveManager : Node
{
	[Export]
	public PackedScene EnemyScene { get; set; }

	[Export]
	public Path2D EnemyPath { get; set; } // Assigne le Path2D depuis l’éditeur
	private List<PathFollow2D> _activeEnemies = new List<PathFollow2D>(); // Stocke PathFollow2D

	private int _waveNumber = 0;

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
			await Task.Delay((int)(spawnInterval * 1000)); // Attendre avant le prochain spawn
		}
	}

	public override void _Process(double delta)
	{
		for (int i = _activeEnemies.Count - 1; i >= 0; i--) // Parcours inversé pour suppression
		{
			PathFollow2D pathFollow = _activeEnemies[i]; // On a directement un PathFollow2D

			if (pathFollow != null)
			{
				pathFollow.Progress += (float)(100 * delta); // Avancer l'ennemi

				// Si l'ennemi arrive à la fin du chemin
				if (pathFollow.ProgressRatio >= 1.0f)
				{
					GD.Print("Ennemi arrivé au bout du chemin !");

					// Supprime l'ennemi et son PathFollow2D
					_activeEnemies.RemoveAt(i); // Retirer de la liste
					pathFollow.QueueFree(); // Supprimer l'ennemi et son parent directement
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

		// Créer un PathFollow2D et l'attacher au Path2D existant
		PathFollow2D pathFollow = new PathFollow2D();
		pathFollow.Progress = 0; // Commence au début du chemin

		// Instancie un nouvel ennemi
		Node2D newEnemy = (Node2D)EnemyScene.Instantiate();
		newEnemy.Position = Vector2.Zero; // Laisse PathFollow2D gérer sa position

		// ✅ Lancer l'animation de l'ennemi s'il possède AnimatedSprite2D
		AnimatedSprite2D sprite = newEnemy.GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
		if (sprite != null)
		{
			sprite.Play("FrontWalk"); // Remplace "walk" par le nom de ton animation de déplacement
		}

		// ✅ Si l'ennemi utilise AnimationPlayer
		AnimationPlayer animPlayer = newEnemy.GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
		if (animPlayer != null)
		{
			animPlayer.Play("FrontWalk"); // Remplace "walk" par le nom de ton animation
		}

		// Ajoute l'ennemi dans le PathFollow2D
		pathFollow.AddChild(newEnemy);

		// Ajoute le PathFollow2D dans le Path2D
		EnemyPath.AddChild(pathFollow);

		// Ajoute à la liste des ennemis actifs
		_activeEnemies.Add(pathFollow);

		GD.Print($"Nouvel ennemi spawné avec animation !");
	}
}
