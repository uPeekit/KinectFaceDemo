﻿namespace KinectDemo
{
    class Constants
    {
        public const char CSV_SEPARATOR = ';';

        public const string DIR_BASE_OUTPUT = @"C:\KinectData\";
        public const string DIR_RESULT = @"result\";
        public const string DIR_CONVERTED = @"converted\";
        public const string DIR_TIME_DELTAS = @"time_deltas\";

        public const string RESULT_DISTANCE_AVG = "distance_avg";
        public const string RESULT_DISTANCE_SUM = "distance_sum";
        public const string RESULT_FILE_NAME = "result";
        public const string LOG_FILE_NAME = "log.txt";

        public static readonly RecordMode MODE_NATIVE = new RecordMode(@"native\", "n");
        public static readonly RecordMode MODE_FOREIGN = new RecordMode(@"foreign\", "f");
    }
}
