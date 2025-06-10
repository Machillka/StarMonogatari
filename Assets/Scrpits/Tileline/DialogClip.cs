using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


public class DialogClip : PlayableAsset, ITimelineClipAsset
{
    // 使得可以播放
    public ClipCaps clipCaps => ClipCaps.None;

    public DialogBehavior dialogBehavior = new DialogBehavior();

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<DialogBehavior>.Create(graph, dialogBehavior);
        return playable;
    }
}
