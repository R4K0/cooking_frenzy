using System.Collections.Generic;

namespace CookingFrenzy.FoodThings
{
	public class FoodTemplate
	{
		public Dictionary<CraftType, CraftProcess> CraftProcesses { private set; get; } = new();
		public string Name { set; get; }
		public string Model { set; get; }
		public bool NeedsWashing { set; get; }

		public void AddProcess( CraftType type, CraftProcess process )
		{
			CraftProcesses.Add( type, process );
		}
	}
}
