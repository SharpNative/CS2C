using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS_2_C
{
    public class FormattedStringBuilder
    {
        private StringBuilder m_sb;
        private string m_tabs;

        /// <summary>
        /// Formatted string builder
        /// </summary>
        public FormattedStringBuilder()
        {
            m_sb = new StringBuilder();
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
            m_sb.Append(m_tabs);
        }

        /// <summary>
        /// Appends text
        /// </summary>
        /// <param name="text">The text</param>
        public void Append(string text)
        {
            m_sb.Append(text);
        }

        /// <summary>
        /// Appends text and a new line
        /// </summary>
        /// <param name="text">The text</param>
        public void AppendLine(string text)
        {
            m_sb.AppendLine(text);
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
