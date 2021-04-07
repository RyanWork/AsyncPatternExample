using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AsyncPattern
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            Program program = new Program();
            var data = await program.GetAllAsync();
        }

        public async Task<SomeParentClass> GetAllAsync()
        {
            IList<SomeDataClass> someDataClasses = new List<SomeDataClass>();
            await foreach (var t in this.SeedData())
            {
                someDataClasses.Add(t);
            }

            return new SomeParentClass()
            {
                SomeDataClasses = someDataClasses
            };
        }

        public async IAsyncEnumerable<SomeDataClass> SeedData()
        {
            IList<SomeDataClass> someDataClasses = new List<SomeDataClass>();
            for(int i = 0; i < 100; i++)
            {
                await DatabaseReadAsync();
                yield return this.FromReader(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            }
        }

        public SomeDataClass FromReader(string name, string lastName)
        {
            return new SomeDataClass()
            {
                Name = name,
                LastName = lastName
            };
        }

        public async Task DatabaseReadAsync()
        {
            await System.Threading.Tasks.Task.Delay(10);
        }
    }

    public class SomeParentClass
    {
        public IEnumerable<SomeDataClass> SomeDataClasses { get; set; }
    }

    public class SomeDataClass
    {
        public string Name { get; set; }

        public string LastName { get; set; }
    }
}
