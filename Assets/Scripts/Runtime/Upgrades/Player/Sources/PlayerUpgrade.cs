using System;
using UnityEngine;

public abstract class PlayerUpgrade : Upgrade {

    public virtual float ModifyMiningSpeed(float retVal) {
        return retVal;
    }

    public virtual float ModifyOreMultiplier(float retVal) {
        return retVal;
    }

    public virtual float ModifySpeed(float retVal) {
        return retVal;
    }

    public virtual bool UnlockCollectorBeam(bool retVal) {
        return retVal;
    }

    public virtual bool UnlockHammerTool(bool retVal) {
        return retVal;
    }

    internal float ModifyMaxHealth(float retVal) {
        return retVal;
    }
}
