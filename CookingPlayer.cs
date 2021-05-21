using Sandbox;
using System;
using System.Linq;
using CookingFrenzy.Game;

namespace CookingFrenzy
{
	public partial class CookingPlayer : Player
	{
		[NetPredicted] public CarryController CarryController { get; set; }
		public override void Respawn()
		{
			SetModel( "models/citizen/citizen.vmdl" );

			//
			// Use WalkController for movement (you can make your own PlayerController for 100% control)
			//
			Controller = new WalkController();

			//
			// Use StandardPlayerAnimator  (you can make your own PlayerAnimator for 100% control)
			//
			Animator = new StandardPlayerAnimator();

			//
			// Use ThirdPersonCamera (you can make your own Camera for 100% control)
			//
			Camera = new FirstPersonCamera();

			// This is what handles carrying shit
			CarryController = new CarryController();

			EnableAllCollisions = true;
			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = false;

			base.Respawn();
		}

		/// <summary>
		/// Called every tick, clientside and serverside.
		/// </summary>
		public override void Simulate( Client cl )
		{
			base.Simulate( cl );
			base.TickPlayerUse();
			CarryController?.Simulate( this );
		}
	}
}
