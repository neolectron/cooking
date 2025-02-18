using Sandbox.Citizen;

[Title( "TopDown Player Controller" )]
[Category( "Physics" )]
[Icon( "directions_walk" )]
public class TopDownPlayerController : Component {
  // Movements
  [Property] public float AirControl { get; set; } = 0.1f;
  [Property] public float MaxForce { get; set; } = 50f;
  [Property] public float Speed { get; set; } = 160f;
  [Property] public float RunSpeed { get; set; } = 290f;
  [Property] public float CrouchSpeed { get; set; } = 40f;
  [Property] public float JumpForce { get; set; } = 400f;
  [Property] public float GroundControl { get; set; } = 4.0f;

  // Components dependencies
  [Property] private CharacterController characterController;
  [Property] private CitizenAnimationHelper animationHelper;
  [Property] private GameObject Head { get; set; }

  // Flags
  [Property] public bool allowSprint { get; set; }
  [Property] public bool allowCrouch { get; set; }
  [Property] public bool allowJump { get; set; }
  [Property] public bool allowAnimation { get; set; }
  [Property] public bool printDebug { get; set; }

  // State
  public bool WasCrouching = false;

  protected override void OnAwake() {
    characterController = characterController.IsValid() ? characterController : Components.Get<CharacterController>();
    animationHelper = animationHelper.IsValid() ? animationHelper : Components.Get<CitizenAnimationHelper>();

    if ( !animationHelper.IsValid() ) {
      Log.Warning( "CitizenAnimationHelper component missing, skipping animations." );
    }
    if ( !characterController.IsValid() ) {
      Log.Error( "TopDown Player Controller requires a CharacterController component on same GameObject." );
      return;
    }
  }

  protected override void OnFixedUpdate() {
    var isCrouching = allowCrouch && Input.Down( "Duck" );
    var isReleaseCrouch = allowCrouch && Input.Released( "Duck" );
    var isRunning = allowSprint && Input.Down( "Run" );
    var isJumping = allowJump && Input.Pressed( "Jump" );

    Vector3 wishVelocity = getWishVelocity( isCrouching, isRunning );
    Move( wishVelocity );
    Rotate( wishVelocity );
    Crouch( isCrouching, isReleaseCrouch );
    if ( isJumping ) Jump();
    if ( allowAnimation ) Animate( wishVelocity, isJumping, isCrouching, isRunning );
    if ( printDebug ) DebugDraw( wishVelocity, isJumping, isCrouching, isRunning );
  }

  void Crouch( bool isCrouching, bool isReleaseCrouch ) {
    if ( !WasCrouching && isCrouching ) {
      characterController.Height /= 2f;
      WasCrouching = true;
      return;
    }

    if ( WasCrouching && isReleaseCrouch ) {
      //TODO: Check if we can stand up
      characterController.Height *= 2f;
      WasCrouching = false;
      return;
    }
  }

  void Jump() {
    if ( !characterController.IsOnGround ) return;

    characterController.Punch( Vector3.Up * JumpForce );
    animationHelper?.TriggerJump(); // Trigger our jump animation if we have one
  }

  Vector3 getWishVelocity( bool isCrouching, bool isRunning ) {
    Vector3 analogMove = Input.AnalogMove;
    Vector3 wishVelocity = new Vector3( -analogMove.y, analogMove.x, analogMove.z ).WithZ( 0 ).Normal;

    wishVelocity = wishVelocity.WithZ( 0 );

    if ( !wishVelocity.IsNearZeroLength ) wishVelocity = wishVelocity.Normal;

    if ( isCrouching ) wishVelocity *= CrouchSpeed; // Crouching takes presedence over sprinting
    else if ( isRunning ) wishVelocity *= RunSpeed; // Sprinting takes presedence over walking
    else wishVelocity *= Speed;

    return wishVelocity;
  }

  void Move( Vector3 wishVelocity ) {
    // Get gravity from our scene
    var gravity = Scene.PhysicsWorld.Gravity;

    if ( characterController.IsOnGround ) {
      // Apply Friction/Acceleration
      characterController.Velocity = characterController.Velocity.WithZ( 0 );
      characterController.Accelerate( wishVelocity );
      characterController.ApplyFriction( GroundControl );
    }
    else {
      // Apply Air Control / Gravity
      characterController.Velocity += gravity * Time.Delta * 0.5f;
      characterController.Accelerate( wishVelocity.ClampLength( MaxForce ) );
      characterController.ApplyFriction( AirControl );
    }

    // Move the character controller
    characterController.Move();

    // Apply the second half of gravity after movement
    if ( !characterController.IsOnGround ) {
      characterController.Velocity += gravity * Time.Delta * 0.5f;
    }
    else {
      characterController.Velocity = characterController.Velocity.WithZ( 0 );
    }
  }

  void Rotate( Vector3 wishVelocity ) {
    if ( wishVelocity.IsNearlyZero() ) return;

    Angles currentAngles = WorldRotation.Angles();
    currentAngles.yaw = Rotation.LookAt( wishVelocity ).Angles().yaw;
    WorldRotation = Rotation.From( currentAngles );
  }

  void Animate( Vector3 wishVelocity, bool isJumping = false, bool isCrouching = false, bool _isRunning = false ) {
    if ( !animationHelper.IsValid() ) return;
    var hasHead = Head.IsValid();

    animationHelper.WithVelocity( wishVelocity );
    if ( isJumping && characterController.IsOnGround ) animationHelper?.TriggerJump();
    animationHelper.AimAngle = hasHead ? Head.WorldRotation : Rotation.LookAt( wishVelocity );
    animationHelper.IsGrounded = characterController.IsOnGround;
    if ( hasHead ) animationHelper.WithLook( Head.WorldRotation.Forward, 1, 0.75f, 0.5f );
    // animationHelper.MoveStyle = CitizenMoveStyle.MoveStyles.Run;
    animationHelper.DuckLevel = isCrouching ? 1f : 0f;
  }

  void DebugDraw( Vector3 wishVelocity, bool isJumping, bool isCrouching, bool isRunning ) {
    var text = $"WishVelocity: {wishVelocity}\nJumping: {isJumping}\nCrouching: {isCrouching}\nRunning: {isRunning}";
    DebugOverlay.Text( WorldPosition + Vector3.Up * 100, text );
  }

}
