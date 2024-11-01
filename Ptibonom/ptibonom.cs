using Godot;
using System;

public partial class ptibonom : CharacterBody2D
{
	public const float Speed = 60.0f;
	public byte direction_state = 1;
	public byte swinging = 0;
	Timer swingTimer;
	CharacterSprites characterSprite;
	Game playerStats;
	
	public override void _Ready()
	{
		characterSprite = GetNode<CharacterSprites>("SelfSprite");
		swingTimer = GetNode<Timer>("SwingTimer");

		playerStats = GetNode<Game>("/root/Game");
	}

	public override void _Process(double delta)
	{
		Vector2 velocity = Velocity;
		Vector2 direction = Input.GetVector("Left", "Right", "Up", "Down").Snapped(Vector2.One);
		Vector2 lookDirection = direction.Normalized().Snapped(Vector2.One);	//Change la valeur float du Vecteur2 en valeur entière (0,1)
		Vector2 joysticDirection = Input.GetVector("West", "East", "North", "South").Snapped(Vector2.One); //Prend la direction du joystick droit

		if(swinging == 1)
		{	
			//Do nothing if you're swinging (hey, it works!)
		}
		else if (direction != Vector2.Zero)	//Handles Movements
		{
			if( !(joysticDirection != Vector2.Zero &&	//Ignore la prochaine ligne si le vector est à zéro (et donc, que le joystick n'est pas touché)
				joysticDirection.X * joysticDirection.X == joysticDirection.Y * joysticDirection.Y || //Check if joysticDirection's value is something like [1,1] or [-1,-1]
				Input.IsActionPressed("Shift")))	//Check if the Shift key isn't pressed
			{
				if(lookDirection == Vector2.Up)	//Change la direction du joueur
					direction_state = 0;
				if(lookDirection == Vector2.Right)
					direction_state = 1;
				if(lookDirection == Vector2.Down)
					direction_state = 2;
				if(lookDirection == Vector2.Left)
					direction_state = 3;
			}

			if(joysticDirection == Vector2.Up)
				direction_state = 0;
			if(joysticDirection == Vector2.Right)
				direction_state = 1;
			if(joysticDirection == Vector2.Down)
				direction_state = 2;
			if(joysticDirection == Vector2.Left)
				direction_state = 3;

			switch(direction_state)
			{
				case 0 :
					characterSprite.ChangeAnimation("WalkN");					
					break;
				case 1 :
					characterSprite.ChangeAnimation("WalkE");
					break;
				case 2 :
					characterSprite.ChangeAnimation("WalkS");
					break;
				case 3 :
					characterSprite.ChangeAnimation("WalkW");
					break;
			}
			velocity.X = direction.X * Speed;
			velocity.Y = direction.Y * Speed;
		}
		else
		{
			if(joysticDirection == Vector2.Up)
				direction_state = 0;
			if(joysticDirection == Vector2.Right)
				direction_state = 1;
			if(joysticDirection == Vector2.Down)
				direction_state = 2;
			if(joysticDirection == Vector2.Left)
				direction_state = 3;
				
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

		if (Input.IsActionPressed("Swing") && swinging == 0)	//Handles Sword Swing
		{
			PackedScene swing = ResourceLoader.Load("res://hit.tscn") as PackedScene;	//Create Sword Swing
			hit swingTemp = swing.Instantiate() as hit;
			swingTemp.Position = Vector2.Zero;

			switch(direction_state)
			{
				case 0 :
					characterSprite.ChangeAnimation("SwingN");
					swingTemp.Position = new Vector2(swingTemp.Position.X, swingTemp.Position.Y - 4);	//Moves the sword swing in front of player
					break;
				case 1 :
					characterSprite.ChangeAnimation("SwingE");
					swingTemp.Position = new Vector2(swingTemp.Position.X + 4, swingTemp.Position.Y);
					break;
				case 2 :
					characterSprite.ChangeAnimation("SwingS");
					swingTemp.Position = new Vector2(swingTemp.Position.X, swingTemp.Position.Y + 4);
					break;
				case 3 :
					characterSprite.ChangeAnimation("SwingW");
					swingTemp.Position = new Vector2(swingTemp.Position.X - 4, swingTemp.Position.Y);
					break;
			}
			swingTemp.direction_state = direction_state;
			swingTemp.strGroup = "Player";
			AddChild(swingTemp);

			swinging++;
			swingTimer.Start(); //Start the Timer for the swing

			velocity.X = 0;
			velocity.Y = 0;
		}

		if (Input.IsActionPressed("Magic") && swinging == 0)	//Handles Magic spells
		{
			PackedScene swing = ResourceLoader.Load("res://shove.tscn") as PackedScene;	//Creates "Shove" spell
			Shove shoveTemp = swing.Instantiate() as Shove;
			shoveTemp.Position = Vector2.Zero;

			switch(direction_state)
			{
				case 0 :
					characterSprite.ChangeAnimation("SwingN");
					shoveTemp.Position = new Vector2(shoveTemp.Position.X, shoveTemp.Position.Y - 4);	//Moves the sword swing in front of player
					break;
				case 1 :
					characterSprite.ChangeAnimation("SwingE");
					shoveTemp.Position = new Vector2(shoveTemp.Position.X + 4, shoveTemp.Position.Y);
					break;
				case 2 :
					characterSprite.ChangeAnimation("SwingS");
					shoveTemp.Position = new Vector2(shoveTemp.Position.X, shoveTemp.Position.Y + 4);
					break;
				case 3 :
					characterSprite.ChangeAnimation("SwingW");
					shoveTemp.Position = new Vector2(shoveTemp.Position.X - 4, shoveTemp.Position.Y);
					break;
			}
			shoveTemp.direction_state = direction_state;
			shoveTemp.strGroup = "Player";
			AddChild(shoveTemp);

			swinging++;
			swingTimer.Start(); //Start the Timer for the swing

			velocity.X = 0;
			velocity.Y = 0;
		}


		Velocity = velocity;
		MoveAndSlide();	
	}

	public void On_swing_timer_timeout()
	{
		swinging++;
		if(swinging == 2)
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
		else if(swinging > 2)
		{
			swinging = 0;
			swingTimer.Stop();
		}	
	}

	public void GetHit(byte direction, int damage)
	{
		if(	swinging == 1 ||
			(direction == 0 && direction_state != 2) ||
			(direction == 1 && direction_state != 3) ||
			(direction == 2 && direction_state != 0) ||
			(direction == 3 && direction_state != 1) )		//If the Player isn't blocking...
		{
			playerStats.playerHP -= damage;	//Réduit la santé du joueur
			if(playerStats.playerHP <= 0) 
			{
				QueueFree();			//Tue le joueur si la santé arrive à 0

				PackedScene corps = ResourceLoader.Load("res://Ptibonom/Corps.tscn") as PackedScene;	//Crée le corps
				Corps corpsTemp = corps.Instantiate() as Corps;
				corpsTemp.Position = Position - Vector2.Zero;

				Camera2D cameraTemp = GetNode<Camera2D>("Camera2D");	//Détache la caméra et attache-la au corps
				RemoveChild(cameraTemp);
				corpsTemp.AddChild(cameraTemp);

				corpsTemp.Create(characterSprite.body.Modulate, characterSprite.chandail.Modulate,
					characterSprite.pants.Modulate, characterSprite.casque.Modulate);	//Change la couleur du corps
				GetParent().AddChild(corpsTemp);
			}
		}	
	}

	public void GetShoved(byte direction, int strength)
	{
		Vector2 velocity = Velocity;	//Bloquer

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
	}
}
