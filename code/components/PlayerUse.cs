using System.Linq;

[Title( "Player use ability" )]
[Category( "Physics" )]
[Icon( "emoji_people" )]
public sealed class PlayerUse : Component {
  // Properties
  [Property] public bool allowUse = true;
  [Property] public float useRange = 25f;
  [Property] public bool debugDraw = false;
  [Property] TagSet useTags { get; set; }
  [Property] SoundEvent useSound { get; set; }

  // State
  private Vector3 lastGizmoPos = Vector3.Zero;

  protected override void OnUpdate() {
    bool isUsing = allowUse && Input.Pressed( "use" );
    if ( isUsing ) { Use(); }

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

  void Use() {

    GameObject usable = GetNearestUsable();
    if ( usable == null ) { return; }
    Log.Info( "Using " + usable.Name );
    Sound.Play( useSound, WorldPosition );

  }

  GameObject GetNearestUsable() {

    Vector3 lookDir = WorldRotation.Normal.Forward;

    SceneTrace wishTrace = Scene.Trace
    .Ray( WorldPosition + lookDir * useRange, Vector3.Zero )
    .Radius( useRange )
    .WithoutTags( "player" )
    .IgnoreGameObjectHierarchy( GameObject );

    if ( debugDraw ) {
      lastGizmoPos = wishTrace.Run().EndPosition;
    }

    return wishTrace.RunAll().FirstOrDefault( traceResult => traceResult.GameObject.Tags.HasAny( useTags ) ).GameObject;
  }
}
