using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectDemo.data
{
    public class RecordMode
    {
        private string directory;
        private string prefix;
        private string language;
        private string name;

        public RecordMode(string directory, string prefix)
        {
            this.directory = directory;
            this.prefix = prefix;
        }

        public RecordMode WithName(string name)
        {
            this.name = name;
            return this;
        }

        public RecordMode WithLanguage(string language)
        {
            this.language = language;
            return this;
        }

        public string FullOutDir() { return Constants.DIR_BASE_OUTPUT + directory; }

        public string ComposeFileName()
        {
            return string.Format("{0}_{1}_{2}", prefix, language, name);
        }
    }
}
