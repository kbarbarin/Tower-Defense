using System.Collections.Generic;
using Godot;

public partial class Tower : Node2D
{
	private Area2D detectionArea;
	private List<enemy> enemiesInRange = new List<enemy>(); // Liste des ennemis à portée

	[Export]
	public int Damage = 50; // Dégâts de la tour

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
	// 🔥 Vérifier que la liste n'est pas vide
	if (enemiesInRange.Count == 0)
	{
		isAttacking = false; // ✅ Arrêter l'attaque si plus d'ennemis
		GD.Print("🚫 Plus d'ennemis en portée, arrêt de l'attaque.");
		return;
	}

	// 🔥 Supprimer tous les ennemis qui ont été supprimés (`QueueFree()`)
	enemiesInRange.RemoveAll(e => !IsInstanceValid(e));

	// ✅ Vérifier à nouveau après nettoyage
	if (enemiesInRange.Count == 0)
	{
		isAttacking = false;
		GD.Print("🚫 Plus d'ennemis valides, arrêt de l'attaque.");
		return;
	}

	enemy target = enemiesInRange[0];

	target.TakeDamage(Damage);
	GD.Print($"🔥 Attaque sur {target.Name} pour {Damage} dégâts !");

	GetTree().CreateTimer(1.0f / AttackSpeed).Timeout += Attack;
}

}
