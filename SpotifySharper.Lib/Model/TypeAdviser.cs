using System;

namespace SpotifySharper.Lib.Model
{
    [Serializable]
    public class TypeAdviser
    {
        public string TypeName { get; set; }

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