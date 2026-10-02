using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Engine;

// Basisklasse für Anwendungen: trennt die Logik (FixedUpdate, Update) vom Zeichnen (Render).
// Ablauf pro Frame: 0..n × FixedUpdate, dann einmal Update, dann einmal Render.
// Die OpenTK-Callbacks sind versiegelt, damit dieser Ablauf nicht umgangen wird.
public abstract class Application : GameWindow
{
    // Begrenzt das Nachholen nach einem Ruckler, sonst kann FixedUpdate den Frame immer weiter verzögern.
    private const int MaxFixedStepsPerFrame = 5;

    private double _fixedDeltaTime = 1.0 / 50.0;
    private double _fixedTimeAccumulator;

    protected Application(GameWindowSettings gameSettings, NativeWindowSettings windowSettings)
        : base(gameSettings, windowSettings) { }

    // Schrittweite von FixedUpdate in Sekunden (Standard: 50 Schritte pro Sekunde).
    public double FixedDeltaTime
    {
        get => _fixedDeltaTime;
        set => _fixedDeltaTime = value > 0 ? value : throw new ArgumentOutOfRangeException(nameof(value));
    }

    // Wird einmal nach dem Erzeugen des OpenGL-Kontexts aufgerufen.
    protected virtual void Initialize() { }

    // Wird nach Initialize und bei jeder Größenänderung des Framebuffers aufgerufen (nie mit 0).
    protected virtual void FramebufferResized(int width, int height) { }

    // Läuft mit fester Schrittweite, unabhängig von der Framerate. Für Physik und Simulation.
    protected virtual void FixedUpdate(float fixedDeltaTime) { }

    // Läuft einmal pro Frame. deltaTime ist die Zeit seit dem letzten Frame in Sekunden.
    protected virtual void Update(float deltaTime) { }

    // Zeichnet den aktuellen Zustand. Ändert keinen Zustand und kennt deshalb keine Zeit.
    protected abstract void Render();

    // Wird vor dem Schließen aufgerufen, solange der OpenGL-Kontext noch existiert.
    protected virtual void Shutdown() { }

    protected sealed override void OnLoad()
    {
        base.OnLoad();
        Initialize();
        if (FramebufferSize.X > 0 && FramebufferSize.Y > 0)
            FramebufferResized(FramebufferSize.X, FramebufferSize.Y);
    }

    protected sealed override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        base.OnFramebufferResize(e);
        if (e.Width > 0 && e.Height > 0)
            FramebufferResized(e.Width, e.Height);
    }

    protected sealed override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);

        _fixedTimeAccumulator += e.Time;
        int steps = 0;
        while (_fixedTimeAccumulator >= _fixedDeltaTime && steps < MaxFixedStepsPerFrame)
        {
            FixedUpdate((float)_fixedDeltaTime);
            _fixedTimeAccumulator -= _fixedDeltaTime;
            steps++;
        }
        // Was nach dem Limit noch übrig ist, wird verworfen: die Simulation läuft dann kurz langsamer.
        if (_fixedTimeAccumulator >= _fixedDeltaTime)
            _fixedTimeAccumulator = 0;

        Update((float)e.Time);
    }

    protected sealed override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);
        // Ein minimiertes Fenster hat keinen Framebuffer, in den gezeichnet werden kann.
        if (FramebufferSize.X == 0 || FramebufferSize.Y == 0) return;

        Render();
        SwapBuffers();
    }

    protected sealed override void OnUnload()
    {
        Shutdown();
        base.OnUnload();
    }
}
