using Godot;
using System;

namespace Miniscript.example.access;

public partial class FireableBox : CsgBox3D
{
    [Export] public float fireRate { get; set; } = 0.2f;

    [Export(PropertyHint.Range, "0, 100,")] 
    public float fireRatePower { get; set; } = 100f;

    private ulong lastShotTime = 0;

    private int firesInARow = 0;

    private bool pressed = false;

    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsMouseButtonPressed(MouseButton.Left)) 
        {
            if (!pressed) GD.Print("Starting Fire!");
            pressed = true;
            Fire();
        } else {
            if (pressed)
            {
                pressed = false;
                GD.Print("Number of fires in a row: " + firesInARow);
                firesInARow = 0;
            }
        }
    }

    public bool CanFire()
    {
        if (Time.GetTicksMsec() - lastShotTime - (fireRate * 1000 / (fireRatePower / 100)) >=0 ) return true;

        return false;
    }

    public void Fire()
    {
        if (!CanFire()) return;

        lastShotTime = Time.GetTicksMsec();
        firesInARow++;
    }
}
