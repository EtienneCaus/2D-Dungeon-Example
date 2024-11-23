using Godot;
using System;

public partial class Main : Node2D
{
	const int maxSize = 50;
	const int minRoomSize = 5;
	const int maxRoomSize = 10;
	TileMapLayer tileMapLayer;
	bool[,] wallsArray = new bool[maxSize,maxSize];
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		tileMapLayer = GetNode<TileMapLayer>("Layer0");
		
		//Rempli le jeu de vide
		for(int y = 0; y<maxSize; y++)
		{
			for(int x = 0; x<maxSize; x++)
				wallsArray[x,y] = true;
		}

		CreateRooms(10);

		//Imprimer l'array sur le main
		for(int y = 0; y<maxSize; y++)
		{
			for(int x = 0; x<maxSize; x++)
			{	
				if(wallsArray[x,y])
					tileMapLayer.SetCell(new Vector2I(x,y),0,new Vector2I(0,0));
				else
					tileMapLayer.SetCell(new Vector2I(x,y),0,new Vector2I(1,0));	
			}	
		}

		Utils utils = GetNode<Utils>("/root/Utils"); //Sauve le jeu
		utils.SaveGame();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("Restart"))
		{
			Utils utils = GetNode<Utils>("/root/Utils");
			if(GetNode<Game>("/root/Game").playerHP <= 0) //"Reset" la vie et l'or du joueur s'il est mort
				utils.LoadGame();
			GetTree().ChangeSceneToFile("res://main.tscn");	//Recharge le jeu si on appui sur "Restart" (R)
		}	
	}

	public void CreateRooms(int nbrRooms)
	{
		Random rnd = new();
		int corner1X, corner1Y, corner2X, corner2Y;
		int centerX, centerY, oldCenterX =0, oldCenterY =0;
	
		//Random XY point , random size , empties the room , find the center?
	
		while(nbrRooms > 0)	//Lorsqu'il reste des pièces à faire...
		{
			do{
				corner1X = rnd.Next(1 , maxSize-1);
				corner1Y = rnd.Next(1 , maxSize-1);
			}while( wallsArray[corner1X,corner1Y] == false //Find a spot that wasn't carved out before
			 || corner1X + minRoomSize >= maxSize -1 || corner1Y + minRoomSize >= maxSize -1); //check out of bound

			do{
				corner2X = rnd.Next(corner1X + minRoomSize, corner1X + maxRoomSize);
				corner2Y = rnd.Next(corner1Y + minRoomSize, corner1Y + maxRoomSize);
			}while( wallsArray[corner1X,corner1Y] == false //Find a spot that wasn't carved out before
			 ||	corner2X >= maxSize-1 || corner2Y >= maxSize-1); //Make sure to not go over the edge of the board				

			for(int x = corner1X ; x<corner2X ; x++)	//Crée la pièce
			{
				for(int y = corner1Y ; y<corner2Y ; y++)
				{
					wallsArray[x,y] = false;	//vide la case
				}
			}

			centerX = (corner1X + corner2X)/2;	//Trouve le centre de la pièce
			centerY = (corner1Y + corner2Y)/2;

			if(oldCenterX==0)	//Si c'est la première pièce
			{
				GetNode<ptibonom>("Ptibonom").Position = new Vector2(centerX * 8,centerY * 8);
			}

			if(oldCenterX>0)	//Si ce n'est pas la première pièce
			{
				int nbre = rnd.Next(1,2);	//Met entre 1 et 2 trucs
				while(nbre > 0)		//Fill the room with stuff
				{
					Node2D thingTemp;	//Temporary object
					if(rnd.Next(0,2) != 1)
					{
						PackedScene ennemie = ResourceLoader.Load("res://Ptibonom/ennemie.tscn") as PackedScene;	//Crée un ennemie
						thingTemp = ennemie.Instantiate() as ennemie;
					}
					else
					{
						PackedScene potion = ResourceLoader.Load("res://potion.tscn") as PackedScene;	//Crée ue potion
						thingTemp = potion.Instantiate() as Potion;
					}

					int x = rnd.Next(corner1X+1, corner2X-1);
					int y = rnd.Next(corner1Y+1, corner2Y-1);

					thingTemp.Position = new Vector2(x * 8, y * 8);

					AddChild(thingTemp);
					nbre--;
				}

				if(oldCenterX>centerX)	//Si l'ancienne pièce est à droite
				{
					for(int x = centerX; x<oldCenterX; x++)
						wallsArray[x, centerY] = false;	//Crée la première partie du couloir
				}
				else	//Sinon
				{
					for(int x = centerX; x>oldCenterX; x--)
						wallsArray[x, centerY] = false;
				}

				if(oldCenterY>centerY)	//Si l'ancienne pièce est en bas
				{
					for(int y = centerY; y<oldCenterY; y++)
						wallsArray[oldCenterX, y] = false;	//Crée la deuxième partie du couloir
				}
				else	//Sinon
				{
					for(int y = centerY; y>oldCenterY; y--)
						wallsArray[oldCenterX, y] = false;
				}
			}

			oldCenterX = centerX;	//Indique le centre de la pièce
			oldCenterY = centerY;	//Maintenant l'ancienne pièce

			nbrRooms--;	//Décrémente le nombre de pièces à faire
		}
	}
}
