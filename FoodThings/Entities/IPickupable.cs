using Sandbox;

namespace CookingFrenzy.FoodThings.Entities
{
	public interface IPickupable
	{
		EntityHandle HoldingPlayer
		{
			get;
			set;
		}
	}
}
