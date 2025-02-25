using System.Collections.Generic;
using Godot;

public partial class Tower : Node2D
{
	private Area2D detectionArea;
	private List<enemy> enemiesInRange = new List<enemy>(); // Liste des ennemis à portée

	[Export]
	public int Damage = 100; // Dégâts de la tour

	[Export]
	public float AttackSpeed = 1.0f; // Attaque par seconde

	private bool isAttacking = false; // 🔥 Pour éviter de créer plusieurs timers

	public override void _Ready()
	{
		// ✅ Vérifie que `detectionArea` existe bien
		detectionArea = GetNodeOrNull<Area2D>("AnimatedSprite2D/Area2D");

		if (detectionArea == null)
		{
			GD.PrintErr("❌ ERREUR : DetectionArea introuvable !");
			return;
		}

		GD.Print("✅ Area2D trouvé !");

		// ✅ Connecte bien les signaux de détection
		detectionArea.AreaEntered += OnEnemyEnter;
		detectionArea.AreaExited += OnEnemyExit;
	}

	private void OnEnemyEnter(Area2D area)
	{
		// 🔥 Remonter 2 niveaux pour atteindre `enemy`
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
		}
	}

	private void Attack()
	{
		if (enemiesInRange.Count == 0)
		{
			isAttacking = false; // ✅ Arrêter l'attaque si plus d'ennemis
			return;
		}

		enemy target = enemiesInRange[0];

		// 🔥 Vérifie si l'ennemi est encore valide avant d'attaquer
		if (!IsInstanceValid(target))
		{
			GD.PrintErr("❌ L'ennemi ciblé n'existe plus !");
			enemiesInRange.RemoveAt(0); // ✅ Supprime l'ennemi de la liste s'il est déjà supprimé
			return;
		}

		target.TakeDamage(Damage);
		GD.Print($"🔥 Attaque sur {target.Name} pour {Damage} dégâts !");

		GetTree().CreateTimer(1.0f / AttackSpeed).Timeout += Attack;
	}
}
