using Godot;
using System;

public partial class Enemy : Area2D
{
    public int X;
    public int Y;
    [Export] public float Health { get; protected set; } = 1;
    [Export] public float MaxHealth { get; protected set; } = 1;

    public EnemyBox ParentBox;

    //   [Export] public  Type { get; set; }

    public void TakeDamage(float damage)
    {
        Health -= damage;
        Health = Mathf.Clamp(Health, 0, MaxHealth);
        if (Health == 0)
        {
            ParentBox.OnEnemyDied(X, this);
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}