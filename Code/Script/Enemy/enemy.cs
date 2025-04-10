using System;
using Godot;

public partial class enemy : Node2D
{
	private AnimatedSprite2D animatedSprite;
	private PathFollow2D pathFollow;
	private GoldManager gold;
	private float pathSpeed;
	private int reward;

	public int life;
	public int attack;
	public float speed;

	public Vector2 Velocity { get; private set; }

	[Export]
	public string EnemyType { get; set; }

	public void Initialize(string type, int hp, int atk, float spd, PathFollow2D pathFollowNode, int rwd)
	{
		EnemyType = type;
		life = hp;
		attack = atk;
		speed = spd;
		pathFollow = pathFollowNode;
		pathSpeed = speed / 1000.0f;
		reward = rwd;
	}

	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		gold = GetNodeOrNull<GoldManager>("../UI/Panel/GoldManager");
		
		if (gold == null) {
			GD.PrintErr("gold is null");
			QueueFree();
		}
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
		if (life <= 0) {
			gold.EarnCoins(reward);
			QueueFree();
			}
	}

	public override void _Process(double delta)
	{
		if (pathFollow == null)
			return;

		// ✅ Calculer l'ancienne position avant le déplacement
		Vector2 previousPosition = pathFollow.GlobalPosition;

		// ✅ Avancer sur le chemin
		pathFollow.Progress += (float)(pathSpeed * delta);

		// ✅ Calculer la nouvelle vitesse (différence de position)
		Velocity = (pathFollow.GlobalPosition - previousPosition) / (float)delta;

		// ✅ Vérifier si l'ennemi est arrivé à la fin du chemin
		if (pathFollow.ProgressRatio >= 1.0f)
		{
			QueueFree();
		}
	}
}
