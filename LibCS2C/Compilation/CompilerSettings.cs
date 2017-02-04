namespace LibCS2C.Compilation
{
    class CompilerSettings
    {
        // Compiler flags
        public static bool EnableRuntimeChecks { get; set; } = true;

        // Error messages name
        public const string RuntimeErrorNullCalledName = "__ERROR_NULL_CALLED__";

        // Error message
        public const string RuntimeErrorNullCalled = "The program tried to call a method of an object that is null";
    }
}
