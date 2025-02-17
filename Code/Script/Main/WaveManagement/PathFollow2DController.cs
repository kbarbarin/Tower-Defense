using System;
using Godot;

public partial class PathFollow2DController : PathFollow2D
{
	[Export]
	public float Speed = 100.0f;
	private Node2D enemyNode;
	private AnimatedSprite2D _animatedSprite;
	private float _pathLength = 3623.46f;

	public override void _Ready()
	{
		enemyNode = GetNodeOrNull<Node2D>("enemy");
		if (enemyNode == null)
		{
			GD.PrintErr("enemy n'a pas été trouvé !");
			return;
		}

		_animatedSprite = enemyNode.GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
		if (_animatedSprite == null)
		{
			GD.PrintErr("AnimatedSprite2D n'a pas été trouvé !");
			return;
		}

		if ((bool)_animatedSprite.SpriteFrames?.HasAnimation("FrontWalk"))
			_animatedSprite.Play("FrontWalk");
		else
			GD.PrintErr("L'animation FrontWalk n'existe pas !");
	}

	public override void _Process(double delta)
	{
		if (_pathLength <= 0)
			return;

		Progress += (float)(Speed * delta);

		if (Progress >= _pathLength)
		{
			if (Loop)
				Progress = 0;
			else
				QueueFree();
		}

		if (enemyNode != null)
			enemyNode.GlobalPosition = GlobalPosition;
	}
}
