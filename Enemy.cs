using Godot;

public partial class Enemy : CharacterBody2D
{
	[Export]
	public float Speed = 120f;

	[Export]
	public NodePath TargetPath;

	[Export]
	public int Damage = 1;

	[Export]
	public float DamageCooldown = 1.0f;

	[Export]
	public PackedScene HitParticlesScene;

	private NavigationAgent2D _navAgent;
	private Node2D _target;
	private float _damageTimer = 0f;
	private Node2D _sprite;  // Changed to Node2D to work with any sprite type

	public override void _Ready()
	{
		_navAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
		
		// Try to find sprite - works with Sprite2D, AnimatedSprite2D, or any visual node
		_sprite = GetNodeOrNull<Sprite2D>("Sprite2D");
		if (_sprite == null)
		{
			_sprite = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
		}
		if (_sprite == null)
		{
			// Try to find any child that's a visual node
			foreach (Node child in GetChildren())
			{
				if (child is Sprite2D || child is AnimatedSprite2D)
				{
					_sprite = (Node2D)child;
					GD.Print($"Found sprite node: {child.Name}");
					break;
				}
			}
		}
		
		if (_sprite == null)
		{
			GD.PrintErr("WARNING: No sprite found on Enemy! Add a Sprite2D or AnimatedSprite2D child node.");
		}

		if (TargetPath != null)
		{
			_target = GetNode<Node2D>(TargetPath);
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_target == null || _navAgent == null)
			return;

		_navAgent.TargetPosition = _target.GlobalPosition;
		Vector2 nextPoint = _navAgent.GetNextPathPosition();
		Vector2 direction = (nextPoint - GlobalPosition).Normalized();
		Velocity = direction * Speed;
		MoveAndSlide();

		// Flip sprite based on movement direction
		if (_sprite != null && Mathf.Abs(direction.X) > 0.01f)
		{
			// Works for both Sprite2D and AnimatedSprite2D
			if (_sprite is Sprite2D sprite2D)
			{
				sprite2D.FlipH = direction.X < 0;
			}
			else if (_sprite is AnimatedSprite2D animSprite)
			{
				animSprite.FlipH = direction.X < 0;
			}
		}

		CheckPlayerCollision(delta);
	}

	private void CheckPlayerCollision(double delta)
	{
		if (_damageTimer > 0)
		{
			_damageTimer -= (float)delta;
		}

		for (int i = 0; i < GetSlideCollisionCount(); i++)
		{
			var collision = GetSlideCollision(i);
			var collider = collision.GetCollider();

			if (collider is Player && _damageTimer <= 0)
			{
				GD.Print("Enemy hit player!"); // DEBUG
				
				if (HitParticlesScene != null)
				{
					var particles = HitParticlesScene.Instantiate() as Node2D;
					if (particles != null)
					{
						particles.GlobalPosition = GlobalPosition;
						GetParent().AddChild(particles);
					}
				}

				var gameManager = GetNode<GameManager>("/root/Main/GameManager");
				
				if (gameManager != null)
				{
					GD.Print($"Dealing {Damage} damage to player"); // DEBUG
					gameManager.TakeDamage(Damage);
				}
				else
				{
					GD.Print("ERROR: GameManager NOT found at /root/Main/GameManager"); // DEBUG
				}

				_damageTimer = DamageCooldown;
			}
		}
	}
}
