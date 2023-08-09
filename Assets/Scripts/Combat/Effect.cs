using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class Effect
{
    public string EffectID;
    public int EffectCount;
    public int EffectBaseCount;
    public int EffectValue_Int = -1;
    public float EffectValue_Float = -1;
    public bool EffectValue_Bool = false;
    public bool EffectIsPassive = false;
    public bool EffectTargetsPlayer;
    public bool EffectTargetsEnemy;
}
