using FlagPFP.Core.Exceptions;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FlagPFP.Core.Loading
{
    /// <summary>
    /// The <c>PrideFlag</c> object holds data about its parameter name and its image file name.
    /// </summary>
    public class PrideFlag
    {
        /// <summary>
        /// The file name of the flag's image.
        /// </summary>
        public string FlagFile { get; set; }
        /// <summary>
        /// (Case sensitive) The parameter name, for example <c>transgender</c> for the Transgender flag.
        /// </summary>
        public string ParameterName { get; set; }
    }

    internal class FlagLoader
    {
        public Dictionary<string, PrideFlag> LoadFlags(string folder)
        {
            List<string> files = Directory.GetFiles(folder).ToList();
            if (files.Count == 0) throw new NoFlagsFoundException();
            Dictionary<string, PrideFlag> finalList = new Dictionary<string, PrideFlag>();

            foreach (string file in files)
            {
                string jsonContent = File.ReadAllText(file);
                PrideFlag flag = JsonConvert.DeserializeObject<PrideFlag>(jsonContent);

                if (!string.IsNullOrWhiteSpace(flag.FlagFile)) finalList.Add(flag.ParameterName, flag);
            }
            return finalList;
        }
    }
}
