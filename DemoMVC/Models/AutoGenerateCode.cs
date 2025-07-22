using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DemoMVC.Models
{
    public class AutoGenerateCode
    {
        public string GenerateCode(string lastId)
        {
            if (string.IsNullOrEmpty(lastId))
                return "PS001";

            string numberPart = lastId.Substring(2);
            int nextNumber = int.Parse(numberPart) + 1;
            return "PS" + nextNumber.ToString("D3"); // VD: PS002, PS003...
        }
    }
}