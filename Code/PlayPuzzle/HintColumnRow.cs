using Godot;
using System;

public class HintColumnRow : Control
{
	public void LoadInHints(ColorHint[] hints)
	{
		for (int i = 0; i < hints.Length; i++)
		{
			ColorHint hint = hints[i];
			if (hint.Amount == 0 && hints.Length > 1)
			{
				continue;
			}
			
			GetNode<HintNumber>("Container/" + i).SetHintNumber(hint.Amount, hint.ColorIndex);
		}
	}
}
