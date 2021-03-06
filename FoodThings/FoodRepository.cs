using System.Collections.Generic;
using Sandbox;

namespace CookingFrenzy.FoodThings
{
	public partial class FoodRepository
	{
		private readonly Dictionary<string, FoodTemplate> _templates = new();
		public FoodTemplate GetByName( string name )
		{
			_templates.TryGetValue( name, out var foundTemplate );

			return foundTemplate;
		}

		public FoodTemplate AddDefinition( string name, string model )
		{
			_templates.Remove( name );

			var foodTemplate = new FoodTemplate() {Name = name, Model = model};

			_templates.Add( name, foodTemplate );

			return foodTemplate;
		}
		
		public FoodRepository()
		{
			RegisterDefs();
		}
		
		private void RegisterDefs()
		{
			var apple = AddDefinition( "Apple", "content/models/applered01.vmdl" );
			AddDefinition( "Cut Apple", "content/models/applered02.vmdl" );

			apple.AddProcess( CraftType.Chop, new CraftProcess( CraftType.Chop, "Cut Apple" )
			{
				WorkRequired = 5
			} );
		}
	}
}
