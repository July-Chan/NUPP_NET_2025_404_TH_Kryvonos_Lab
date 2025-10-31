using System;
using PublicTransit.Common.App.Classes;
using PublicTransit.Common.App.Crud;

namespace PublicTransit.Common.Tests
{
    public class TestEntity : IWithId
    {
        public Guid Id { get; }
        public string Name { get; set; }

        public TestEntity(string name)
        {
            Id = Guid.NewGuid();
            Name = name;
        }
    }
}
