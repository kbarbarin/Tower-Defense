using Godot;
using System.Collections.Generic;

public partial class tower : Node2D
{
	private Area2D detectionArea;
	private List<enemy> enemiesInRange = new List<enemy>(); // Liste des ennemis à portée

	[Export] public int Damage = 10; // Dégâts de la tour
	[Export] public float AttackSpeed = 1.0f; // Attaque par seconde

	public override void _Ready()
	{
		detectionArea = GetNode<Area2D>("DetectionArea");
		detectionArea.BodyEntered += OnEnemyEnter;
		detectionArea.BodyExited += OnEnemyExit;

	}

	public override void _Process(double delta)
	{
 		// Attaque en boucle
		GetTree().CreateTimer(1.0f / AttackSpeed).Timeout += Attack;
   }

	private void OnEnemyEnter(Node2D body)
	{
		if (body is enemy e)
			enemiesInRange.Add(e);
	}

	private void OnEnemyExit(Node2D body)
	{
		if (body is enemy e)
			enemiesInRange.Remove(e);
	}

	private void Attack()
	{
		if (enemiesInRange.Count > 0)
		{
			enemy target = enemiesInRange[0]; // Attaque le premier ennemi dans la liste
			target.TakeDamage(Damage);
		}

		// Reprogrammer l'attaque
		GetTree().CreateTimer(1.0f / AttackSpeed).Timeout += Attack;
	}
}
