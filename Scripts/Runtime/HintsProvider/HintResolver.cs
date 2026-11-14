using System;
using System.Collections.Generic;

namespace PostEnot.DxHints
{
    public class HintResolver<THint, TContext, TInteractableBase>
    {
        public HintResolver(TContext context) => Context = context;

        public TContext Context { get; }
        public HintProvider<THint, TContext> DefaultProvider { get; set; }

        private Type InteractableBaseType => typeof(TInteractableBase);

        private readonly Dictionary<Type, HintProvider<THint, TContext>> _providers = new();

        public bool TryResolve(Type interactableType, THint hint)
        {
            if ((interactableType == null) || (interactableType == InteractableBaseType))
            {
                if (DefaultProvider == null)
                {
                    return false;
                }
                return DefaultProvider.TryPerform(hint, Context);
            }
            if (_providers.TryGetValue(interactableType, out HintProvider<THint, TContext> provider))
            {
                return provider.TryPerform(hint, Context);
            }
            return TryResolve(interactableType.BaseType, hint);
        }

        public HintResolver<THint, TContext, TInteractableBase> Register<TInteractable>(Func<THint, TContext, bool> provider)
            where TInteractable : TInteractableBase
        {
            Type type = typeof(TInteractable);
            if (_providers.ContainsKey(type))
            {
                throw new InvalidOperationException();
            }
            HintProviderWrapper<THint, TContext> wrappedProvider = new(provider);
            _providers.Add(type, wrappedProvider);
            return this;
        }

        public HintResolver<THint, TContext, TInteractableBase> Register<TInteractable, THintProvider>()
            where TInteractable : TInteractableBase
            where THintProvider : HintProvider<THint, TContext>, new()
        {
            THintProvider hintProvider = new();
            return Register<TInteractable>(hintProvider);
        }

        public HintResolver<THint, TContext, TInteractableBase> Register<TInteractable>(HintProvider<THint, TContext> provider)
            where TInteractable : TInteractableBase
        {
            Type type = typeof(TInteractable);
            if (_providers.ContainsKey(type))
            {
                throw new InvalidOperationException();
            }
            _providers.Add(type, provider);
            return this;
        }
    }
}
