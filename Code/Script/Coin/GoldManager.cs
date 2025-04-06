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
		GetNodeOrNull<AnimatedSprite2D>("Gold").Play("Idle");
	}

	public bool SpendCoins(int price)
	{
		if (coin >= price)
		{
			coin -= price;
			UpdateGoldUI();
			return true;
		}
			return false;
	}

	public void EarnCoins(int money)
	{
		coin += money;
		UpdateGoldUI();
	}

	public bool IsEnoughCoin(int price)
	{
		return coin >= price;
	}

	private void UpdateGoldUI()
	{
		label.Text = $"{coin}";
	}
}
