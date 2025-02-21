using System.Linq;

[Title( "Player use ability" )]
[Category( "Physics" )]
[Icon( "emoji_people" )]
public sealed class PlayerUse : Component, Component.ITriggerListener, PlayerController.IEvents {
  // Properties
  [Property] public bool allowUse = true;
  [Property] public bool allowPickups = true;
  [Property] public float useRange = 25f;
  [Property] public float useSphereRadius = 25f;
  [Property] public bool debugDraw = false;

  [Property]
  [Description( "Will automatically add the Usable Component to any GameObject with these tags" )]
  TagSet AutoUseTags { get; set; } = ["usable"];

  [Property]
  [Description( "Will automatically add the Pickup Component to any GameObject with these tags" )]
  TagSet AutoPickupTags { get; set; } = ["pickup"];

  // TODO: allow to walk on pickups to pick them up
  [Property]
  [Description( "Will automatically add the Pickup Component as Trigger to any GameObject with these tags" )]
  TagSet AutoTriggerPickup { get; set; } = ["triggerpickup"];

  [Property] SoundEvent failSound { get; set; }

  // State
  private Vector3 lastGizmoPos = Vector3.Zero;

  protected override void OnUpdate() {
    bool isTryingToUse = allowUse && Input.Pressed( "use" );
    //TODO: sadly I won't be able to override it like that, it still call FailPressing if no usable object was in range.
    if ( isTryingToUse ) { FindUsableObjects(); }

    // Debug
    if ( debugDraw && lastGizmoPos != Vector3.Zero ) {
      DebugPrintGizmo();
    }
  }

  [Button( "Clear gizmo" )]
  void _DebugClearGizmo() {
    lastGizmoPos = Vector3.Zero;
  }

  [Button( "Print gizmo" )]
  void DebugPrintGizmo() {
    Gizmo.Draw.SolidSphere( lastGizmoPos, useRange );
  }

  public GameObject FindUsableObjects() {
    Log.Info( "FindUsableObjects " );

    Vector3 lookDir = WorldRotation.Normal.Forward;

    SceneTrace wishTrace = Scene.Trace
    .Ray( WorldPosition + lookDir * useRange, Vector3.Zero )
    .Radius( useSphereRadius )
    .WithoutTags( "player" )
    .IgnoreGameObjectHierarchy( GameObject );


    if ( debugDraw ) {
      lastGizmoPos = wishTrace.Run().EndPosition;
    }


    var match = ( SceneTraceResult traceResult ) => traceResult.GameObject
      .Tags.HasAny( [.. AutoPickupTags, .. AutoUseTags] );

    SceneTraceResult traceResult = wishTrace.RunAll().FirstOrDefault( match );
    return traceResult.GameObject;
  }

  //---------------------- PlayerController.IEvents ----------------------//
  //
  // Summary:
  //     Used by the Using system to find components we can interact with. By default
  //     we can only interact with IPressable components. Return a component if we can
  //     use it, or else return null.
  public Component GetUsableComponent( GameObject go ) {
    if ( go.Tags.HasAny( AutoUseTags ) ) {
      return go.GetOrAddComponent<Usable>();
    }
    if ( go.Tags.HasAny( AutoPickupTags ) ) {
      return go.GetOrAddComponent<Pickup>();
    }
    return null;

  }

  //
  // Summary:
  //     We have started using something (use was pressed)
  public void StartPressing( Component target ) {
    Log.Info( "Start pressing" );
  }

  //
  // Summary:
  //     We have started using something (use was pressed)
  public void StopPressing( Component target ) {
    Log.Info( "Stop pressing" );
  }

  //
  // Summary:
  //     We pressed USE but it did nothing
  public void FailPressing() {
    Log.Info( "Failed pressing" );
    Sound.Play( failSound, WorldPosition );
  }

  //---------------------- PlayerController.ITriggerListener ----------------------//
  //
  // Summary:
  //     Called when a collider enters the trigger.
  void OnTriggerEnter( Collider other ) {
  }

  //
  // Summary:
  //     Called when a collider stops touching the trigger.
  void OnTriggerExit( Collider other ) {
  }

  //
  // Summary:
  //     Called when a gameobject enters the trigger.
  void OnTriggerEnter( GameObject other ) {
  }

  //
  // Summary:
  //     Called when a gameobject stops touching the trigger.
  void OnTriggerExit( GameObject other ) {
  }
}
