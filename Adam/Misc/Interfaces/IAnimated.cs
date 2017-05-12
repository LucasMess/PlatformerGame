namespace ThereMustBeAnotherWay.Misc.Interfaces
{
    public enum AnimationState
    {
        Still,
        Walking,
        Jumping,
        Charging,
        Talking,
        Sleeping,
        Flying,
        Transforming,
    }

    public interface IAnimated
    {
        Animation Animation { get; }
        AnimationData[] AnimationData { get; }
        AnimationState CurrentAnimationState { get; set; }

        void Animate();
    }
}
