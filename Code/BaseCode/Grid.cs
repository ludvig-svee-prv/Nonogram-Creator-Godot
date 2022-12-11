using Godot;
using System;

public abstract class Grid : Control
{
    protected VariableHolder variableHolder;
	public int CurrentColorIndex { get; protected set; } = 0;
	public bool ColorIsEnabled { get; protected set; } = false;

    public override void _Ready()
    {
		variableHolder = GetNode<VariableHolder>("/root/VariableHolder");
    }

    public abstract void SquareClicked(int x, int y);

    public abstract void SquareClickedErase(int x, int y);

    public abstract void ToggleColor(bool isPressed);

    public void SetColor(int colorIndex)
	{
		CurrentColorIndex = colorIndex;
	}
}
