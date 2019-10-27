using System;

namespace FX
{
    internal class FxExplorer
    {
        public FxExplorer()
        {
        }

        internal void Explore(Type type)
        {
            var typeExplorer = new TypeExplorer();
            typeExplorer.Explore(type);
            typeExplorer.Explore(typeof(int));

        }
    }
}