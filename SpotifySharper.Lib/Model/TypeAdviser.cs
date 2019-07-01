using System;

namespace SpotifySharper.Lib.Model
{
    public class TypeAdviser
    {
        public string TypeName { get; }

        private TypeAdviser()
        {
        }

        public TypeAdviser(string typeName)
        {
            TypeName = typeName;
        }

        public Type ResolveType()
            => Type.GetType(TypeName);
    }
}