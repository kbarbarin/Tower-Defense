using System;
using System.Collections.Generic;
using System.Text.Json;
using Godot;
using TowerDefense.Models;

internal partial class main : TileMap
{
	private Camera2D camera;

	public override void _Ready()
	{
		GD.Print("Initialisation du projet...");
		camera = GetNode<Camera2D>("Camera2D");

		// Ce code sert à générer les fichiers json pour la map
		// ReadMap("res://assets/map/map_data.json");
		// SaveAnimatedObjects("res://assets/map/animated_objects_data.json");

		// Ce code sert à lire les json et de créer la map
		// GenerateMap("res://assets/map/map_data.json");
		// LoadAnimatedObjects("res://assets/map/animated_objects_data.json");


		CenterCamera();
		AdjustCameraForMobile();
//		GenerateEnemy();
	}
}
