using System;
using System.Collections.Generic;
using System.Text.Json;
using Godot;
using TowerDefense.Models;

internal partial class main
{
    private void ReadMap(string filePath)
    {
        GD.Print("Lecture de la map...");

        var mapArray = new List<CellData>();

        for (int layer = 0; layer < GetLayersCount(); layer++) // Parcours de toutes les couches
        {
            for (int y = 0; y < GetUsedRect().Size.Y; y++)
            {
                for (int x = 0; x < GetUsedRect().Size.X; x++)
                {
                    Vector2I cellPos = new Vector2I(x, y);
                    int cellId = GetCellSourceId(layer, cellPos);

                    if (cellId != -1)
                    {
                        Vector2I atlasCoords = GetCellAtlasCoords(layer, cellPos);

                        mapArray.Add(
                            new CellData
                            {
                                Position = new SerializableVector2I(cellPos),
                                CellId = cellId,
                                AtlasCoords = new SerializableVector2I(atlasCoords),
                                Layer = layer,
                            }
                        );
                    }
                }
            }
        }

        string jsonString = JsonSerializer.Serialize(
            mapArray,
            new JsonSerializerOptions { WriteIndented = true }
        );

        using (FileAccess file = FileAccess.Open(filePath, FileAccess.ModeFlags.Write))
        {
            file.StoreString(jsonString);
        }

        GD.Print($"Données de la carte enregistrées dans {filePath}");
    }
}
