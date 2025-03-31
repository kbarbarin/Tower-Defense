using System;
using Godot;

public partial class GoldManager : Node2D
{
    private int coin;
    private Label label;

    public override void _Ready()
    {
        coin = 100;
        label = GetNodeOrNull<Label>("Label");
    }

    public override void _Process(double delta) { }

    public bool SpendCoins(int price)
    {
        if (coin >= price)
        {
            coin -= price;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void EarnCoins(int money)
    {
        coin += money;
    }
}
