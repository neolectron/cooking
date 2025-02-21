public sealed class Usable : Component, Component.IPressable {
  [Property] public Action<GameObject> OnUse { get; set; }

  public void Use( GameObject user ) {
    OnUse?.Invoke( user );
  }

  public void Hover( IPressable.Event e ) {
    // Implement hover logic here
    Log.Info( "Hover event triggered." );
  }

  public void Look( IPressable.Event e ) {
    // Implement look logic here
    Log.Info( "Look event triggered." );
  }

  public void Blur( IPressable.Event e ) {
    // Implement blur logic here
    Log.Info( "Blur event triggered." );
  }

  public bool Press( IPressable.Event e ) {
    // Implement press logic here
    Log.Info( "Press event triggered." );
    return true;
  }

  public bool Pressing( IPressable.Event e ) {
    // Implement pressing logic here
    Log.Info( "Pressing event triggered." );
    return true;
  }

  public void Release( IPressable.Event e ) {
    // Implement release logic here
    Log.Info( "Release event triggered." );
  }

  public bool CanPress( IPressable.Event e ) {
    // Implement can press logic here
    Log.Info( "CanPress event triggered." );
    return true;
  }
}
