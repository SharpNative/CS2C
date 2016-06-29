namespace Sharpen
{
    public class Test
    {
        private static int Length(string str)
        {
            return 3;
        }
    }

    public unsafe class Console
    {
        private static byte* vidmem = (byte*)0xB8000;

        public static int X { get; private set; } = 0;
        public static int Y { get; private set; } = 0;

        public unsafe static void PutChar(char ch)
        {
            if (ch == '\n')
            {
                X = 0;
                Y++;
            }
            else if (ch == '\t')
            {
                X = (X + 4) & ~(4 - 1);
            }
            else
            {
                vidmem[(Y * 25 + X) * 2 + 0] = (byte)ch;
                vidmem[(Y * 25 + X) * 2 + 1] = 0x07;

                X++;
            }

            if (X == 80)
            {
                X = 0;
                Y++;
            }

            // TODO: scroll
        }

        public static void Write(string text)
        {
            for (int i = 0; i < Test.Length(text); i++)
                PutChar(text[i]);
        }

        public static void WriteLine(string text)
        {
            Write(text);
            PutChar('\n');
        }
    }
}
