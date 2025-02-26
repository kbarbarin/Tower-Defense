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

	private void OnEnemyEnter(Area2D area)
	{
		enemy e = area.GetParent().GetParent() as enemy;

		if (e != null)
		{
			enemiesInRange.Add(e);
			GD.Print($"✅ Enemy {e.Name} détecté !");

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
			GD.Print($"❌ Enemy {e.Name} est sorti !");

			// ✅ Vérifier si la tour doit encore attaquer
			if (enemiesInRange.Count == 0)
			{
				isAttacking = false;
				GD.Print("🛑 Plus d'ennemis en vue, arrêt de l'attaque.");
			}
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

		if (soldier != null)
		{
			soldier.Play("sideAttack");
		}

		enemy target = enemiesInRange[0];

		// ✅ Créer et tirer un projectile
		if (ProjectileScene != null)
		{
			Projectile projectile = (Projectile)ProjectileScene.Instantiate();
			projectile.Launch(GlobalPosition, target, Damage);

			// 🔥 Utilisation de `CallDeferred()` pour éviter l'erreur de mise à jour physique
			GetParent().CallDeferred("add_child", projectile);
		}

		// target.TakeDamage(Damage);
		GetTree().CreateTimer(1.0f / AttackSpeed).Timeout += Attack;
	}
}
