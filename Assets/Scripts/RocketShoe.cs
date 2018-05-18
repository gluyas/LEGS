public class RocketShoe : Shoe
{
	public float ForceMax;
	public float ForceMin;
	//[NonSerialized] public float fuel;
	
	private void OnValidate()
	{
		Type = ShoeType.Rocket;
	}

}