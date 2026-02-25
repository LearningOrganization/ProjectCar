public readonly struct SceneId
{
    private readonly PlayingScene? _playing;
    private readonly NonPlayingScene? _nonPlaying;

    public SceneId(PlayingScene scene) { _playing = scene; _nonPlaying = null; }
    public SceneId(NonPlayingScene scene) { _nonPlaying = scene; _playing = null; }

    public bool IsPlaying => _playing.HasValue;
    public PlayingScene Playing => _playing.Value;
    public NonPlayingScene NonPlaying => _nonPlaying.Value;

    public static implicit operator SceneId(PlayingScene s) => new SceneId(s);
    public static implicit operator SceneId(NonPlayingScene s) => new SceneId(s);

    // overrided for sceneloader
    public override string ToString() => IsPlaying 
        ? _playing.Value.ToString() 
        : _nonPlaying.Value.ToString();
}