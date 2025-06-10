using UnityEngine;
using UnityEngine.Playables;
using Farm.Dialog;

[System.Serializable]
public class DialogBehavior : PlayableBehaviour
{
    private PlayableDirector director;
    public DialogPiece dialogPiece;

    public override void OnPlayableCreate(Playable playable)
    {
        // base.OnPlayableCreate(playable);
        director = playable.GetGraph().GetResolver() as PlayableDirector;
    }

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        // base.OnBehaviourPlay(playable, info);
        EventHandler.CallShowDialogEvent(dialogPiece);

        if (Application.isPlaying)
        {
            if (dialogPiece.hasToPause)
            {
                // Pause Timeline
                TimelineManager.Instance.PauseTimeline(director);
            }
            else
            {
                EventHandler.CallShowDialogEvent(null);
            }
        }
    }
}