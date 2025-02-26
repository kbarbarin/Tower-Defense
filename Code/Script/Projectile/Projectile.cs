using System;
using Godot;

public partial class Projectile : Node2D
{
	private Vector2 velocity;
	private enemy target;
	private int damage;

	[Export]
	public float Speed = 300.0f; // Vitesse du projectile

	public void Launch(Vector2 startPos, enemy targetEnemy, int dmg)
	{
		Position = startPos;
		target = targetEnemy;
		damage = dmg;

		if (target != null)
		{
			Vector2 direction = (target.GlobalPosition - Position).Normalized();
			velocity = direction * Speed;
		}
	}

	public override void _Process(double delta)
	{
		if (target == null || !IsInstanceValid(target))
		{
			QueueFree(); // ✅ Si la cible n'existe plus, on supprime le projectile
			return;
		}

		Position += velocity * (float)delta;

		if (Position.DistanceTo(target.GlobalPosition) < 10f)
		{
			target.TakeDamage(damage); // ✅ Inflige des dégâts à l'ennemi
			QueueFree();
		}
	}
}
