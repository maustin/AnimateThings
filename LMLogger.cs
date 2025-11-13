namespace LinkedMovement {
    public enum LogLevel {
        Debug = 0,
        Info = 1,
        Error = 2,
    }

    public static class LMLogger {
        private static LogLevel minLogLevel = LogLevel.Debug;

        public static void SetLogLevel(LogLevel level) {
            minLogLevel = level;
        }

        public static void Debug(string message) {
            if (minLogLevel <= LogLevel.Debug) {
                UnityEngine.Debug.Log("Animate Things: [debug] " + message);
            }
        }

        public static void Info(string message) {
            UnityEngine.Debug.Log("Animate Things: [info] " + message);
        }

        public static void Error(string message) {
            UnityEngine.Debug.LogError("Animate Things: [error] " + message);
        }
    }
}
