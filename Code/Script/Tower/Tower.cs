using System.Collections.Generic;
using Godot;

public partial class Tower : Node2D
{
	private Area2D detectionArea;
	private List<enemy> enemiesInRange = new List<enemy>();

	[Export]
	public int Damage = 50;

	[Export]
	public float AttackSpeed = 1.0f;

	[Export]
	public PackedScene ProjectileScene; // 🎯 Assigné depuis l’éditeur

	private bool isAttacking = false;

	private AnimatedSprite2D soldier;

	public override void _Ready()
	{
		detectionArea = GetNodeOrNull<Area2D>("AnimatedSprite2D/Area2D");

		if (detectionArea == null)
		{
			GD.PrintErr("❌ ERREUR : DetectionArea introuvable !");
			return;
		}
		detectionArea.AreaEntered += OnEnemyEnter;
		detectionArea.AreaExited += OnEnemyExit;

		soldier = GetNodeOrNull<AnimatedSprite2D>("Soldier");

		if (soldier == null)
		{
			GD.PrintErr("❌ ERREUR : AnimatedSprite2D introuvable !");
		}
		else
		{
			soldier.Play("Idle");
		}
	}

	public override void _Process(double delta)
	{
		if (enemiesInRange.Count > 0)
		{
			enemy target = enemiesInRange[0];

			if (IsInstanceValid(target))
			{
				string attackAnimation = GetAnimationForDirection(target.GlobalPosition);
				Vector2 direction = (target.GlobalPosition - GlobalPosition).Normalized();

				if (direction.X > 0)
					soldier.Scale = new Vector2(-1, 1); // ✅ Regarde à gauche
				else
					soldier.Scale = new Vector2(1, 1); // ✅ Regarde à droite

				if (soldier != null && soldier.SpriteFrames.HasAnimation(attackAnimation))
				{
					soldier.Play(attackAnimation);
				}
				else
				{
					GD.PrintErr($"❌ Animation '{attackAnimation}' introuvable !");
				}
			}
		}
	}

	private void OnEnemyEnter(Area2D area)
	{
		enemy e = area.GetParent().GetParent() as enemy;

		if (e != null)
		{
			enemiesInRange.Add(e);
			if (!isAttacking)
			{
				isAttacking = true;
				Attack();
			}
		}
		else
		{
			GD.PrintErr($"❌ ERREUR : {area.Name} n'a pas trouvé d'ennemi !");
		}
	}

	private void OnEnemyExit(Node2D body)
	{
		if (body is enemy e)
		{
			enemiesInRange.Remove(e);
			if (enemiesInRange.Count == 0)
			{
				isAttacking = false;
			}
		}
	}

	private string GetAnimationForDirection(Vector2 enemyPosition)
	{
		Vector2 direction = (enemyPosition - GlobalPosition).Normalized();

		if (Mathf.Abs(direction.X) > Mathf.Abs(direction.Y))
		{
			return "sideAttack";
		}
		else
		{
			string anim = direction.Y > 0 ? "frontAttack" : "backAttack";
			return anim;
		}
	}

	private void Attack()
	{
		if (enemiesInRange.Count == 0)
		{
			isAttacking = false;
			return;
		}

		enemiesInRange.RemoveAll(e => !IsInstanceValid(e));

		if (enemiesInRange.Count == 0)
		{
			isAttacking = false;
			return;
		}

		enemy target = enemiesInRange[0];

		if (ProjectileScene != null)
		{
			Projectile projectile = (Projectile)ProjectileScene.Instantiate();
			projectile.Launch(GlobalPosition, target, Damage);
			GetParent().CallDeferred("add_child", projectile);
		}

		GetTree().CreateTimer(1.0f / AttackSpeed).Timeout += Attack;
	}
}
