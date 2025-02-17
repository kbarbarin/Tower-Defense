using System;
using Godot;

internal partial class main
{
    private void CenterCamera()
    {
        if (TileSet != null)
        {
            Vector2I cellSize = TileSet.TileSize;
            Vector2 mapSize = GetUsedRect().Size * cellSize;
            Vector2 centerPosition = GetUsedRect().Position * cellSize + mapSize / 2;

            if (camera != null)
            {
                camera.Position = new Vector2I((int)centerPosition.X, (int)centerPosition.Y);
                GD.Print($"Caméra centrée sur la position : {camera.Position}");
            }
            else
            {
                GD.PrintErr("Caméra introuvable !");
            }
        }
        else
        {
            GD.PrintErr("TileSet introuvable ou non assigné.");
        }
    }
}
