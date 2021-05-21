using System;
using Sandbox;

namespace CookingFrenzy.FoodThings
{
	public static class CraftTypeExtension
	{
		public static string ToNiceString( this CraftType type )
		{
			return type switch
			{
				CraftType.Bake => "Bakeable",
				CraftType.Chop => "Choppable",
				_ => type.ToString()
			};
		}
	}
	
	public enum CraftType
	{
		Chop,
		Bake
	}

	/// <summary>
	/// The CraftProcess class is responsible for storing template data related to processing of that food.
	/// 
	/// It also stores the current processing state.
	/// </summary>
	[Serializable]
	public partial class CraftProcess : NetworkClass, ICloneable
	{
		public CraftProcess() { }

		public CraftProcess( CraftType type, string resultName )
		{
			Type = type;
			ResultName = resultName;
		}
		
		[Net] private CraftType Type { get; set; }
		[Net] public int WorkRequired { get; set; } = 1;
		[Net] public int WorkDone { get; set; }
		[Net] private string ResultName { get; set; }
		public FoodTemplate Result
		{
			get
			{
				return CookingFrenzyGM.Repository?.GetByName( ResultName ) ?? null;
			}
			set
			{
				ResultName = value.Name;
			}
		}
		
		public object Clone()
		{
			return new CraftProcess( Type, ResultName ) {WorkRequired = WorkRequired, WorkDone = 0};
		}
	}
}
