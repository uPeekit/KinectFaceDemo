using KinectDemo.data;

namespace KinectDemo
{
    class Constants
    {
        public const char CSV_SEPARATOR = ';';
        public const string DIR_BASE_OUTPUT = @"C:\KinectData\";
        public const string DIR_RESULT = @"result\";
        public const string RESULT_FILE_NAME = "result";
        public static readonly RecordMode MODE_NATIVE = new RecordMode(@"native\", "n");
        public static readonly RecordMode MODE_FOREIGN = new RecordMode(@"foreign\", "f");
    }
}
