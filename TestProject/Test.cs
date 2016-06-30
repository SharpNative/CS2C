using System.Runtime.InteropServices;

namespace Sharpen
{
    class GDT
    {
        // GDT entry in table
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct GDT_Entry
        {
            public ushort limit_lo;
            public ushort base_lo;
            public byte base_mid;
            public byte access;
            public byte granularity;
            public byte base_hi;
        }

        // GDT pointer
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct GDT_Pointer
        {
            public ushort limit;
            public uint base_address;
        }

        // Data selector constants
        enum GDT_Data
        {
            R = 0x00,       // Read only
            RA = 0x01,      // Read only, accessed
            RW = 0x02,      // Read, write
            RWA = 0x03,     // Read, write, accessed
            RED = 0x04,     // Read only, expand down
            REDA = 0x05,    // Read only, expand down, accessed
            RWED = 0x06,    // Read, write, expand down
            RWEDA = 0x07,   // Read, write, expand down, accessed
            E = 0x08,       // Execute only
            EA = 0x09,      // Execute, accessed
            ER = 0x0A,      // Execute, read
            ERA = 0x0B,     // Execute, read, accessed
            EC = 0x0C,      // Execute, conforming
            ECA = 0x0D,     // Execute, conforming, accessed
            ERC = 0x0E,     // Execute, read, conforming
            ERCA = 0x0F     // Execute, read, conforming, accessed
        };

        private static GDT_Entry[] m_entries;
        private static GDT_Pointer m_ptr;

        /// <summary>
        /// Converts descriptor type
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        private static int DescriptorType(int a)
        {
            return (a << 0x04);
        }

        /// <summary>
        /// Converts privilege level
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        private static int Privilege(int a)
        {
            return (a << 0x05);
        }

        /// <summary>
        /// Converts present
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        private static int Present(int a)
        {
            return (a << 0x07);
        }

        /// <summary>
        /// Converts available
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        private static int Available(int a)
        {
            return (a << 0x0C);
        }

        /// <summary>
        /// Converts size
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        private static int Size(int a)
        {
            return (a << 0x06);
        }

        /// <summary>
        /// Converts granularity
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        private static int Granularity(int a)
        {
            return (a << 0x07);
        }

        /// <summary>
        /// Sets a GDT entry in the table
        /// </summary>
        /// <param name="num">The entry number</param>
        /// <param name="base_address">The base address</param>
        /// <param name="limit">The limit</param>
        /// <param name="access">The access type</param>
        /// <param name="granularity">Granularity</param>
        public static void SetEntry(int num, ulong base_address, ulong limit, byte access, byte granularity)
        {
            // Address
            m_entries[num].base_lo = (ushort)(base_address & 0xFFFF);
            m_entries[num].base_mid = (byte)((base_address >> 16) & 0xFF);
            m_entries[num].base_hi = (byte)((base_address >> 24) & 0xFF);

            // Limit
            m_entries[num].limit_lo = (ushort)(limit & 0xFFFF);
            m_entries[num].granularity = (byte)((limit >> 16) & 0xFF);

            // Set access and granularity
            m_entries[num].granularity |= (byte)(granularity & 0xF0);
            m_entries[num].access = access;
        }

        /// <summary>
        /// Initializes the GDT
        /// </summary>
        public static unsafe void Init()
        {
            // Allocate data
            m_entries = new GDT_Entry[3];
            m_ptr = new GDT_Pointer();

            // Set GDT table pointer
            m_ptr.limit = (ushort)((3 * sizeof(GDT_Entry)) - 1);
            fixed (GDT_Entry* ptr = m_entries)
            {
                m_ptr.base_address = (uint)ptr;
            }

            // NULL segment
            SetEntry(0, 0, 0, 0, 0);

            // Kernel code segment
            SetEntry(1, 0, 0xFFFFFFFF, (byte)((int)GDT_Data.ER | DescriptorType(1) | Privilege(0) | Present(1)), (byte)(Size(1) | Granularity(1)));

            // Kernel data segment
            SetEntry(2, 0, 0xFFFFFFFF, (byte)((int)GDT_Data.RW | DescriptorType(1) | Privilege(0) | Present(1)), (byte)(Size(1) | Granularity(1)));

            // Flush GDT
            fixed (GDT_Pointer* ptr = &m_ptr)
            {
                FlushGDT(ptr);
            }

            Console.WriteLine("GDT Installed");
        }

        /// <summary>
        /// Flushes the GDT table
        /// </summary>
        /// <param name="ptr">The pointer to the table</param>
        private static extern unsafe void FlushGDT(GDT_Pointer* ptr);
    }
}
