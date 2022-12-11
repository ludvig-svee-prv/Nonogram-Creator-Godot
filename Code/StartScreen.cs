using Godot;
using System;

public class StartScreen : Panel
{
	bool loadingPuzzleForPlay = false;

	Panel loadFilePanel;
	Panel loadTextPanel;

	LineEdit loadTextLine;

	FileDialog loadFileDialog;
	AcceptDialog loadPuzzleError;

	VariableHolder variableHolder;

	public override void _Ready()
	{
		loadFilePanel = GetNode<Panel>("LoadFile");
		loadTextPanel = GetNode<Panel>("LoadText");

		loadTextLine = loadTextPanel.GetNode<LineEdit>("Panel/Line");

		loadFileDialog = loadFilePanel.GetNode<FileDialog>("FileDialog");
		loadPuzzleError = loadFilePanel.GetNode<AcceptDialog>("LoadFileError");

		variableHolder = GetNode<VariableHolder>("/root/VariableHolder");
	}

	private void ShowEditor()
	{
		GetTree().ChangeScene("res://Scenes/Create.tscn");
	}

	private void ShowPlayScene()
	{
		GetTree().ChangeScene("res://Scenes/Play.tscn");
	}



	private void ShowLoadFromFile(bool isForPlay)
	{
		loadingPuzzleForPlay = isForPlay;
		loadFileDialog.Visible = true;
		loadFilePanel.Visible = true;
	}


	private void ShowLoadFromText(bool isForPlay)
	{
		loadingPuzzleForPlay = isForPlay;
		loadTextLine.Text = "";
		loadTextPanel.Visible = true;
	}

	private void LoadInText()
	{
		string jsonString = loadTextLine.Text;
		try
		{
			PuzzleDefinition puzzle = new PuzzleDefinition(jsonString);
			variableHolder.CurrentPuzzleDefinition = puzzle;

			if (loadingPuzzleForPlay)
			{
				ShowPlayScene();
			}
			else
			{
				ShowEditor();
			}
		}
		catch (Exception e)
		{
			ShowLoadPuzzleErrorPopup(e.Message, false);
		}


	}

	private void JsonFileSelected(String path)
	{
		loadFileDialog.Visible = false;
		File jsonFile = new File();
		Error loadFileStatus = jsonFile.Open(path, File.ModeFlags.Read);
		if (loadFileStatus == Error.Ok)
		{
			try
			{
				PuzzleDefinition puzzle = new PuzzleDefinition(jsonFile.GetAsText());
				variableHolder.CurrentPuzzleDefinition = puzzle;

				if (loadingPuzzleForPlay)
				{
					ShowPlayScene();
				}
				else
				{
					ShowEditor();
				}
			}
			catch (Exception e)
			{
				ShowLoadPuzzleErrorPopup(e.Message, false);
			}
		}
		else
		{
			ShowLoadPuzzleErrorPopup("Open file status - " + loadFileStatus.ToString(), true);
		}
	}

	private void ShowLoadPuzzleErrorPopup(string error, bool loadingFromFile)
	{
		GD.PrintErr(error);

		loadFilePanel.Visible = true;
		if (loadingFromFile)
		{
			loadPuzzleError.DialogText = "Failed to open file. Ensure that the file is a correct Nonogram JSON file. Error: " + error;
		}
		else
		{
			loadPuzzleError.DialogText = "Failed to parse string. Ensure that the string is a correct Nonogram JSON format. Error: " + error;
		}
		loadPuzzleError.PopupCentered();
	}

	private void CloseLoadFile()
	{
		if (loadFilePanel != null) // Needed since emit is based on canvas hide and is auto triggered during startup.
		{
			loadFilePanel.Visible = false;
		}
	}

	private void CloseLoadText()
	{
		loadTextPanel.Visible = false;
	}


	private void QuitApplication()
	{
		GetTree().Quit();
	}
}

