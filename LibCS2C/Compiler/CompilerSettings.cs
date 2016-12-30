namespace LibCS2C.Compiler
{
    class CompilerSettings
    {
        // Compiler flags
        public static bool EnableRuntimeChecks { get; set; } = true;

        // Error messages (defined in program's C source file)
        public const string RuntimeErrorNullCalled = "__ERROR_NULL_CALLED__";
    }
}
