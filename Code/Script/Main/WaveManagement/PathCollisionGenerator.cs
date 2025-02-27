using System.Collections.Generic;
using Godot;

public partial class PathCollisionGenerator : Node
{
	private Path2D Path;

	private CollisionPolygon2D collisionPolygon;
	private float width = 50.0f; // 📏 Largeur du chemin

	public override void _Ready()
	{
		Path = (Path2D)Owner;
		if (Path == null)
		{
			GD.PrintErr("❌ ERREUR : Path2D non assigné !");
			return;
		}

		Area2D noBuildZone = Path.GetNodeOrNull<Area2D>("NoBuildZone");
		if (noBuildZone == null)
		{
			GD.PrintErr("❌ ERREUR : NoBuildZone introuvable !");
			return;
		}

		collisionPolygon = new CollisionPolygon2D();
		noBuildZone.AddChild(collisionPolygon);
		GenerateCollision();
	}

	private void GenerateCollision()
	{
		Curve2D curve = Path.Curve;
		if (curve == null)
		{
			GD.PrintErr("❌ ERREUR : Curve2D est null !");
			return;
		}

		List<Vector2> leftPoints = new List<Vector2>();
		List<Vector2> rightPoints = new List<Vector2>();

		for (int i = 0; i < curve.PointCount; i++)
		{
			Vector2 point = curve.GetPointPosition(i);
			Vector2 normal = curve.GetPointPosition(Mathf.Min(i + 1, curve.PointCount - 1)) - point;
			normal = new Vector2(-normal.Y, normal.X).Normalized() * (width / 2);

			leftPoints.Add(point + normal);
			rightPoints.Insert(0, point - normal);
		}

		leftPoints.AddRange(rightPoints);
		collisionPolygon.Polygon = leftPoints.ToArray();
	}
}
