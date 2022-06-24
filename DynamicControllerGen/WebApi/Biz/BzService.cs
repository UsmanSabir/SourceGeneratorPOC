using System.Diagnostics;

namespace WebApi.Biz
{
    public interface IBizService : CoreLib.IBusinessService
    {
        int TryMe(string s);

        int TryDefault(string d= "test");

        void Raw(Abc abc);

        void DoneLocal();
        Task DoneAsync();

        int MultiLocal(int id, string s, Abc abc);
        Task<int> MultiAsync(int id, string s, Abc abc);


    }
    public class BzService : IBizService
    {
        public BzService()
        {
            Debugger.Break();
            Debug.WriteLine("Created");
        }

        public void DoneLocal()
        {
            Debug.WriteLine("Done");
        }

        public async Task DoneAsync()
        {
            
        }

        public int MultiLocal(int id, string s, Abc abc)
        {
            Debug.WriteLine($"Multi: {id}:{s}");
            return 5;
        }

        public async Task<int> MultiAsync(int id, string s, Abc abc)
        {
            return 5;
        }

        public void Raw(Abc abc)
        {
            Debug.WriteLine("Raw Done");
        }

        public int TryDefault(string d = "test")
        {
            Debug.WriteLine($"TryDefault: {d}");
            return 5;
        }

        public int TryMe(string s)
        {
            Debug.WriteLine($"TryMe: {s}");
            return 0;
        }
    }

    public class HelloWorldService : IBizService
    {
        public HelloWorldService()
        {
            Debugger.Break();
            Debug.WriteLine("Created");
        }

        public void DoneLocal()
        {
            Debug.WriteLine("Done");
        }

        public async Task DoneAsync()
        {

        }

        public int MultiLocal(int id, string s, Abc abc)
        {
            Debug.WriteLine($"Multi: {id}:{s}");
            return 5;
        }

        public async Task<int> MultiAsync(int id, string s, Abc abc)
        {
            return 5;
        }

        public void Raw(Abc abc)
        {
            Debug.WriteLine("Raw Done");
        }

        public int TryDefault(string d = "test")
        {
            Debug.WriteLine($"TryDefault: {d}");
            return 5;
        }

        public int TryMe(string s)
        {
            Debug.WriteLine($"TryMe: {s}");
            return 0;
        }

        public int SayHelloWorld(string s)
        {
            Debug.WriteLine($"TryMe: {s}");
            return 0;
        }
    }

    public record Abc(int Id, string Name, string Value);
}
