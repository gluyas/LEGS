public class RocketShoe : Shoe
{
	public float ForceMax;
	public float ForceMin;
	
	private void OnValidate()
	{
		Type = ShoeType.Rocket;
	}

}