using Godot;
using Godot.Collections;
using System;

public partial class Utils : Node
{

	public void Reload()
	{
		GetTree().ChangeSceneToFile("res://main.tscn");
	}
}