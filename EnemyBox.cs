using Godot;
using System;
using System.Collections.Generic;

public partial class EnemyBox : Area2D
{
    [Export] private CollisionShape2D BoundingBox;

    [Export] private PackedScene _enemyScene;
    [Export] public int EnemyGridWidth = 10;
    [Export] public int EnemyGridHeight = 5;

    [Export] public float RowSpacing = 10;
    [Export] public float ColumnSpacing = 10;
    [Export] private float speed = 150;

    private List<List<Enemy>> _enemyGrid;


    private bool _direction = false;

    public EnemyBox()
    {
        _enemyGrid = new List<List<Enemy>>();

        for (int x = 0; x < EnemyGridWidth; x++)
        {
            _enemyGrid.Add(new List<Enemy>());
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        if (_enemyScene is null)
        {
            throw new NullReferenceException("Enemy scene not set");
        }

        float horizontalOffset = (EnemyGridWidth * ColumnSpacing) / 2f;
        float verticalOffset = (EnemyGridHeight * RowSpacing) / 2f;

        // half spacing needed to make the grid enemies be center to the world. we where half size off being perfect 
        float columHalfSpacing = ColumnSpacing / 2;
        float rowHalfSpacing = RowSpacing / 2;

        for (int x = 0; x < EnemyGridWidth; x++)
        {
            for (int y = 0; y < EnemyGridHeight; y++)
            {
                var enemy = _enemyScene.Instantiate<Enemy>();
                enemy.X = x;
                enemy.Y = y;
                enemy.ParentBox = this;
                _enemyGrid[x].Add(enemy);
                float xPos = x * ColumnSpacing + columHalfSpacing;
                float yPos = y * RowSpacing + rowHalfSpacing;
                enemy.Position = new Vector2(xPos - horizontalOffset, yPos - verticalOffset);

                AddChild(enemy);
            }
        }

        RecalculateBoundingBox();
    }

    public void OnBodyEnter(Node2D other)
    {
        if (other.IsInGroup("Wall"))
        {
            // flip the direction we are moving and move down one level
            _direction = !_direction;

            Translate(Vector2.Down * RowSpacing);
        }
    }

    public void OnEnemyDied(int x, Enemy enemy)
    {
        _enemyGrid[x].Remove(enemy);
        enemy.QueueFree();
        RecalculateBoundingBox();
    }

    // there has to be better ways than looping over all elements but its fine
    public void RecalculateBoundingBox()
    {
        float minX = float.MaxValue;
        float minY = float.MaxValue;

        float maxX = float.MinValue;
        float maxY = float.MinValue;

        float totalX = 0;
        float totalY = 0;

        int currentCount = 0;
        foreach (var column in _enemyGrid)
        {
            foreach (var enemy in column)
            {
                currentCount++;
                totalY += enemy.Position.Y;
                totalX += enemy.Position.X;

                if (enemy.Position.X <= minX)
                {
                    minX = enemy.Position.X;
                }

                if (enemy.Position.Y <= minY)
                {
                    minY = enemy.Position.Y;
                }

                if (enemy.Position.X >= maxX)
                {
                    maxX = enemy.Position.X;
                }

                if (enemy.Position.Y >= maxY)
                {
                    maxY = enemy.Position.Y;
                }
            }
        }

        var xSize = Mathf.Abs(minX - maxX);
        var ySize = Mathf.Abs(minY - maxY);

        RectangleShape2D shape = (RectangleShape2D)BoundingBox.Shape;
        Vector2 min = new Vector2(minX - ColumnSpacing / 2, minY - RowSpacing / 2);
        Vector2 max = new Vector2(maxX + ColumnSpacing / 2, maxY + RowSpacing / 2);
        Rect2 rect = new(min, max - min);
        shape.Size = rect.Size;
        //rect.Position = Position;


        BoundingBox.Position = rect.GetCenter(); //new Vector2(totalX / currentCount, totalY / currentCount);
    }

    public void OnAreaEntered(Area2D other)
    {
        if (other.IsInGroup("End"))
        {
            GD.Print("END");
            speed = 0;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (_direction)
        {
            Translate(Vector2.Left * speed * (float)delta);
        }
        else
        {
            Translate(Vector2.Right * speed * (float)delta);
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}