using System;

namespace BinaryAnalysis.Helpers.Network
{
    public static class GooglePageRank
    {
        private const uint GOOGLE_MAGIC = 0xE6359A60;

        private static uint zeroFill(uint a, int b)
        {
            checked
            {
                uint z = 0x80000000;
                if (Convert.ToBoolean(z & a))
                {
                    a = (a >> 1);
                    a &= (~z);
                    a |= 0x40000000;
                    a = (a >> (b - 1));
                }
                else
                {
                    a = (a >> b);
                }
            }
            return a;
        }

        private static uint[] mix(uint a, uint b, uint c)
        {
            unchecked
            {
                a -= b; a -= c; a ^= (uint)(zeroFill(c, 13));
                b -= c; b -= a; b ^= (uint)(a << 8);
                c -= a; c -= b; c ^= (uint)(zeroFill(b, 13));
                a -= b; a -= c; a ^= (uint)(zeroFill(c, 12));
                b -= c; b -= a; b ^= (uint)(a << 16);
                c -= a; c -= b; c ^= (uint)(zeroFill(b, 5));
                a -= b; a -= c; a ^= (uint)(zeroFill(c, 3));
                b -= c; b -= a; b ^= (uint)(a << 10);
                c -= a; c -= b; c ^= (uint)(zeroFill(b, 15));

                return new uint[3] { a, b, c };
            }
        }

        private static uint GoogleCH(uint[] url, uint length, uint init)
        {
            unchecked
            {
                if (length == 0)
                {
                    length = (uint)url.Length;
                }
                uint a, b;
                a = b = 0x9E3779B9;
                uint c = init;
                int k = 0;
                uint len = length;
                uint[] m_mix = new uint[3];
                while (len >= 12)
                {
                    a += (uint)(url[k + 0] + (url[k + 1] << 8) + (url[k + 2] << 16) + (url[k + 3] << 24));

                    b += (uint)(url[k + 4] + (url[k + 5] << 8) + (url[k + 6] << 16) + (url[k + 7] << 24));
                    c += (uint)(url[k + 8] + (url[k + 9] << 8) + (url[k + 10] << 16) + (url[k + 11] << 24));
                    m_mix = mix(a, b, c);
                    a = m_mix[0]; b = m_mix[1]; c = m_mix[2];

                    k += 12;
                    len -= 12;
                }

                c += length;

                switch (len)              /* all the case statements fall through */
                {
                    case 11:
                        {
                            c += (uint)(url[k + 10] << 24);
                            c += (uint)(url[k + 9] << 16);
                            c += (uint)(url[k + 8] << 8);
                            b += (uint)(url[k + 7] << 24);
                            b += (uint)(url[k + 6] << 16);
                            b += (uint)(url[k + 5] << 8);
                            b += (uint)(url[k + 4]);
                            a += (uint)(url[k + 3] << 24);
                            a += (uint)(url[k + 2] << 16);
                            a += (uint)(url[k + 1] << 8);
                            a += (uint)(url[k + 0]);
                            break;
                        }
                    case 10:
                        {
                            c += (uint)(url[k + 9] << 16);
                            c += (uint)(url[k + 8] << 8);
                            b += (uint)(url[k + 7] << 24);
                            b += (uint)(url[k + 6] << 16);
                            b += (uint)(url[k + 5] << 8);
                            b += (uint)(url[k + 4]);
                            a += (uint)(url[k + 3] << 24);
                            a += (uint)(url[k + 2] << 16);
                            a += (uint)(url[k + 1] << 8);
                            a += (uint)(url[k + 0]);
                            break;
                        }
                    case 9:
                        {
                            c += (uint)(url[k + 8] << 8);
                            b += (uint)(url[k + 7] << 24);
                            b += (uint)(url[k + 6] << 16);
                            b += (uint)(url[k + 5] << 8);
                            b += (uint)(url[k + 4]);
                            a += (uint)(url[k + 3] << 24);
                            a += (uint)(url[k + 2] << 16);
                            a += (uint)(url[k + 1] << 8);
                            a += (uint)(url[k + 0]);
                            break;
                        }
                    /* the first byte of c is reserved for the length */
                    case 8:
                        {
                            b += (uint)(url[k + 7] << 24);
                            b += (uint)(url[k + 6] << 16);
                            b += (uint)(url[k + 5] << 8);
                            b += (uint)(url[k + 4]);
                            a += (uint)(url[k + 3] << 24);
                            a += (uint)(url[k + 2] << 16);
                            a += (uint)(url[k + 1] << 8);
                            a += (uint)(url[k + 0]);
                            break;
                        }
                    case 7:
                        {
                            b += (uint)(url[k + 6] << 16);
                            b += (uint)(url[k + 5] << 8);
                            b += (uint)(url[k + 4]);
                            a += (uint)(url[k + 3] << 24);
                            a += (uint)(url[k + 2] << 16);
                            a += (uint)(url[k + 1] << 8);
                            a += (uint)(url[k + 0]);
                            break;
                        }
                    case 6:
                        {
                            b += (uint)(url[k + 4]);
                            a += (uint)(url[k + 3] << 24);
                            a += (uint)(url[k + 2] << 16);
                            a += (uint)(url[k + 1] << 8);
                            a += (uint)(url[k + 0]);
                            break;
                        }
                    case 5:
                        {
                            b += (uint)(url[k + 4]);
                            a += (uint)(url[k + 3] << 24);
                            a += (uint)(url[k + 2] << 16);
                            a += (uint)(url[k + 1] << 8);
                            a += (uint)(url[k + 0]);
                            break;
                        }
                    case 4:
                        {
                            a += (uint)(url[k + 3] << 24);
                            a += (uint)(url[k + 2] << 16);
                            a += (uint)(url[k + 1] << 8);
                            a += (uint)(url[k + 0]);
                            break;
                        }
                    case 3:
                        {
                            a += (uint)(url[k + 2] << 16);
                            a += (uint)(url[k + 1] << 8);
                            a += (uint)(url[k + 0]);
                            break;
                        }
                    case 2:
                        {
                            a += (uint)(url[k + 1] << 8);
                            a += (uint)(url[k + 0]);
                            break;
                        }
                    case 1:
                        {
                            a += (uint)(url[k + 0]);
                            break;
                        }
                    /* case 0: nothing left to add */
                }
                m_mix = mix(a, b, c);
                /*-------------------------------------------- report the result */
                return m_mix[2];
            }
        }

        private static uint GoogleCH(string url, uint length)
        {
            uint[] m_urluint = new uint[url.Length];
            for (int i = 0; i < url.Length; i++)
            {
                m_urluint[i] = url[i];
            }
            return GoogleCH(m_urluint, length, GOOGLE_MAGIC);
        }

        private static uint GoogleCH(string sURL)
        {
            return GoogleCH(sURL, 0);
        }

        private static uint GoogleCH(uint[] url, uint length)
        {
            return GoogleCH(url, length, GOOGLE_MAGIC);
        }

        private static uint[] c32to8bit(uint[] arr32)
        {
            uint[] arr8 = new uint[arr32.GetLength(0) * 4 + 3];

            for (int i = 0; i < arr32.GetLength(0); i++)
            {
                for (int bitOrder = i * 4; bitOrder <= i * 4 + 3; bitOrder++)
                {
                    arr8[bitOrder] = arr32[i] & 255;
                    arr32[i] = zeroFill(arr32[i], 8);
                }
            }
            return arr8;
        }

        //???,ToolBar ??>>=2.0.114
        public static string CalculateChecksum(string sURL)
        {
            uint ch = GoogleCH("info:" + sURL);

            ch = (((ch / 7) << 2) | (((uint)(ch % 13)) & 7));

            uint[] prbuf = new uint[20];
            prbuf[0] = ch;
            for (int i = 1; i < 20; i++)
            {
                prbuf[i] = prbuf[i - 1] - 9;
            }
            ch = GoogleCH(c32to8bit(prbuf), 80);

            return string.Format("6{0}", ch);
        }

        //???,ToolBar ??<2.0.114
        public static string CalculateChecksumOld(string sURL)
        {
            uint ch = GoogleCH("info:" + sURL);

            string CalculateChecksum = "6" + Convert.ToString((ch));
            return CalculateChecksum;
        }
    }
}        

