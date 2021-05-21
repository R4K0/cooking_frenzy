using System;
using Sandbox;
using Sandbox.ScreenShake;

namespace CookingFrenzy.FoodThings.Entities.Machines
{
	[Library("cuttingboard")]
	public partial class CuttingBoard : MachineEntity, IUse
	{
		private EntityHandle Knife { get; set; }
		private EntityHandle TouchedProp { get; set; }
		private EntityHandle _lastPlayer;
		private AnimEntity KnifeEntity => (AnimEntity)Knife;
		
		[NetPredicted] private TimeSince LastChop { get; set; }

		public CuttingBoard()
		{
			EnableTouch = true;
		}
		public override void Spawn()
		{
			base.Spawn();
			
			var animKnife = new AnimEntity();
				animKnife.SetModel( "content/models/knife_mesh.vmdl" );
				animKnife.Parent = this;
				animKnife.Position = Position;

			Knife = animKnife;
		}

		public override CraftType WorkType => CraftType.Chop;
		protected override string Model => "content/models/cutting_board.vmdl";

		public override bool CanProcess()
		{
			return LastChop.Relative > 0.8f && TouchedProp.IsValid && ((FoodEntity) TouchedProp).CanProcess( WorkType );
		}

		[ClientRpc]
		private void DoShake()
		{
			new Perlin( 1f, 1f, 2f );
		}
		public override bool Process()
		{
			KnifeEntity.SetAnimParam( "Chop", true );
			
			LastChop = 0;
			var touchedProp = (FoodEntity)TouchedProp;
			
			if( touchedProp.DoProcess( WorkType ) )
				touchedProp.Position += Vector3.Up * 7f;

			var particles = Particles.Create( "content/particles/chop.vpcf", touchedProp, "" );

			if ( _lastPlayer.IsValid )
			{
				DoShake( To.Single( _lastPlayer ) );
			}

			return true;
		}

		public bool OnUse( Entity user )
		{
			_lastPlayer = user;
			Process();
			
			return false;
		}

		public bool IsUsable( Entity user )
		{
			return CanProcess();
		}

		public override void StartTouch( Entity other )
		{
			if ( TouchedProp.IsValid || other is not FoodEntity food || LastChop.Relative < 0.8f || !food.CanProcess( WorkType ) )
				return;

			TouchedProp = food;
			
			food.PhysicsBody.AngularDamping = 10f;
			food.PhysicsBody.LinearDamping = 4f;
			
			KnifeEntity?.SetAnimParam( "isReady", true );
		}

		public override void EndTouch( Entity other )
		{
			if ( TouchedProp != other || !TouchedProp.IsValid )
				return;

			((FoodEntity)TouchedProp).PhysicsBody.AngularDamping = 0f;
			((FoodEntity)TouchedProp).PhysicsBody.LinearDamping = 0f;
			
			TouchedProp = null;
			KnifeEntity?.SetAnimParam( "isReady", false );
		}
	}
}
