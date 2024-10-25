using Godot;
using System;

public partial class hit : AnimatableBody2D
{
	AnimatedSprite2D selfSprite;
	public byte direction_state = 1;
	public string strGroup ="";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		selfSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		switch(direction_state)		//Animation
		{
			case 0 :
				selfSprite.RotationDegrees = 270;
				break;
			case 1 :
				selfSprite.RotationDegrees = 0;
				break;
			case 2 :
				selfSprite.RotationDegrees = 90;
				break;
			case 3 :
				selfSprite.RotationDegrees = 180;
				break;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector2 position = Position;
		switch(direction_state)		//Animation
		{
			case 0 :
				position.Y -= 1;
				break;
			case 1 :
				position.X += 1;
				break;
			case 2 :
				position.Y += 1;
				break;
			case 3 :
				position.X -= 1;
				break;
		}
		Position = position;
	}

	public void On_animated_sprite_2d_animation_looped()
	{
		QueueFree();
	}	

	public void On_enemy_collision_body_entered(Node2D body)
	{
		if(strGroup == "Player" && body.IsInGroup("Enemy"))
		{
			ennemie enemy = (ennemie)body;
			//body.GetHit();
			enemy.GetHit(direction_state, 1);
			QueueFree();
		}	
		else if(strGroup == "Enemy" && body.IsInGroup("Player"))
		{
			ptibonom player = (ptibonom)body;
			//body.GetHit();
			player.GetHit(direction_state, 1);
			QueueFree();			
		}	
	}
}
