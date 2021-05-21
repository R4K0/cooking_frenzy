using CookingFrenzy.FoodThings;
using CookingFrenzy.FoodThings.Entities;
using Sandbox.UI;

namespace CookingFrenzy.UI.FloatingTags
{
	public class FoodTag : FloatingUIPanel<FoodEntity>
	{
		private Label _headerLabel;
		private Panel _ActionContainer;
		public override string Stylesheet => "/UI/FloatingTags/FoodTag.scss";
		public override void BuildPanel()
		{
			DeleteChildren( true );
			
			_headerLabel = AddChild<Label>( "Title" );
			_headerLabel.Text = BelongingEntity.Name;

			_ActionContainer= AddChild<Panel>( "ProcessContainer" );
			foreach ( var (craftType, craftProcess) in BelongingEntity.CraftProcesses )
			{
				_ActionContainer.AddChild<Label>( "ProcessText" )
					.Text = craftType.ToNiceString();
			}
		}
		public override void UpdateFromEnt( FoodEntity ent )
		{
			_headerLabel.Text = BelongingEntity.Name;
			
			_ActionContainer.DeleteChildren( true );
			foreach ( var (craftType, craftProcess) in BelongingEntity.CraftProcesses )
			{
				_ActionContainer.AddChild<Label>( "ProcessText" )
					.Text = craftType.ToNiceString();
			}
		}
	}
}
