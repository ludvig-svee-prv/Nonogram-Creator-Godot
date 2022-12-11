using Godot;
using System;

public class SavePuzzlePopup : Panel
{
	private PuzzleDefinition currentPuzzle;
	private FileDialog saveToFile;

	public override void _Ready()
	{
		saveToFile = GetNode<FileDialog>("../SaveToFile");
	}

	public void ShowPopup(PuzzleDefinition definition)
	{
		currentPuzzle = definition;
		this.Visible = true;
	}

	private void HidePopup()
	{
		this.Visible = false;
	}


	private void CopyToClipboard()
	{
		OS.Clipboard = currentPuzzle.JSONString;
		GD.Print(OS.Clipboard);
	}


	private void SaveToFilePressed()
	{
		saveToFile.Visible = true;
	}




	private void FileSelected(String path)
	{
		saveToFile.Visible = false;
		File saveFile = new File();

		string filePath = path;
		if (path.EndsWith("/"))
		{
			filePath += "missing_name.json";
		}
		else if (!path.ToLower().EndsWith(".json") && !path.ToLower().EndsWith(".txt"))
		{
			filePath += ".json";
		}

		saveFile.Open(filePath, File.ModeFlags.WriteRead);

		saveFile.StoreLine(currentPuzzle.JSONString);
		saveFile.Close();

		HidePopup();
	}
}




