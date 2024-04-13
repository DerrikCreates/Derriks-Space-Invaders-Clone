using Godot;
using System;

public partial class Player : CharacterBody2D
{
    [Export] public PackedScene Projectile;

    public const float Speed = 300.0f;


    // Get the gravity from the project settings to be synced with RigidBody nodes.
    public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    private float shootTimer = 0;

    public override void _PhysicsProcess(double delta)
    {
        shootTimer += (float)delta;
        Vector2 velocity = Velocity;

        // Get the input direction and handle the movement/deceleration.
        // As good practice, you should replace UI actions with custom gameplay actions.
        var direction = Input.GetAxis("ui_left", "ui_right");

        if (direction != 0)
        {
            velocity.X = direction * Speed;
        }
        else
        {
            velocity.X = 0; // Mathf.MoveToward(Velocity.X, 0, 1f);
        }

        if (Input.IsKeyPressed(Key.Up))
        {
            if (shootTimer >= 1)
            {
                var instance = Projectile.Instantiate<Projectile>();
                GetWindow().AddChild(instance);
                instance.Position = Position;
                instance.Fire(200);
                shootTimer = 0;
            }
        }

        Velocity = velocity;
        MoveAndSlide();
    }
}