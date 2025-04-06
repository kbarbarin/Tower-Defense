using Godot;

public partial class UIManager : Control
{
	[Export]
	public PackedScene TowerScene; // 📌 Assigne la scène de la tour dans l'éditeur
	private Tower currentTower = null; // 📌 Stocke la tour en cours de placement
	private bool isPlacing = false; // 📌 Indique si le joueur place une tour

	private int price = 100;
	private GoldManager gold;

	public override void _Ready()
	{
		// Récupère le bouton et connecte son action
		TextureButton towerButton = GetNodeOrNull<TextureButton>("Panel/VBoxContainer/TowerButton");
		gold = GetNodeOrNull<GoldManager>("./Panel/GoldManager");

		if (towerButton != null)
		{
			towerButton.Pressed += OnTowerButtonPressed;
			GD.Print("✅ TowerButton connecté !");
		}
		else
		{
			GD.PrintErr("❌ ERREUR : TowerButton introuvable !");
		}
	}

	private void OnTowerButtonPressed()
	{
		if (TowerScene == null)
		{
			GD.PrintErr("❌ ERREUR : TowerScene n'est pas assigné !");
			return;
		}

		GD.Print("🏗 Tour sélectionnée !");
		isPlacing = true; // 📌 Active le mode placement
		currentTower = TowerScene.Instantiate<Tower>(); // 📌 Instancie une nouvelle tour
		AddChild(currentTower); // 📌 Ajoute la tour à l'UI
	}

	public override void _Process(double delta)
	{
		if (isPlacing && currentTower != null)
		{
			currentTower.GlobalPosition = GetGlobalMousePosition(); // 📌 Suit la souris
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (
			isPlacing
			&& @event is InputEventMouseButton mouseEvent
			&& mouseEvent.Pressed
			&& mouseEvent.ButtonIndex == MouseButton.Left
		)
		{
			if (!IsValidPlacement(currentTower.GlobalPosition) || !gold.IsEnoughCoin(price))
			{
				GD.PrintErr("❌ Impossible de placer ici !");
				return;
			}
			gold.SpendCoins(price);
			GD.Print("✅ Tour placée !");
			currentTower.IsPlaced = true;
			currentTower.AddToGroup("towers");
			isPlacing = false;
			currentTower = null;
		}
	}

	private bool IsValidPlacement(Vector2 position)
	{
		Area2D pathArea = GetNodeOrNull<Area2D>("../Path2D/NoBuildZone");

		if (pathArea != null)
		{
			foreach (Node child in pathArea.GetChildren()) // ✅ Vérifie chaque enfant du `NoBuildZone`
			{
				if (child is CollisionShape2D shape && shape.Shape is RectangleShape2D rect)
				{
					Vector2 shapePos = shape.GlobalPosition;
					Vector2 halfExtents = rect.Size / 1.5f;

					// ✅ Ajuster la position pour tester la BASE de la tour (ex : 30px en dessous)
					Vector2 basePosition = position + new Vector2(0, 35); // Ajuste 30 selon la hauteur de la tour

					// ✅ Vérifier si la BASE de la tour est dans le rectangle
					if (
						basePosition.X > shapePos.X - halfExtents.X
						&& basePosition.X < shapePos.X + halfExtents.X
						&& basePosition.Y > shapePos.Y - halfExtents.Y
						&& basePosition.Y < shapePos.Y + halfExtents.Y
					)
					{
						GD.PrintErr("❌ Impossible de placer ici : Zone interdite !");
						return false;
					}
				}
			}
		}

		if (IsOverlappingOtherTowers(position))
		{
			GD.PrintErr("❌ Une autre tour est déjà placée ici !");
			return false;
		}

		return true;
	}

	private bool IsOverlappingOtherTowers(Vector2 position)
	{
		foreach (Node tower in GetTree().GetNodesInGroup("towers"))
		{
			if (
				tower is Node2D towerNode
				&& towerNode.GlobalPosition.DistanceSquaredTo(position) < 2500
			) // ✅ Distance au carré (50²)
			{
				return true;
			}
		}
		return false;
	}
}
