using Godot;
using System;
using System.Collections.Generic;

public class ColorEnabler : GridContainer
{
	private Color disabledColor = new Color("#33494949");
	private Color enabledColor = new Color(1, 1, 1);

	private static Color[] ColorArray = new Color[11]
	{
		 new Color(1,1,1),
		 new Color("#0d0d0d"),
		 new Color("#6ff64e"),
		 new Color("#1d2af5"),
		 new Color("#ff4ddf"),
		 new Color("#9c09ee"),
		 new Color("#fff732"),
		 new Color("#ff8300"),
		 new Color("#6dffea"),
		 new Color("#ff1f1f"),
		 new Color("#8a8a8a")
	};

	public bool ColorIsEnabled
	{
		get { return grid.ColorIsEnabled; }
	}

	ColorSelect[] colorSquares;

	private DesignGrid grid;

	public override void _Ready()
	{
		grid = GetNode<DesignGrid>("../CreateGrid");

		int childCount = GetChildCount();
		colorSquares = new ColorSelect[childCount];
		for (int i = 0; i < childCount; i++)
		{
			colorSquares[i] = GetChild<ColorSelect>(i);
		}
		ToggleColor(false);
	}


	private void ToggleColor(bool button_pressed)
	{
		grid.ToggleColor(button_pressed);
		Modulate = button_pressed ? enabledColor : disabledColor;
	}

	public void ChangeColor(int colorIndex)
	{
		foreach (ColorSelect cs in colorSquares)
		{
			cs.DeselectColor();
		}

		grid.SetColor(colorIndex);
	}

	public static Color ColorIndexLookup(int index)
	{
		if (index < 0 || index >= ColorArray.Length)
		{
			return ColorArray[0];
		}

		return ColorArray[index];
	}
}

