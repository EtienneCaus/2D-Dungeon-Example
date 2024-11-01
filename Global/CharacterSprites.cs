using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class CharacterSprites : Node
{
	public AnimatedSprite2D body, chandail, sword, pants, bouclier, casque;
	List<AnimatedSprite2D> spriteElements = new();
    string animation;
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		body = GetNode<AnimatedSprite2D>("Body");
		chandail = GetNode<AnimatedSprite2D>("Chandail");
		sword = GetNode<AnimatedSprite2D>("Epee");
		pants = GetNode<AnimatedSprite2D>("Pants");
		bouclier = GetNode<AnimatedSprite2D>("Bouclier");
		casque = GetNode<AnimatedSprite2D>("Casque");

		spriteElements.Add(body);
		spriteElements.Add(chandail);
		spriteElements.Add(sword);
		spriteElements.Add(pants);
		spriteElements.Add(bouclier);
		spriteElements.Add(casque);
		animation = "IdleE";
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//Pour chaque éléments de la sprite, change l'animation
		spriteElements.ForEach(delegate(AnimatedSprite2D sprt)
		{
			sprt.Play(animation);
		});
	}

	public void ChangeAnimation(String strAnim)
	{
		animation = strAnim;
	}	
}
