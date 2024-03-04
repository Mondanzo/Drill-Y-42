public abstract class DrillUpgrade : Upgrade {

    public virtual float ModifyMaxGear(float retVal) {
        return retVal;
    }

    public virtual float ModifyBaseSpeed(float retVal) {
        return retVal;
    }

    public virtual float ModifyBaseFuelConsumption(float retVal) {
        return retVal;
    }

    internal virtual bool ChangeDrillRepairStatus(bool retVal) {
        return retVal;
    }

	internal virtual float ModifyMaxFuel(float retVal) {
		return retVal;
	}
}
