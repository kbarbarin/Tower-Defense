using Godot;

public partial class UIManager : Control
{
	[Export]
	public PackedScene TowerScene; // 📌 Assigne la scène de la tour dans l'éditeur
	private Tower currentTower = null; // 📌 Stocke la tour en cours de placement
	private bool isPlacing = false; // 📌 Indique si le joueur place une tour

	public override void _Ready()
	{
		// Récupère le bouton et connecte son action
		TextureButton towerButton = GetNodeOrNull<TextureButton>("Panel/VBoxContainer/TowerButton");

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
			if (!IsValidPlacement(currentTower.GlobalPosition)) // 📌 Vérifie la position
			{
				GD.PrintErr("❌ Impossible de placer ici !");
				return;
			}

			GD.Print("✅ Tour placée !");
			currentTower.IsPlaced = true; // ✅ Active la tour après placement
			currentTower.AddToGroup("towers"); // 📌 Ajoute la tour au groupe
			isPlacing = false;
			currentTower = null;
		}
	}

	private bool IsValidPlacement(Vector2 position)
	{
		Area2D pathArea = GetNodeOrNull<Area2D>("../Path2D/NoBuildZone");

		if (pathArea != null)
		{
			foreach (Area2D area in pathArea.GetOverlappingAreas()) // ✅ Vérifie si la tour chevauche un chemin
			{
				if (area is Area2D && area.Position.DistanceSquaredTo(position) < 2500) // 50² pour éviter float
				{
					GD.PrintErr("❌ Impossible de placer ici !");
					return false;
				}
			}
		}

		if (IsOverlappingOtherTowers(position)) // Vérifie si une autre tour est déjà là
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
