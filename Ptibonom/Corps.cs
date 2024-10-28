using Godot;
using System;

public partial class Corps : Node2D
{
	public Sprite2D body, chandail, pants, casque;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Create(Color cBody, Color cChandail, Color cPants, Color cCasque)
	{
		body = GetNode<Sprite2D>("Body");
		chandail = GetNode<Sprite2D>("Chandail");
		pants = GetNode<Sprite2D>("Pants");
		casque = GetNode<Sprite2D>("Casque");

		body.Modulate = cBody;
		chandail.Modulate = cChandail;
		pants.Modulate = cPants;
		casque.Modulate = cCasque;
	}
}
