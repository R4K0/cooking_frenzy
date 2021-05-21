using CookingFrenzy.FoodThings.Entities;
using Sandbox;
using Sandbox.Rcon;

namespace CookingFrenzy.Game
{
	public static class PlayerCarryExtension
	{
		public static bool IsCarrying( this CookingPlayer player )
		{
			return player.CarryController?.CarriedProp.IsValid ?? false;
		}
	}
	
	public partial class CarryController : NetworkClass
	{
		[NetPredicted] private TimeSince LastPickup { get; set; }
		[Net] public EntityHandle CarriedProp { get; set; }
		
		private bool CanPickup( IPickupable pickupable )
		{
			Log.Info( "Can Pickup called" );
			
			return LastPickup.Relative >= 1 && !pickupable.HoldingPlayer.IsValid && !CarriedProp.IsValid;
		}

		private void Drop()
		{
			var pickupableAsModel = (ModelEntity) CarriedProp;
			var pickupable = (IPickupable) pickupableAsModel;

			pickupableAsModel.PhysicsBody.AngularDamping = 0f;
			pickupableAsModel.PhysicsBody.LinearDamping = 0f;
			
			pickupable.HoldingPlayer = null;
			CarriedProp = null;
		}

		private void DoPickup( IPickupable pickupable, CookingPlayer pawn )
		{
			pickupable.HoldingPlayer = pawn;
			CarriedProp = pickupable as ModelEntity;
		}

		public void PhysicsManipulate( CookingPlayer pawn )
		{
			if(!CarriedProp.IsValid)
				return;

			using ( Prediction.Off() )
			{
				var modelEntity = (ModelEntity) CarriedProp;
				if ( modelEntity.PhysicsBody is null )
				{
					return;
				}
				var desiredPosition = pawn.EyePos + pawn.EyeRot.Forward * 40;
				var force = (desiredPosition - modelEntity.Position) * (modelEntity.PhysicsBody.Mass * 1f) * 175f;

				modelEntity.PhysicsBody.LinearDamping = 10f;
				modelEntity.PhysicsBody.AngularDamping = 7f;
				modelEntity.PhysicsBody.ApplyForce( force );
			}
		}

		public void DoThrow( CookingPlayer pawn )
		{
			var carriedProp = (ModelEntity)CarriedProp;
			Drop();
			
			carriedProp.PhysicsBody.ApplyForce( pawn.EyeRot.Forward * 25000f );
		}

		public void Simulate( CookingPlayer pawn )
		{
			switch (CarriedProp.IsValid)
			{
				case true when pawn.Input.Pressed( InputButton.Use ) || pawn.LifeState == LifeState.Dead:
					Drop();
					return;
				case true when pawn.Input.Pressed( InputButton.Attack1 ):
					DoThrow( pawn );
					return;
			}

			if (LastPickup.Relative >= 1 && !CarriedProp.IsValid && pawn.Input.Pressed( InputButton.Use ) )
			{
				var traceResult = Trace.Ray( pawn.EyePos, pawn.EyePos + pawn.EyeRot.Forward * 85 )
					.Radius( 4 )
					.Ignore( pawn )
					.Run();
				
				DebugOverlay.Text( traceResult.EndPos, $"Hit: {traceResult.Hit}, Entity: {(traceResult.Hit ? traceResult.Entity : "N/A")}", 5f );

				if ( traceResult.Hit && traceResult.Entity is IPickupable entity && CanPickup( entity ) )
				{
					DebugOverlay.Text( traceResult.EndPos - Vector3.Down * 5, "Picking up!", 5f );
					DoPickup( entity, pawn );
				}
			}
			
			//Log.Info( $"Testing: {CarriedProp.IsSet}" );
			
			PhysicsManipulate( pawn );
		}
	}
}
