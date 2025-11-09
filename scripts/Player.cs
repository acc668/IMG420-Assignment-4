using Godot;

public partial class Player : CharacterBody2D
{
	[Export]
	public float Speed = 200f;

	[Export]
	public float JumpVelocity = -300f;

	[Export]
	public float Gravity = 800f;

	private AnimatedSprite2D _anim;

	public override void _Ready()
	{
		_anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 v = Velocity;

		float inputX = Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left");
		v.X = inputX * Speed;

		v.Y += Gravity * (float)delta;

		if (IsOnFloor() && Input.IsActionJustPressed("ui_accept"))
		{
			v.Y = JumpVelocity;
		}

		Velocity = v;
		MoveAndSlide();

		if (_anim != null)
		{
			if (Mathf.Abs(inputX) > 0.01f)
			{
				_anim.FlipH = inputX > 0;  // FIXED: Changed < to >
				_anim.Play("walk");
			}
			else
			{
				_anim.Play("idle");
			}
		}
	}
}
