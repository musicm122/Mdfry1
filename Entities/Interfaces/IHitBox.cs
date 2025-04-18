﻿namespace Mdfry1.Entities.Interfaces;

public interface IHitBox
{
    int Damage { get; set; }
    float EffectForce { get; set; }
    bool IsDebugging { get; set; }

    bool IsDebugPrintEnabled();
    void _Ready();
}