using Godot;

public partial class UIManager : Control
{
	[Export]
	public PackedScene TowerScene; // 📌 Assigne la scène de tour dans l'éditeur
	private Node2D currentTower = null;

	public override void _Ready()
	{
		GetNode<TextureButton>("Panel/VBoxContainer/TowerButton").Pressed += OnTowerButtonPressed;
	}

	private void OnTowerButtonPressed()
	{
		GD.Print("🏗 Tour sélectionnée !");
		currentTower = (Node2D)TowerScene.Instantiate();
		AddChild(currentTower);
	}

	public override void _Process(double delta)
	{
		if (currentTower != null)
		{
			currentTower.GlobalPosition = GetGlobalMousePosition(); // 📌 Suit la souris
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (currentTower != null && @event is InputEventMouseButton mouseEvent)
		{
			if (mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
			{
				// ✅ Vérifier si la zone est valide avant de placer la tour
				if (!IsValidPlacement(currentTower.GlobalPosition))
				{
					GD.PrintErr("❌ Impossible de placer ici !");
					return;
				}

				GD.Print("✅ Tour placée !");
				currentTower = null;
			}
		}
	}

	private bool IsValidPlacement(Vector2 position)
	{
		Area2D pathArea = GetNode<Area2D>("../Path2D/NoBuildZone");

		foreach (var body in pathArea.GetOverlappingBodies())
		{
			if (body is Node2D)
			{
				GD.PrintErr("🚫 Zone interdite détectée !");
				return false; // 🚫 Empêche de placer la tour
			}
		}
		return true; // ✅ Placement autorisé
	}
}
