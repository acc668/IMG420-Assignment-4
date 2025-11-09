using Godot;

public partial class Pickup : Area2D
{
	[Export]
	public PackedScene ParticlesScene;

	[Export]
	public int ScoreValue = 10;

	public override void _Ready()
	{
		Connect("body_entered", new Callable(this, nameof(OnBodyEntered)));
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body is Player)
		{
			if (ParticlesScene != null)
			{
				var particles = ParticlesScene.Instantiate() as Node2D;

				if (particles != null)
				{
					particles.GlobalPosition = GlobalPosition;
					GetParent().AddChild(particles);
				}
			}

			// FIXED: Changed to look in Main scene instead of autoload
			var gameManager = GetNode<GameManager>("/root/Main/GameManager");

			if (gameManager != null)
			{
				gameManager.AddScore(ScoreValue);
			}
			else
			{
				GD.PrintErr("GameManager not found at /root/Main/GameManager");
			}

			QueueFree();
		}
	}
}
