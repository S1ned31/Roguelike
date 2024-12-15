using System;

namespace Roguelike.Entities
{
    public class CustomException : Exception
    {
        public CustomException(string massage) : base(massage) { }
        public CustomException(string massage, Exception innerExeption) : base(massage, innerExeption) { }
    }
}