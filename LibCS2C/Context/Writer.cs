using LibCS2C.Util;

namespace LibCS2C.Context
{
    public class Writer
    {
        /// <summary>
        /// Gets the output writer
        /// </summary>
        public FormattedStringBuilder CurrentWriter { get; private set; }

        /// <summary>
        /// If a postcode should be outputted
        /// </summary>
        public bool ShouldOutputPost { get; set; } = false;

        // String builders
        public FormattedStringBuilder SbEnums { get; private set; } = new FormattedStringBuilder();
        public FormattedStringBuilder SbStructs { get; private set; } = new FormattedStringBuilder();
        public FormattedStringBuilder SbClassStructs { get; private set; } = new FormattedStringBuilder();
        public FormattedStringBuilder SbDelegates { get; private set; } = new FormattedStringBuilder();
        public FormattedStringBuilder SbMethodPrototypes { get; private set; } = new FormattedStringBuilder();
        public FormattedStringBuilder SbMethodDeclarations { get; private set; } = new FormattedStringBuilder();
        private FormattedStringBuilder m_sbTempBuffer = new FormattedStringBuilder();
        private FormattedStringBuilder m_sbPostBuffer = new FormattedStringBuilder();
        private FormattedStringBuilder[] m_stringBuilders;

        // Current writing destination
        private WriterDestination m_currentDestination;

        /// <summary>
        /// Current destionation of the writer, this is because a destination depends on the structure of the code
        /// Therefor, we cannot assume one generator is one writer
        /// </summary>
        public WriterDestination CurrentDestination
        {
            get
            {
                return m_currentDestination;
            }

            set
            {
                m_currentDestination = value;
                CurrentWriter = m_stringBuilders[(int)value];
            }
        }

        /// <summary>
        /// Initializes the writer
        /// </summary>
        public Writer()
        {
            m_stringBuilders = new FormattedStringBuilder[] { SbEnums, SbStructs, SbClassStructs, SbDelegates, SbMethodPrototypes, SbMethodDeclarations, m_sbTempBuffer, m_sbPostBuffer };
        }

        /// <summary>
        /// Adds a tab
        /// </summary>
        public void Indent()
        {
            CurrentWriter.Indent();
        }

        /// <summary>
        /// Removes a tab
        /// </summary>
        public void UnIndent()
        {
            CurrentWriter.UnIndent();
        }

        /// <summary>
        /// Appends the indent tabs
        /// </summary>
        public void AppendIndent()
        {
            CurrentWriter.AppendIndent();
        }

        /// <summary>
        /// Appends text
        /// </summary>
        /// <param name="text">The text</param>
        public void Append(string text)
        {
            CurrentWriter.Append(text);
        }

        /// <summary>
        /// Appends text and a new line
        /// </summary>
        /// <param name="text">The text</param>
        public void AppendLine(string text)
        {
            CurrentWriter.AppendLine(text);
        }

        /// <summary>
        /// Flushes the temporary buffer
        /// </summary>
        /// <returns>The buffer contents</returns>
        public string FlushTempBuffer()
        {
            string ret = m_sbTempBuffer.ToString();
            m_sbTempBuffer.Clear();
            return ret;
        }

        /// <summary>
        /// Checks if the post buffer is empty
        /// </summary>
        /// <returns>If it's empty</returns>
        public bool IsPostBufferEmpty()
        {
            return m_sbPostBuffer.IsEmpty();
        }

        /// <summary>
        /// Flushes the post buffer
        /// </summary>
        /// <returns>The buffer contents</returns>
        public string FlushPostBuffer()
        {
            string ret = m_sbPostBuffer.ToString();
            m_sbPostBuffer.Clear();
            return ret;
        }
    }
}
