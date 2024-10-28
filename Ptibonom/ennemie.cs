using Godot;
using System;
using static Godot.Mathf;

public partial class ennemie : CharacterBody2D
{
	public const float initialSpeed = 60.0f;
	float Speed;
	//AnimatedSprite2D selfSprite;
	CharacterSprites characterSprite;
	Timer timer, swingTimer, chaseTimer;
	byte direction_state = 1;
	int sword_state = 0;
    string action_state = "patrol";
	Node2D chasedBody=null;
	RayCast2D raycast;
	Color rayColor = new Color(1,0,0);
	int selfHP = 5;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Speed = initialSpeed;
		timer = GetNode<Timer>("Timer");
		swingTimer = GetNode<Timer>("SwingTimer");
		chaseTimer = GetNode<Timer>("ChaseTimer");
		raycast = GetNode<RayCast2D>("RayCast2D");
		characterSprite = GetNode<CharacterSprites>("SelfSprite");
		GetNode<ProgressBar>("HealthBar").MaxValue = selfHP;
		GetNode<ProgressBar>("HealthBar").Value = selfHP;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector2 velocity = Velocity;
		Vector2 oldPosition = Position;

		if (action_state == "patrol" || action_state == "chase")	//Handles Patrol
		{
			switch(direction_state)	//Moves according to direction
			{
				case 0 :
					characterSprite.ChangeAnimation("WalkN");
					velocity.X = 0 * Speed;
					velocity.Y = -1 * Speed;
					break;
				case 1 :
					characterSprite.ChangeAnimation("WalkE");
					velocity.X = 1 * Speed;
					velocity.Y = 0 * Speed;
					break;
				case 2 :
					characterSprite.ChangeAnimation("WalkS");
					velocity.X = 0 * Speed;
					velocity.Y = 1 * Speed;
					break;
				case 3 :
					characterSprite.ChangeAnimation("WalkW");
					velocity.X = -1 * Speed;
					velocity.Y = 0 * Speed;
					break;
			}
		}
		else if(action_state == "swing")
		{
			if(sword_state == 0)
			{
				switch(direction_state)		//Swing animation
				{
					case 0 :
						characterSprite.ChangeAnimation("SwingN");
						break;
					case 1 :
						characterSprite.ChangeAnimation("SwingE");
						break;
					case 2 :
						characterSprite.ChangeAnimation("SwingS");
						break;
					case 3 :
						characterSprite.ChangeAnimation("SwingW");
						break;
				}
			}
			else
			{
				switch(direction_state)		//Idle animation
				{
					case 0 :
						characterSprite.ChangeAnimation("IdleN");
						break;
					case 1 :
						characterSprite.ChangeAnimation("IdleE");
						break;
					case 2 :
						characterSprite.ChangeAnimation("IdleS");
						break;
					case 3 :
						characterSprite.ChangeAnimation("IdleW");
						break;
				}
			}

			velocity.X = 0;
			velocity.Y = 0;
		}	
		else if (action_state == "idle")
		{
			switch(direction_state)		//Idle animation
			{
				case 0 :
					characterSprite.ChangeAnimation("IdleN");
					break;
				case 1 :
					characterSprite.ChangeAnimation("IdleE");
					break;
				case 2 :
					characterSprite.ChangeAnimation("IdleS");
					break;
				case 3 :
					characterSprite.ChangeAnimation("IdleW");
					break;
			}
			velocity.X = 0;
			velocity.Y = 0;
		}

		Velocity = velocity; //Moves		
		MoveAndSlide();
		
		if( Speed == initialSpeed &&	//Check if not stunned
			oldPosition.Snapped(1) == Position.Snapped(1) && action_state == "patrol") //If the NPC hit a wall
		{
			//GD.Print(oldPosition.Snapped(1) + " = " + Position.Snapped(1));
			//GD.Print(oldPosition + " | " + Position);
			direction_state ++;		//turn around
			if(direction_state > 3)
				direction_state = 0;
			direction_state ++;

			timer.Start();	//Reset the timer
		}
	}

	public void On_swing_timer_timeout()
	{
		if(action_state == "swing")
		{
			sword_state ++;		//Increment the sword_state animation
			if(sword_state > 2)
			{
				sword_state = 0;	//reset the sword_state animation

				PackedScene swing = ResourceLoader.Load("res://hit.tscn") as PackedScene;	//Create Sword Swing
				hit swingTemp = swing.Instantiate() as hit;
				swingTemp.Position = Vector2.Zero;

				switch(direction_state)		//Swing animation
				{
					case 0 :
						swingTemp.Position = new Vector2(swingTemp.Position.X, swingTemp.Position.Y - 4);	//Moves the sword swing in front of player
						break;
					case 1 :
						swingTemp.Position = new Vector2(swingTemp.Position.X + 4, swingTemp.Position.Y);
						break;
					case 2 :
						swingTemp.Position = new Vector2(swingTemp.Position.X, swingTemp.Position.Y + 4);
						break;
					case 3 :
						swingTemp.Position = new Vector2(swingTemp.Position.X - 4, swingTemp.Position.Y);
						break;
				}
				swingTemp.direction_state = direction_state;
				swingTemp.strGroup = "Enemy";
				AddChild(swingTemp);
			}	
		}
		else
		{
			sword_state = 2;	//reset the swordswing
		} 

		if(Speed < initialSpeed)	//If stunned :
		{
			Speed += 10;
			if(Speed > initialSpeed)
				Speed = initialSpeed;
		}
	}	

	public void On_timer_timeout()
	{
		RandomNumberGenerator rand = new();
		if (action_state == "patrol")	//Change direction if patrolling
		{
			direction_state = (byte)rand.RandiRange(0,3);
		}
	}

	public void On_area_2d_body_entered(Node2D body)
	{
		if(body.IsInGroup("Player"))
		{		//Check to see if the player is positioned in front of the NPC	
			if(body.Position.X > Position.X + 4)
			{
				if (direction_state == 1)
					action_state = "swing";
			}
			else if(body.Position.X < Position.X - 4)
			{
				if(direction_state == 3)
					action_state = "swing";
			}	
			else if(body.Position.Y > Position.Y + 4)
			{
				if(direction_state == 2)
					action_state = "swing";
			}	
			else if(body.Position.Y < Position.Y - 4)
			{
				if(direction_state == 0)
					action_state = "swing";
			}	
		}		
	}
	public void On_area_2d_body_exited(Node2D body)
	{
		if(body.IsInGroup("Player"))
		{
			if(action_state == "swing" || action_state == "chase")
				action_state = "chase";
			else
				action_state = "patrol";
		}	
	}

	public void _on_detection_area_body_entered(Node2D body)	//Start the "Timer" when the player enters
	{
		
		if(body.IsInGroup("Player"))
		{
			chasedBody = body;
			chaseTimer.Start();
		}	
	}

	public void _on_detection_area_body_exited(Node2D body)		//Stops the "Timer" when the player leaves
	{
		
		if(body.IsInGroup("Player"))
		{
			chasedBody = null;
			chaseTimer.Stop();
			action_state = "patrol";
		}	
	}

	public void _on_chase_timer_timeout()
	{
		Vector2 position = Position;
		RandomNumberGenerator rand = new();

		if (action_state == "chase")
		{
			//Si le joueur est proche; donne un coup d'épée
			if(rand.RandiRange(0,1) == 0)
			{
				if(chasedBody.Position.Y > Position.Y - 5 && chasedBody.Position.Y < Position.Y + 5)
				{
					position.Y = chasedBody.Position.Y;
					Position = position;
					if(chasedBody.Position.X > Position.X)
						direction_state = 1;
					else
						direction_state = 3;
				}				
				else
				{
					if(chasedBody.Position.Y < Position.Y)
						direction_state = 0;
					else
						direction_state = 2;
				}
			}		
			else
			{
				if(chasedBody.Position.X > Position.X - 5 && chasedBody.Position.X < Position.X + 5)
				{
					position.X = chasedBody.Position.X;
					Position = position;
					if(chasedBody.Position.Y < Position.Y)
						direction_state = 0;
					else
						direction_state = 2;
				}	
				else
				{
					if(chasedBody.Position.X > Position.X)
						direction_state = 1;
					else
						direction_state = 3;
				}	
			}	
		}
	}		

	public override void _PhysicsProcess(double delta)
	{
		if(chasedBody != null)
		{	
			raycast.TargetPosition = chasedBody.Position - Position; //Take the chased entity position as the raycast target
				//Removes the current position to the target position so it doesn't take the global position (won't work otherwise)
			if(raycast.IsColliding())
			{
				if(raycast.GetCollider() == chasedBody)
				{
					rayColor = new Color(1,1,0);	//turn drawn ray yellow

					switch(direction_state)	//Moves according to direction
					{
						case 0 :
							//if(chasedBody.Position.X) //I'd need a Raycast here :/
							if(RadToDeg(raycast.GetAngleTo(raycast.GetCollisionPoint())) < -70 && RadToDeg(raycast.GetAngleTo(raycast.GetCollisionPoint())) > -120)
							{
								rayColor = new Color(0,1,0);//turn drawn ray green
								if(action_state == "patrol")
									action_state = "chase";	
							}	
							break;
						case 1 :
							if(RadToDeg(raycast.GetAngleTo(raycast.GetCollisionPoint())) > -30 && RadToDeg(raycast.GetAngleTo(raycast.GetCollisionPoint())) < 30)
							{
								rayColor = new Color(0,1,0);
								if(action_state == "patrol")
									action_state = "chase";	
							}
							break;
						case 2 :
							if(RadToDeg(raycast.GetAngleTo(raycast.GetCollisionPoint())) > 70 && RadToDeg(raycast.GetAngleTo(raycast.GetCollisionPoint())) < 120)
							{
								rayColor = new Color(0,1,0);
								if(action_state == "patrol")
									action_state = "chase";
							}
							break;
						case 3 :
							if(RadToDeg(raycast.GetAngleTo(raycast.GetCollisionPoint())) < -150 || RadToDeg(raycast.GetAngleTo(raycast.GetCollisionPoint())) > 150)
							{
								rayColor = new Color(0,1,0);
								if(action_state == "patrol")
									action_state = "chase";
							}
							break;
					}

					if(Vector2.Zero.DistanceTo(chasedBody.Position - Position) < 12 && action_state != "patrol")  //si la distance est courte (et que l'on poursuit)
					{
						action_state = "swing";	//Tape avec l'épée

						if(RadToDeg(raycast.GetAngleTo(raycast.GetCollisionPoint())) < -70 && RadToDeg(raycast.GetAngleTo(raycast.GetCollisionPoint())) > -120)
							direction_state = 0;
						else if(RadToDeg(raycast.GetAngleTo(raycast.GetCollisionPoint())) > -30 && RadToDeg(raycast.GetAngleTo(raycast.GetCollisionPoint())) < 30)
							direction_state = 1;
						else if(RadToDeg(raycast.GetAngleTo(raycast.GetCollisionPoint())) > 70 && RadToDeg(raycast.GetAngleTo(raycast.GetCollisionPoint())) < 120)
							direction_state = 2;
						if(RadToDeg(raycast.GetAngleTo(raycast.GetCollisionPoint())) < -150 || RadToDeg(raycast.GetAngleTo(raycast.GetCollisionPoint())) > 150)
							direction_state = 3;
					}
					else if(action_state == "swing")
						action_state = "chase";
				}	
				else
				{
					action_state = "patrol";
					rayColor = new Color(1,0,0); //turn drawn ray red
				}
				//GD.Print(RadToDeg(raycast.GetAngleTo(raycast.GetCollisionPoint())));
			}
		}
		//QueueRedraw();	
	}

/*    public override void _Draw()
    {
        base._Draw();
		if(chasedBody != null)
			DrawLine(Vector2.Zero, chasedBody.Position - Position, rayColor);
    }*/

	public void GetHit(byte direction, int damage)
	{
		if(	sword_state == 0 || sword_state == 0 || // if swinging or right after
			(direction == 0 && direction_state != 2) ||
			(direction == 1 && direction_state != 3) ||
			(direction == 2 && direction_state != 0) ||
			(direction == 3 && direction_state != 1) )		//If the Enemy isn't blocking...
		{
			selfHP -= damage;
			if(selfHP <= 0)
			{
				QueueFree();
				GetNode<Game>("/root/Game").playerGold += 100;  //Augmente l'argent du joeur

				PackedScene corps = ResourceLoader.Load("res://Ptibonom/Corps.tscn") as PackedScene;	//Crée le corps
				Corps corpsTemp = corps.Instantiate() as Corps;
				corpsTemp.Position = Position - Vector2.Zero;

				corpsTemp.Create(characterSprite.body.Modulate, characterSprite.chandail.Modulate,
					characterSprite.pants.Modulate, characterSprite.casque.Modulate);	//Change la couleur du corps
				GetParent().AddChild(corpsTemp);
			}
		}
		GetNode<ProgressBar>("HealthBar").Visible = true;
		GetNode<ProgressBar>("HealthBar").Value = selfHP;
	}

	public void GetShoved(byte direction, int strength)
	{
		Vector2 velocity = Velocity;	//Bloquer
		Speed = 0;

		if(direction == 0 )
			velocity.Y = -strength;
		if(direction == 1)
			velocity.X = strength;
		if(direction == 2)
			velocity.Y = strength;
		if(direction == 3)
			velocity.X = -strength;

		Velocity = velocity; //Moves
		MoveAndSlide();
		GetNode<ProgressBar>("HealthBar").Visible = true;
		GetNode<ProgressBar>("HealthBar").Value = selfHP;
	}
}
