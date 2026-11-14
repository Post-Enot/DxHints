namespace PostEnot.DxHints
{
    public abstract class HintProvider<THint, TContext>
    {
        public abstract bool TryPerform(THint hint, TContext context);
    }
}
