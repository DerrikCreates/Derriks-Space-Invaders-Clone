using Godot;
using System;

public partial class Projectile : Area2D
{
    // Called when the node enters the scene tree for the first time.

    private float speed = 0;

    public void Fire(float speed)
    {
        this.speed = speed;
    }

    public void OnAreaEntered(Area2D other)
    {
        if (other.IsInGroup("Enemy"))
        {
            var enemy = other as Enemy;

            enemy?.TakeDamage(1);
        }
    }

    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public override void _PhysicsProcess(double delta)
    {
        Translate(Vector2.Up * speed * (float)delta);
    }
}