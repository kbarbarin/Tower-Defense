using System;
using Godot;

public partial class Projectile : Node2D
{
	private Vector2 velocity;
	private enemy target;
	private int damage;

	[Export] public float Speed = 300.0f; // Vitesse du projectile

	public void Launch(Vector2 startPos, enemy targetEnemy, int dmg)
	{
		Position = startPos;
		target = targetEnemy;
		damage = dmg;

		if (target != null)
		{
			// ✅ Calculer la position future de l'ennemi
			Vector2 enemyVelocity = target.Velocity; // 🔥 Ajoute une variable `Velocity` dans `enemy.cs`
			float timeToTarget = Position.DistanceTo(target.GlobalPosition) / Speed;
			Vector2 predictedPosition = target.GlobalPosition + enemyVelocity * timeToTarget;

			// ✅ Ajuster la trajectoire du projectile
			Vector2 direction = (predictedPosition - Position).Normalized();
			velocity = direction * Speed;
		}
	}

	public override void _Process(double delta)
	{
		if (target == null || !IsInstanceValid(target))
		{
			QueueFree(); // ✅ Supprimer le projectile si la cible est morte
			return;
		}

		// ✅ Recalculer la direction en temps réel
		Vector2 direction = (target.GlobalPosition - Position).Normalized();
		velocity = direction * Speed;

		Position += velocity * (float)delta;

		// ✅ Vérifier si le projectile touche l’ennemi
		if (Position.DistanceTo(target.GlobalPosition) < 10f)
		{
			target.TakeDamage(damage);
			QueueFree();
		}
	}
}
