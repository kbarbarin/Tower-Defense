using System;
using Godot;

public partial class enemy : Node2D
{
	private AnimatedSprite2D animatedSprite;
	private AnimationPlayer animationPlayer;
	private Vector2 targetPosition;

	[Export]
	public string EnemyType { get; set; } // Paramètre exporté pour spécifier le type d'ennemi

	public override void _Ready()
	{
		// Initialisation des variables
		animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animationPlayer = GetNode<AnimationPlayer>("AnimatedSprite2D/AnimationPlayer");

		// Charger les animations selon le type d'ennemi
		SetupEnemy(EnemyType);

		// Définir une position de cible de test (à modifier selon les besoins)
	}

	private void SetupEnemy(string type)
	{
		// Charger la configuration d'animation selon le type d'ennemi
		switch (type)
		{
			case "Wolf":
				if (animatedSprite.SpriteFrames.HasAnimation("FrontWolf"))
				{
					animatedSprite.Play("FrontWolf");
				}
				else
				{
					GD.PrintErr("Animation 'Wolf/WolfForwardWalk' introuvable !");
				}
				GD.Print("Ennemi défini : Wolf");
				break;
			case "Orc":
				//				animatedSprite.Play("walk");
				GD.Print("Ennemi défini : Orc");
				break;
			default:
				GD.PrintErr("Type d'ennemi inconnu : " + type);
				break;
		}
	}

	public override void _Process(double delta)
	{
		// Déplacer l'ennemi vers une position cible avec une vitesse de base
		GD.Print("Mise à jour de l'ennemi à chaque frame");
		Vector2 direction = (targetPosition - Position).Normalized();

		float speed = 100.0f; // Vitesse de déplacement de l'ennemi
		Position += direction * speed * (float)delta;

		// Changer l'animation si l'ennemi est proche de la cible
		if (Position.DistanceTo(targetPosition) < 5.0f)
		{
			animatedSprite.Play("RESET");
		}
	}
}
