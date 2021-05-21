using Sandbox;

namespace CookingFrenzy.FoodThings.Entities.Machines
{
	public interface IProcessMachine
	{
		public CraftType WorkType { get; }
		public bool CanProcess();
		public bool Process();
	}
	
	public abstract class MachineEntity : ModelEntity, IProcessMachine
	{
		public override void Spawn()
		{
			SetModel( Model );
			
			MoveType = MoveType.Physics;
			CollisionGroup = CollisionGroup.Interactive;
			PhysicsEnabled = true;
			UsePhysicsCollision = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;
		}
		protected virtual string Model => "content/models/cutting_board.vmdl";
		public abstract CraftType WorkType { get; }
		public abstract bool CanProcess();
		public abstract bool Process();
	}
}
