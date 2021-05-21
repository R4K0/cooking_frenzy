using System.Collections.Generic;
using Sandbox;

namespace CookingFrenzy.FoodThings.Entities
{
	public partial class FoodEntity : ModelEntity, IPickupable
	{
		public FoodEntity()
		{
			MoveType = MoveType.Physics;
			CollisionGroup = CollisionGroup.Interactive;
			PhysicsEnabled = true;
			UsePhysicsCollision = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;
		}

		public string Name { get; private set; }
		public bool NeedsWashing { get; set; }
		public Dictionary<CraftType, CraftProcess> CraftProcesses { get; private set; } = new();

		public bool SetTemplate( FoodTemplate template )
		{
			// Make sure we are server
			Host.AssertServer();
			
			Name = (string) template.Name.Clone();
			NeedsWashing = template.NeedsWashing;

			CraftProcesses.Clear();
			foreach ( var (key, value) in template.CraftProcesses )
			{
				CraftProcesses.Add( key, (CraftProcess) value.Clone() );
			}
			
			SetModel( template.Model );

			NetworkDirty( "Name", NetVarGroup.Net );

			return true;
		}

		public bool SetTemplate( string name )
		{
			// Make sure we are server
			Host.AssertServer();
			
			var foodTemplate = CookingFrenzyGM.Repository.GetByName( name );
			if ( foodTemplate is null )
				return false;

			SetTemplate( foodTemplate );
			return true;
		}

		public bool CanProcess( CraftType type )
		{
			return CraftProcesses.ContainsKey( type );
		}

		/// <summary>
		/// Tries to progress a craft process. Increments WorkDone and checks if it is equal or higher to required work,
		/// if it is, then it turns the template of the current object to the result template.
		/// </summary>
		/// <param name="type">The type of craft process you want to progress</param>
		/// <returns>Returns true when the process transformed the template of this object</returns>
		public bool DoProcess( CraftType type )
		{
			CraftProcesses.TryGetValue( type, out var process );

			if ( process is null )
				return false;

			process.WorkDone++;

			if ( process.WorkRequired > process.WorkDone )
				return false;

			SetTemplate( process.Result );
			return true;
		}

		#region Static Helpers
		public static FoodEntity Create( string templateName, Vector3 position, Rotation rotation )
		{
			Host.AssertServer();

			var foodTemplate = CookingFrenzyGM.Repository.GetByName( templateName );
			if ( foodTemplate is null )
				return null;

			var foodEntity = new FoodEntity() {Position = position, Rotation = rotation};
			foodEntity.SetTemplate( foodTemplate );

			return foodEntity;
		}

		public static FoodEntity Create( FoodTemplate template, Vector3 position, Rotation rotation )
		{
			Host.AssertServer();

			var foodEntity = new FoodEntity() {Position = position, Rotation = rotation};
			foodEntity.SetTemplate( template );

			return foodEntity;
		}

		public static FoodEntity CreateEyepos( string templateName, Player player )
		{
			Host.AssertServer();

			var foodTemplate = CookingFrenzyGM.Repository.GetByName( templateName );
			if ( foodTemplate is null )
				return null;
			
			var foodEntity = new FoodEntity()
			{
				Position = player.EyePos + player.EyeRot.Forward * 30, Rotation = player.EyeRot
			};
			foodEntity.SetTemplate( foodTemplate );

			return foodEntity;
		}
		#endregion
		
		#region Networking
		public override bool NetRead( NetRead read )
		{
			Name = read.ReadUtf8();
			NeedsWashing = read.Read<bool>();

			var length = read.Read<int>();
			CraftProcesses.Clear();
			for ( var i = 0; i < length; i++ )
			{
				CraftProcesses[read.Read<CraftType>()] = read.ReadClass<FoodThings.CraftProcess>( null, null );
			}

			return true;
		}

		public override bool NetWrite( NetWrite writer )
		{
			writer.WriteUtf8( Name );
			writer.Write( NeedsWashing );
			
			writer.Write( CraftProcesses.Count );
			foreach ( var (craftType, craftProcess) in CraftProcesses )
			{
				writer.Write( craftType );
				writer.Write( craftProcess );
			}

			return true;
		}
		#endregion

		public EntityHandle HoldingPlayer { get; set; }
	}
}
