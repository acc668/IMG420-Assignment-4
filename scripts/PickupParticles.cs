using Godot;

public partial class PickupParticles : GpuParticles2D
{
	public override void _Ready()
	{
		Emitting = true;
		var timer = GetTree().CreateTimer(Lifetime);
		timer.Timeout += QueueFree;
	}
}
