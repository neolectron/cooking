using Sandbox.Movement;

[Title( "MoveMode - Absolute" )]
[Description( "Rotates the entity to face the direction of movement." )]
[Category( "Movement" )]
[Icon( "directions_walk" )]
public class MoveModeAbsolute : MoveMode {
  [Property]
  public int Priority { get; set; } = 0;

  [Property]
  public float GroundAngle { get; set; } = 45f;

  [Property]
  public float StepUpHeight { get; set; } = 18f;

  [Property]
  public float StepDownHeight { get; set; } = 18f;

  public override bool AllowGrounding => true;

  public override bool AllowFalling => true;

  public override int Score( PlayerController controller ) {
    return Priority;
  }

  protected override void OnFixedUpdate() {
    base.OnFixedUpdate();
    Log.Info( "OnFixedUpdate" );
    DebugDraw();
  }

  public override void AddVelocity() {
    Log.Info( "AddVelocity" );
    base.Controller.WishVelocity = base.Controller.WishVelocity.WithZ( 0f );
    base.AddVelocity();
  }

  public override void PrePhysicsStep() {
    Log.Info( "PrePhysicsStep" );
    base.PrePhysicsStep();
    if ( StepUpHeight > 0f ) {
      TrySteppingUp( StepUpHeight );
    }
  }

  public override void PostPhysicsStep() {
    Log.Info( "PostPhysicsStep" );
    base.PostPhysicsStep();
    if ( StepDownHeight > 0f ) {
      StickToGround( StepDownHeight );
    }
  }

  public override bool IsStandableSurface( in SceneTraceResult result ) {
    Log.Info( "IsStandableSurface" );
    if ( Vector3.GetAngle( in Vector3.Up, in result.Normal ) > GroundAngle ) {
      return false;
    }

    return true;
  }

  [Description( "Read inputs, return WishVelocity" )]
  public override Vector3 UpdateMove( Rotation eyes, Vector3 input ) {
    Log.Info( "UpdateMove" );
    Angles value = eyes.Angles();
    value.pitch = 0f;
    eyes = value;
    return base.UpdateMove( eyes, input );
  }

  public override void OnModeBegin() {
    Log.Info( "MoveModeAbsolute has started." );
  }

  public override void OnModeEnd( MoveMode next ) {
    Log.Info( "MoveModeAbsolute has stopped. Next mode: " + next );
  }

  public void DebugDraw() {
    // From Me to the direction I'm moving
    Gizmo.Draw.Color = Color.Red;
    Gizmo.Draw.Line( WorldPosition, WorldPosition + Controller.Velocity );

    // From Me to the direction I should be looking at
    Angles shouldLookAt = getRotation( Controller.Velocity ).Angles();
    Gizmo.Draw.Color = Color.Blue;
    Gizmo.Draw.Line( WorldPosition, WorldPosition + Rotation.From( shouldLookAt ).Forward * 50 );
  }


  Rotation getRotation( Vector3 wishVelocity ) {
    Angles currentAngles = WorldRotation.Angles();
    currentAngles.yaw = Rotation.LookAt( wishVelocity ).Angles().yaw;
    return currentAngles;
  }
}
