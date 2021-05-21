using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookingFrenzy
{
	public class FloatingUIPanel<T> : Panel where T : ModelEntity
	{
		public T BelongingEntity { set; get; }
		public virtual string Stylesheet => "/ui/main.scss";

		public FloatingUIPanel()
		{
			StyleSheet.Load( Stylesheet );
		}

		public virtual void BuildPanel()
		{
			Label l = AddChild<Label>();

			l.Text = $"Example FloatingUI Panel {BelongingEntity.NetworkIdent}";
		}

		// Meant to be overriden
		public virtual void UpdateFromEnt( T ent ) { }
	}

	public class FloaterCollection<T, P> : Panel where T : ModelEntity where P : FloatingUIPanel<T>, new()
	{
		Dictionary<T, P> PanelCollection;
		public virtual float MaxDistance => 300f;
		public virtual int MaxToShow => 15;

		public Vector3 Offset = Vector3.Zero;

		public FloaterCollection()
		{
			PanelCollection = new();
			
			Style.Width = Length.Fraction( 1 );
			Style.Dirty();
		}

		public override void Tick()
		{
			base.Tick();

			
			Camera currentCamera = Local.Client.Pawn.Camera as Camera;

			if ( currentCamera is null )
				return;

			List<T> PurgeList = new();
			PurgeList.AddRange( PanelCollection.Keys );

			IEnumerable<T> AllEnts = Entity.All.OfType<T>().OrderBy( x => Vector3.DistanceBetween( x.Position, currentCamera.Pos ) );

			int count = 0;
			foreach ( T Entity in AllEnts )
			{
				if ( UpdateTag( Entity ) )
				{
					PurgeList.Remove( Entity );

					count++;
				}

				if ( count >= MaxToShow )
					break;
			}

			foreach ( T Entity in PurgeList )
			{
				Log.Warning( $"Purged tag for ({Entity.NetworkIdent})!" );
				PanelCollection[ Entity ].Delete();
				PanelCollection.Remove( Entity );
			}
		}

		public virtual P CreateNametag(T Entity)
		{
			P Panel = new();

			Panel.Parent = this;
			Panel.BelongingEntity = Entity;
			Panel.BuildPanel();

			return Panel;
		}

		// Feel free to change how we update tags I guess
		public virtual bool UpdateTag( T entity )
		{
			//
			// Where we putting the label, in world coords
			//
			var position = entity.Position + Offset;

			var currentCamera = Local.Client.Pawn.Camera as Camera;

			if ( currentCamera is null )
				return false;

			//
			// Are we too far away?
			//
			var dist = position.Distance( currentCamera.Pos );
			if ( dist > MaxDistance )
				return false;

			//
			// Are we looking in this direction?
			//
			var lookDir = (position - currentCamera.Pos).Normal;
			if ( currentCamera.Rot.Forward.Dot( lookDir ) < 0.5 )
				return false;

			var alpha = dist.LerpInverse( MaxDistance, MaxDistance * 0.1f, true );

			// If I understood this I'd make it proper function
			var objectSize = 0.05f / dist / (2.0f * MathF.Tan( (currentCamera.FieldOfView / 2.0f).DegreeToRadian() )) * 1500.0f;

			objectSize = objectSize.Clamp( 0.05f, 1.0f );

			if ( !PanelCollection.TryGetValue( entity, out var tag ) )
			{
				tag = CreateNametag( entity );
				PanelCollection[ entity ] = tag;

				Log.Warning( $"Created a new tag for ({entity.NetworkIdent})!" );
			}

			Vector3 screenPos = position.ToScreen();
			tag.UpdateFromEnt( entity );

			tag.Style.Position = PositionMode.Absolute;
			tag.Style.Left = Length.Fraction( screenPos.x );
			tag.Style.Top = Length.Fraction( screenPos.y );
			tag.Style.Opacity = alpha;

			var transform = new PanelTransform();
			transform.AddTranslateY( Length.Fraction( -1.0f ) );
			transform.AddScale( objectSize );
			transform.AddTranslateX( Length.Fraction( -0.5f ) );

			tag.Style.Transform = transform;
			tag.Style.Dirty();

			return true;
		}
	}
}
