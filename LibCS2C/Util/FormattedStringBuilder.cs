using System.Text;

namespace LibCS2C.Util
{
    public class FormattedStringBuilder
    {
        private StringBuilder m_sb;
        private string m_tabs;

        private bool m_alreadyIndented = false;

        private bool m_empty = true;

        /// <summary>
        /// Formatted string builder
        /// </summary>
        public FormattedStringBuilder()
        {
            m_sb = new StringBuilder();
            m_tabs = "";
        }

        /// <summary>
        /// Clears the buffer
        /// </summary>
        public void Clear()
        {
            m_sb.Clear();
            m_empty = true;
            m_tabs = "";
        }

        /// <summary>
        /// Adds a tab
        /// </summary>
        public void Indent()
        {
            m_tabs += "\t";
        }
        
        /// <summary>
        /// Removes a tab
        /// </summary>
        public void UnIndent()
        {
            if (m_tabs.Length == 0)
                return;

            m_tabs = m_tabs.Substring(0, m_tabs.Length - 1);
        }

        /// <summary>
        /// Appends the indent tabs
        /// </summary>
        public void AppendIndent()
        {
            m_empty = false;
            m_sb.Append(m_tabs);
        }

        /// <summary>
        /// Appends text
        /// </summary>
        /// <param name="text">The text</param>
        public void Append(string text)
        {
            if(!m_alreadyIndented)
            {
                m_alreadyIndented = true;
                AppendIndent();
            }

            m_empty = false;
            m_sb.Append(text);
        }

        /// <summary>
        /// Appends text and a new line
        /// </summary>
        /// <param name="text">The text</param>
        public void AppendLine(string text)
        {
            if(!m_alreadyIndented)
            {
                m_alreadyIndented = true;
                AppendIndent();
            }

            m_empty = false;
            m_sb.AppendLine(text);
            m_alreadyIndented = false;
        }

        /// <summary>
        /// Append text at the start
        /// </summary>
        /// <param name="text">The text</param>
        /*public void Prepend(string text)
        {
            m_empty = false;
            m_sb.Insert(0, text);
        }*/

        /// <summary>
        /// Checks if the stringbuilder is empty
        /// </summary>
        /// <returns>If it's empty</returns>
        public bool IsEmpty()
        {
            return m_empty;
        }

        /// <summary>
        /// Converts the buffer to a string
        /// </summary>
        /// <returns>The string</returns>
        public override string ToString()
        {
            return m_sb.ToString();
        }
    }
}
