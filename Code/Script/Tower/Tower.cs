using System.Collections.Generic;
using Godot;

public partial class Tower : Node2D
{
	private Area2D detectionArea;
	private List<enemy> enemiesInRange = new List<enemy>(); // Liste des ennemis à portée

	[Export]
	public int Damage = 10; // Dégâts de la tour

	[Export]
	public float AttackSpeed = 1.0f; // Attaque par seconde

	public override void _Ready()
	{
		detectionArea = GetNodeOrNull<Area2D>("AnimatedSprite2D/Area2D");
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
			GD.Print("Enemy enter : ");
	}

	private void OnEnemyExit(Node2D body)
	{
		if (body is enemy e)
			enemiesInRange.Remove(e);
		GD.Print("Enemy exit : ");
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
