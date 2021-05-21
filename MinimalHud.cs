using CookingFrenzy.FoodThings.Entities;
using CookingFrenzy.UI.FloatingTags;
using Sandbox.UI;

namespace CookingFrenzy
{
	/// <summary>
	/// This is the HUD entity. It creates a RootPanel clientside, which can be accessed
	/// via RootPanel on this entity, or Local.Hud.
	/// </summary>
	public partial class CookingHud : Sandbox.HudEntity<RootPanel>
	{
		public CookingHud()
		{
			if ( !IsClient )
				return;

			RootPanel.SetTemplate( "/MinimalHud.html" );
			
			RootPanel.AddChild<ChatBox>();

			new FloaterCollection<FoodEntity, FoodTag>()
			{
				Parent = RootPanel
			};
		}
	}

}
