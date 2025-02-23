using System;
using Godot;

public partial class enemy : Node2D
{
	private AnimatedSprite2D animatedSprite;
	private PathFollow2D pathFollow;
	private float pathSpeed;

	public int life;
	public int attack;
	public float speed;

	[Export]
	public string EnemyType { get; set; }

	public void Initialize(string type, int hp, int atk, float spd, PathFollow2D pathFollowNode)
	{
		EnemyType = type;
		life = hp;
		attack = atk;
		speed = spd;
		pathFollow = pathFollowNode;
		pathSpeed = speed / 1000.0f;

		GD.Print($"Ennemi initialisé : {type} - HP: {hp}, ATK: {atk}, SPD: {spd}");
	}

	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		SetupEnemy(EnemyType);
	}

	private void SetupEnemy(string type)
	{
		switch (type)
		{
			case "Wolf":
				if (animatedSprite.SpriteFrames.HasAnimation("FrontWalk"))
				{
					animatedSprite.Play("FrontWalk");
				}
				else
				{
					GD.PrintErr("Animation 'FrontWalk' introuvable !");
				}
				break;
			case "Orc":
				GD.Print("Ennemi défini : Orc");
				break;
			default:
				GD.PrintErr("Type d'ennemi inconnu : " + type);
				break;
		}
	}

	public void TakeDamage(int damage)
	{
		life -= damage;
		if (life <= 0)
			QueueFree();
	}

	public override void _Process(double delta)
	{
		if (pathFollow == null)
			return;

		pathFollow.Progress += (float)(pathSpeed * delta);
		if (pathFollow.ProgressRatio >= 1.0f)
		{
			GD.Print("Ennemi arrivé au bout du chemin !");
			QueueFree();
		}
	}
}
