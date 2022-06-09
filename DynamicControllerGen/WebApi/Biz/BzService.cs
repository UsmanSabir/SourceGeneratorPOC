using System.Diagnostics;

namespace WebApi.Biz
{
    public interface IBizService : CoreLib.IBusinessService
    {
        int TryMe(string s);

        void Raw(Abc abc);

        void Done();

        int Multi(int id, string s, Abc abc);

    }
    public class BzService : IBizService
    {
        public BzService()
        {
            Debugger.Break();
            Debug.WriteLine("Created");
        }

        public void Done()
        {
            Debug.WriteLine("Done");
        }

        public int Multi(int id, string s, Abc abc)
        {
            Debug.WriteLine($"Multi: {id}:{s}");
            return 5;
        }

        public void Raw(Abc abc)
        {
            Debug.WriteLine("Raw Done");
        }

        public int TryMe(string s)
        {
            Debug.WriteLine($"TryMe: {s}");
            return 0;
        }
    }

    public record Abc(int Id, string Name, string Value);
}
