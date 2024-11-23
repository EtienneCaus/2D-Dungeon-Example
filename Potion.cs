using Godot;
using System;

public partial class Potion : Node2D
{
    public void _on_area_2d_body_entered(Node2D body)
	{
		if (body.IsInGroup("Player") && //If the body is the player and the health isn't maxed
                GetNode<Game>("/root/Game").playerHP < GetNode<Game>("/root/Game").playerMaxHP)
        {
			GetNode<Game>("/root/Game").playerHP += 1;
			Tween tween = GetTree().CreateTween();
			Tween tween1 = GetTree().CreateTween();
			tween.TweenProperty(this, "position", new Vector2(Position.X,Position.Y-35), 0.35f);
			tween1.TweenProperty(this, "modulate:a", 0, 0.3f);
			tween.TweenCallback(Callable.From(QueueFree));
		}
	}
}
