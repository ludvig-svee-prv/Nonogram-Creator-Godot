using Godot;
using System;

public class CompletedPopup : Panel
{
	private PlayingGrid playGrid;
	private Control viewGrid;

	public override void _Ready()
	{
		playGrid = GetNode<PlayingGrid>("../PlayingGrid");
	}

	public void ShowCompletedPopup()
	{
		viewGrid.Visible = false;
		this.Visible = true;
	}
	
	private void GoToMainMenu()
	{
		GetTree().ChangeScene("res://Scenes/StartScene.tscn");
	}
}
