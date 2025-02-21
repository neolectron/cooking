public sealed class Pickup : Component, Component.IPressable {
  //TODO: make the pickup a trigger, so players can walk on it to automatically pick it up.
  [Property] public bool autoPickupOnTriggerEnter = true;

  protected override void OnAwake() {
    Log.Info( "awake" );
  }

  public void Hover( IPressable.Event e ) {
    Prop p = Components.Get<Prop>();
    p.Tint = Color.Red;
  }

  public void Look( IPressable.Event e ) {
    Prop p = Components.Get<Prop>();
    p.Tint = Color.Red;
  }

  public void Blur( IPressable.Event e ) {
    Prop p = Components.Get<Prop>();
    p.Tint = Color.White;
  }

  //
  // Summary:
  //     Describes who pressed it.
  public bool Press( IPressable.Event e ) {
    Log.Info( "Press event triggered by " + e.Source.GameObject.Name );
    return true;
  }

  public bool Pressing( IPressable.Event e ) {
    Log.Info( "Pressing event triggered." );
    return true;
  }

  public void Release( IPressable.Event e ) {
    Log.Info( "Release event triggered." );
  }

  public bool CanPress( IPressable.Event e ) {
    var text = "Yes you can Press me!";
    DebugOverlay.Text( WorldPosition + Vector3.Up * 40, text );
    return true;
  }
}
