using System;
using Godot;

internal partial class main
{
    private void AdjustCameraForMobile()
    {
        if (camera == null)
        {
            GD.PrintErr("La caméra n'est pas initialisée.");
            return;
        }

        float baseWidth = 1920.0f;
        float baseHeight = 1080.0f;

        Vector2 screenSize = GetViewportRect().Size;

        float zoomFactor = Math.Min(screenSize.X / baseWidth, screenSize.Y / baseHeight);
        camera.Zoom = new Vector2(zoomFactor, zoomFactor);

        camera.MakeCurrent();

        GD.Print($"Caméra ajustée pour le mobile avec un facteur de zoom : {zoomFactor}");
    }
}
