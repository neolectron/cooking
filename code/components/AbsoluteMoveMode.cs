using Sandbox.Movement;

[Title( "MoveMode - Absolute" )]
[Description( "Rotates the entity to face the direction of movement." )]
[Category( "Movement" )]
[Icon( "transfer_within_a_station" )]
public class MoveModeAbsolute : MoveMode, PlayerController.IEvents {
  [Property]
  public int Priority { get; set; } = 0;

  [Property]
  public float GroundAngle { get; set; } = 45f;

  [Property]
  public float StepUpHeight { get; set; } = 18f;

  [Property]
  public float StepDownHeight { get; set; } = 18f;

  [Property]
  public override bool AllowGrounding => true;

  [Property]
  public override bool AllowFalling => true;

  [Property]
  public bool AllowRunning { get; set; } = false;

  [Property]
  public bool PrintDebug { get; set; } = false;

  public bool IsRunning { get; set; } = false;

  public override int Score( PlayerController controller ) {
    return Priority;
  }

  protected override void OnFixedUpdate() {
    base.OnFixedUpdate();
    IsRunning = AllowRunning && Input.Down( "Run" );
    DebugDraw();
  }

  public void OnEyeAngles( ref Angles angles ) {
    Log.Info( "OnEyeAngles" );
    if ( Controller.WishVelocity.IsNearlyZero() ) return; // keep previous rotation when vectors are too small
    //TODO: Fix Roation so it always faces the direction of movement
    angles = Rotation.LookAt( Controller.WorldPosition + Controller.WishVelocity ).Angles();
  }

  [Description( "Read inputs, return WishVelocity" )]
  public override Vector3 UpdateMove( Rotation eyes, Vector3 input ) {
    // Log.Info( "UpdateMove" );

    PlayerController player = Controller;
    Vector3 wishVelocity = new Vector3( -input.y, input.x, input.z ).WithZ( 0 ).Normal;

    if ( player.IsDucking ) wishVelocity *= player.DuckedSpeed;
    else if ( IsRunning ) wishVelocity *= player.RunSpeed;
    else wishVelocity *= player.WalkSpeed;

    if ( PrintDebug ) {
      Gizmo.Draw.Color = Color.Blue;
      Gizmo.Draw.Line( player.WorldPosition, player.WorldPosition + wishVelocity );
    }

    // Angles wishAngles = Rotation.LookAt( wishVelocity.Normal );
    // eyes = wishAngles;

    // return base.UpdateMove( eyes, wishVelocity );
    return wishVelocity;
  }

  public override void OnModeBegin() {
    Log.Info( "MoveModeAbsolute has started." );
    Controller.UseLookControls = false;
    Controller.LookSensitivity = 0f;
  }

  public override void OnModeEnd( MoveMode next ) {
    Log.Info( "MoveModeAbsolute has stopped. Next mode: " + next );
  }

  public override void AddVelocity() {
    // Log.Info( "AddVelocity" );
    Controller.WishVelocity = Controller.WishVelocity.WithZ( 0f );
    base.AddVelocity();
  }

  public override void PrePhysicsStep() {
    // Log.Info( "PrePhysicsStep" );
    base.PrePhysicsStep();
    if ( StepUpHeight > 0f ) {
      TrySteppingUp( StepUpHeight );
    }
  }

  public override void PostPhysicsStep() {
    // Log.Info( "PostPhysicsStep" );
    base.PostPhysicsStep();
    if ( StepDownHeight > 0f ) {
      StickToGround( StepDownHeight );
    }
  }

  public override bool IsStandableSurface( in SceneTraceResult result ) {
    // Log.Info( "IsStandableSurface" );
    if ( Vector3.GetAngle( in Vector3.Up, in result.Normal ) > GroundAngle ) {
      return false;
    }

    return true;
  }

  public void DebugDraw() {
    DebugOverlay.Text( Controller.WorldPosition + Vector3.Up * 100, "AbsoluteMoveMode" );

    // From Me to the direction I'm moving
    // Gizmo.Draw.Color = Color.Red;
    // Gizmo.Draw.Line( WorldPosition, WorldPosition + Controller.Velocity );
  }


}
