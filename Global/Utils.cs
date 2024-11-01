using Godot;
using Godot.Collections;
using System;

public partial class Utils : Node
{
	const string savePath = "res://savegame.bin";
	
	public void SaveGame()
	{
		Game game = GetNode<Game>("/root/Game");

		using FileAccess file = FileAccess.Open(savePath, FileAccess.ModeFlags.Write);
		Dictionary<string, int> data = new()
        {
            {"playerHP", game.playerHP},
			{"playerGold", game.playerGold}
		};
        string jstr = Json.Stringify(data);
		file.StoreLine(jstr);
		file.Close();	//unneded when using "using" FileAccess
	}

	public void LoadGame()
	{
		Game game = GetNode<Game>("/root/Game");
		using FileAccess file = FileAccess.Open(savePath, FileAccess.ModeFlags.Read);

		if(FileAccess.FileExists(savePath) && !file.EofReached())
		{
			Dictionary<string, int> currentLine = (Dictionary<string, int>)Json.ParseString(file.GetLine());
			if(currentLine != null){
				game.playerHP = currentLine["playerHP"];
				game.playerGold = currentLine["playerGold"];
			}
		}
	}
}