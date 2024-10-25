using Godot;
using System;

public partial class Main : Node2D
{
	const int maxSize = 50;
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

		//Créer une pièce simple, puis deux pièces simples reliées par un couloir
		for(int y = 0; y<20; y++)
		{
			for(int x = 0; x<20; x++)
				wallsArray[x,y] = false;
		}
		
		for(int y = 30; y<50; y++)
		{
			for(int x = 30; x<50; x++)
				wallsArray[x,y] = false;
		}
		//Crée le couloir
		for(int y=10; y<40; y++)
			wallsArray[10,y] = false;
		for(int y=10; y<40; y++)
			wallsArray[19,y] = false;
		for(int x=10; x<40; x++)
			wallsArray[x,40] = false;
		
		//CreateRooms(4); --> Créée 4 pièces reliès par des couloirs
		
		
		//Emboite le tableau de jeu
		for(int y = 0; y<maxSize; y++)
		{
			for(int x = 0; x<maxSize; x++)
			{
				if(x==0  || x==maxSize-1|| y==0 || y==maxSize-1)
					wallsArray[x,y] = true;
			}
		}
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
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
