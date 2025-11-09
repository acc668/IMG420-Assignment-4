using Godot;

public partial class GameManager : Node
{
    [Export]
    public int StartingHealth = 3;

    [Export]
    public Label ScoreLabel;

    [Export]
    public Label HealthLabel;

    private int _score = 0;
    private int _health;

    public override void _Ready()
    {
        _health = StartingHealth;
        UpdateUI();
    }

    public void AddScore(int points)
    {
        _score += points;
        UpdateUI();
    }

    public void TakeDamage(int damage)
    {
        _health -= damage;
        if (_health < 0) _health = 0;
        UpdateUI();

        if (_health <= 0)
        {
            GameOver();
        }
    }

    public void Heal(int amount)
    {
        _health += amount;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (ScoreLabel != null)
        {
            ScoreLabel.Text = $"Score: {_score}";
        }

        if (HealthLabel != null)
        {
            HealthLabel.Text = $"Health: {_health}";
        }
    }

    private void GameOver()
    {
        GD.Print("Game Over! Restarting in 2 seconds...");
        
        // Wait 2 seconds then restart
        GetTree().CreateTimer(2.0).Timeout += () => 
        {
            GetTree().ReloadCurrentScene();
        };
    }
}