using System;
using CookingFrenzy.FoodThings.Entities;
using Sandbox;

namespace CookingFrenzy.Game.ConsoleCommands
{
	public static class ConsoleCommands
	{
		[ServerCmd("spawn_food", Help = "Spawns a food with given identifer")]
		public static void SpawnFood( string foodName )
		{
			if ( ConsoleSystem.Caller?.Pawn is not Player callerPawn || foodName is null )
				return;

			Log.Info( FoodEntity.CreateEyepos( foodName, callerPawn ) is null
				? $"Could not find a food template named {foodName}"
				: $"Spawned a food entity with a template of {foodName}" );
		}
	}
}
