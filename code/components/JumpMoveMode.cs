using Sandbox.Movement;
[Title( "MoveMode - Jump" )]
[Description( "[WIP] Make the player jump" )]
[Category( "Movement" )]
[Icon( "directions_walk" )]

public sealed class JumpMoveMode : MoveMode {
  [Property]
  public int Priority { get; set; } = 0;

  [Property]
  public float JumpForce { get; set; } = 400f;

  [Property, Range( 1, 20, 1 )]
  public int JumpAmount { get; set; } = 1;

  private int jumpLeft { get; set; } = 1;

  public override int Score( PlayerController controller ) {
    return Priority;
  }

  protected override void OnAwake() {
    base.OnAwake();
    jumpLeft = JumpAmount;
  }

  protected override void OnFixedUpdate() {
    base.OnFixedUpdate();
    CharacterController character = GetComponent<CharacterController>();
    if ( !character.IsValid() ) return;
    if ( !character.IsOnGround && jumpLeft == 0 ) return;

    if ( character.IsOnGround ) {
      jumpLeft = JumpAmount;
    }

    if ( Input.Pressed( "Jump" ) ) {
      character.Punch( Vector3.Up * JumpForce );
      jumpLeft--;
    }
  }
}
