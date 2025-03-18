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
	public PackedScene ProjectileScene;

	public bool IsPlaced { get; set; } = false;

	private bool isAttacking = false;

	private AnimatedSprite2D soldier;
	private AnimatedSprite2D tower;

	public override void _Ready()
	{
		detectionArea = GetNodeOrNull<Area2D>("AnimatedSprite2D/Area2D");
		tower = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
		soldier = GetNodeOrNull<AnimatedSprite2D>("Soldier");

		tower.Play("idle");

		if (detectionArea == null)
		{
			GD.PrintErr("❌ ERREUR : DetectionArea introuvable !");
			return;
		}
		detectionArea.AreaEntered += OnEnemyEnter;
		detectionArea.AreaExited += OnEnemyExit;

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
		if (!IsPlaced)
			return;

		if (enemiesInRange.Count > 0)
		{
			enemy target = enemiesInRange[0];

			if (IsInstanceValid(target))
			{
				string attackAnimation = GetAnimationForDirection(target.GlobalPosition);
				Vector2 direction = (target.GlobalPosition - GlobalPosition).Normalized();

				if (direction.X > 0)
					soldier.Scale = new Vector2(-1, 1);
				else
					soldier.Scale = new Vector2(1, 1);

				if (soldier != null && soldier.SpriteFrames.HasAnimation(attackAnimation))
				{
					soldier.Play(attackAnimation);
				}
				else
				{
					GD.PrintErr($"❌ Animation '{attackAnimation}' introuvable !");
				}
			}
			else
			{
				soldier.Play("Idle");
			}
		}
	}

	private void OnEnemyEnter(Area2D area)
	{
		enemy e = area.GetParent().GetParent() as enemy;

		if (e == null)
			return; // ✅ Ignore les autres objets

		enemiesInRange.Add(e);
		GD.Print($"✅ Enemy {e.Name} détecté !");

		if (!isAttacking && IsPlaced) // ✅ Ne tire pas tant que la tour n'est pas posée
		{
			isAttacking = true;
			Attack();
		}
	}

	private void OnEnemyExit(Node2D body)
	{
		GD.Print($"onEnemyExit {body}");

		if (body is Area2D area)
		{
			Node parent = area.GetParent()?.GetParent(); // 🔥 Récupère le 2ᵉ parent directement

			if (parent is enemy e) // ✅ Vérifie que c'est bien un enemy
			{
				enemiesInRange.Remove(e);
				GD.Print($"❌ Enemy {e.Name} removed!");

				if (enemiesInRange.Count == 0)
				{
					isAttacking = false;
				}
			}
			else
			{
				GD.PrintErr($"❌ ERREUR : {body.Name} n'a pas d'ennemi parent !");
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
		if (!IsPlaced)
			return;
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
