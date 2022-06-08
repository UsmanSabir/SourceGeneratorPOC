using System.Diagnostics;

namespace WebApi.Biz
{
    public interface IBizService : CoreLib.IBusinessService
    {
        int TryMe(string s);

        void Raw(Abc abc);
    }
    public class BzService : IBizService
    {
        public BzService()
        {
            Debugger.Break();
            Debug.WriteLine("Created");
        }

        public void Raw(Abc abc)
        {
            Debug.WriteLine("Raw Done");
        }

        public int TryMe(string s)
        {
            return 0;
        }
    }

    public record Abc(int Id, string Name, string Value);
}
