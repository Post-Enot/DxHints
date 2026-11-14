using System;

namespace PostEnot.DxHints
{
    internal sealed class HintProviderWrapper<THint, TContext> : HintProvider<THint, TContext>
    {
        public HintProviderWrapper(Func<THint, TContext, bool> predicate)
            => _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));

        private readonly Func<THint, TContext, bool> _predicate;

        public override bool TryPerform(THint target, TContext context) => _predicate(target, context);
    }
}
