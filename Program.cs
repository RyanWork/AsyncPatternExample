using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AsyncPattern
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            Program program = new Program();
            
            var data = await program.GetAllEnumerableAsync();
            var data2 = await program.GetAllListAsync();

            // using DisposableClass disposable = new DisposableClass();
            // await using DisposableAsync disposableAsync = new DisposableAsync();
            // await using ChildDisposableAsync childDisposableAsync = new ChildDisposableAsync();
        }

        #region AsyncEnumerable

        public async Task<IEnumerable<SomeDataClass>> GetAllEnumerableAsync()
        {
            IList<SomeDataClass> someDataClasses = new List<SomeDataClass>();
            await foreach (var t in this.GetFromDatabaseEnumerableAsync())
            {
                someDataClasses.Add(t);
            }

            return someDataClasses;
        }

        public async Task<IEnumerable<SomeDataClass>> GetAllListAsync()
        {
            IList<SomeDataClass> anotherList = new List<SomeDataClass>();
            IList<SomeDataClass> someDataClasses = await GetFromDatabaseListAsync();
            foreach (var t in someDataClasses)
            {
                anotherList.Add(t);
            }

            return anotherList;
        }

        public async IAsyncEnumerable<SomeDataClass> GetFromDatabaseEnumerableAsync()
        {
            for(int i = 0; i < 10000; i++)
            {
                await DatabaseReadAsync();
                yield return this.FromReader(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            }
        }

        public async Task<IList<SomeDataClass>> GetFromDatabaseListAsync()
        {
            IList<SomeDataClass> someDataClasses = new List<SomeDataClass>();
            for(int i = 0; i < 10000; i++)
            {
                await DatabaseReadAsync();
                someDataClasses.Add(this.FromReader(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));
            }

            return someDataClasses;
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

        public async Task HeavyProcessingAsync()
        {
            await System.Threading.Tasks.Task.Delay(1000);
        }

        #endregion AsyncEnumerable

        #region AsyncDisposable

        public class DisposableClass : IDisposable
        {
            private Stream stream;

            public DisposableClass()
            {
                this.stream = new MemoryStream();
            }

            // Dispose method required ONLY by IDisposable
            public void Dispose()
            {
                this.Dispose(true);
                // Tell the runtime that we already cleaned it up ourselves
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (this.stream != null)
                    {
                        this.stream.Dispose();
                    }

                    // Other logic . . .
                }
            }
        }

        public class DisposableAsync : IDisposable, IAsyncDisposable
        {
            private Stream stream;

            // Dispose method required ONLY by IDisposable
            public void Dispose()
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (this.stream != null)
                    {
                        this.stream.Dispose();
                    }
                }
            }

            public async ValueTask DisposeAsync()
            {
                await this.DisposeAsyncCore();
                
                this.Dispose(false);
                GC.SuppressFinalize(this);
            }

            protected virtual async ValueTask DisposeAsyncCore()
            {
                if (this.stream != null)
                {
                    await this.stream.DisposeAsync();
                }
            }
        }

        public class ChildDisposableAsync : DisposableAsync, IDisposable, IAsyncDisposable
        {
            private Stream someOtherStream;

            public ChildDisposableAsync()
            {
                this.someOtherStream = new MemoryStream();
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (this.someOtherStream != null)
                    {
                        this.someOtherStream.Dispose();
                    }
                }

                base.Dispose(disposing);
            }

            protected override async ValueTask DisposeAsyncCore()
            {
                if (this.someOtherStream != null)
                {
                    await this.someOtherStream.DisposeAsync();
                }

                await base.DisposeAsyncCore();
            }
        }

        #endregion AsyncDisposable
    }
}
