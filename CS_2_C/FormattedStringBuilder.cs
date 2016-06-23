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

        public FormattedStringBuilder()
        {
            m_sb = new StringBuilder();
            m_tabs = "";
        }

        public void Indent()
        {
            m_tabs += "\t";
        }

        public void UnIndent()
        {
            if (m_tabs.Length == 0)
                return;

            m_tabs = m_tabs.Substring(0, m_tabs.Length - 1);
        }

        public void AppendIndent()
        {
            m_sb.Append(m_tabs);
        }

        public void Append(string text)
        {
            m_sb.Append(text);
        }

        public void AppendLine(string text)
        {
            m_sb.AppendLine(text);
        }

        public override string ToString()
        {
            return m_sb.ToString();
        }
    }
}
