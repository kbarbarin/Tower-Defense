using System;
using Godot;
using System.Collections.Generic;
using System.Text.Json;
using TowerDefense.Models;

internal partial class main
{
    private void LoadAnimatedObjects(string filePath)
    {
        GD.Print("Chargement des objets animés...");

        // Vérifiez si le fichier existe
        if (!FileAccess.FileExists(filePath))
        {
            GD.PrintErr($"Le fichier {filePath} n'existe pas.");
            return;
        }

        using (FileAccess file = FileAccess.Open(filePath, FileAccess.ModeFlags.Read))
        {
            string jsonContent = file.GetAsText();
            var animatedElements = JsonSerializer.Deserialize<List<AnimatedElementData>>(
                jsonContent
            );

            if (animatedElements == null || animatedElements.Count == 0)
            {
                GD.PrintErr("Aucun élément animé trouvé dans le fichier.");
                return;
            }

            // Référence au nœud parent qui contiendra les AnimatedSprite2D
            var animatedObjectsNode = GetNode<Node2D>("AnimatedObjects");
            if (animatedObjectsNode == null)
            {
                GD.PrintErr("Le nœud 'AnimatedObjects' est introuvable.");
                return;
            }

            // Création des AnimatedSprite2D à partir des données chargées
            foreach (var element in animatedElements)
            {
                var animatedSprite = new AnimatedSprite2D();

                // Vérifiez si la ressource d'animation existe avant de l'assigner
                var spriteFrames = ResourceLoader.Load<SpriteFrames>(element.AnimationType);
                if (spriteFrames == null)
                {
                    GD.PrintErr(
                        $"Impossible de charger la ressource d'animation : {element.AnimationType}"
                    );
                    continue;
                }

                animatedSprite.SpriteFrames = spriteFrames;
                animatedSprite.Position = element.Position.ToVector2();
                animatedSprite.Animation = element.AnimationName;
                animatedSprite.Name = $"{element.ParentFolder}_{element.AnimationName}";

                animatedObjectsNode.AddChild(animatedSprite);
                animatedSprite.Play(element.AnimationName);
                GD.Print($"Ajout de l'AnimatedSprite2D : {animatedSprite.Name}");
            }
        }

        GD.Print("Chargement des objets animés terminé.");
    }
}
