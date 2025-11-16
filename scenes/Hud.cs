using Godot;
using System;

public partial class Hud : CanvasLayer
{
	[Export]
	private Label scoreLabel;
	[Export]
	private Label messageLabel;
	[Export]
	private Timer messageTimer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
    {
		messageTimer.Autostart = false;
		messageTimer.Timeout += HandleMessageTimerEnd;
    }

    private void HandleMessageTimerEnd()
    {
		messageLabel.Text = string.Empty;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}

	public void SetScore(uint score)
	{
		scoreLabel.Text = score.ToString("D10");
	}
	
	public void SetMessage(string message,int timeSeconds)
    {
		messageLabel.Text = message;
		messageTimer.WaitTime = timeSeconds;
		messageTimer.Start();
    }
}
