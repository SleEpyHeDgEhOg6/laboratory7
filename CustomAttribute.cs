using System;
//пользовательский атрибут. Атрибут для добавления комментариев к классам и перечислениям
namespace AnimalLibrary
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Struct, 
        AllowMultiple = false, Inherited = false)]
    public class CommentAttribute : Attribute
    {
        public string Comment { get; }

        public CommentAttribute(string comment)
        {
            Comment = comment;
        }
    }
}
