namespace RSA_LW
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Tests were passed: " + TestBigInt());

            Console.WriteLine("Enter source text >>");
            var sourceText = BigInteger.RSAMachine.Encoding(Console.ReadLine());
            var rsaMachine = new BigInteger.RSAMachine();
            var encryptedText = rsaMachine.Encryption(new BigInteger(sourceText, 10));
            Console.WriteLine("Encrypted text: {0}", encryptedText);

            var decodedText = BigInteger.RSAMachine.Decoding(
                rsaMachine
                .Decryption(encryptedText)
                .ToString()
            );
            Console.WriteLine("Decrypted text: {0}", decodedText);
        }

        public static string TestBigInt()
        {
            bool passed = true;
            {
                var bi1 = new BigInteger("151413121110987654321", 10);
                var bi2 = new BigInteger("151413121110987654321", 10);
                var expected = new BigInteger("302826242221975308642", 10);
                var result = bi1 + bi2;
                if (!result.Equals(expected))
                {
                    passed = false;
                    Console.WriteLine("Test 1 failed");
                }
            }
            {
                var bi1 = new BigInteger("41754548797798156409875433186152102228498479694796653351848954185997611243506278579601980980219484255929864359810447039931", 10);
                var bi2 = new BigInteger("41751237506897446098754331861521022284984796947966533518489541859976112435062785796019809802194842559456434523897631", 10);
                var expected = new BigInteger("41754507046560649512429334431820240707476194709999705385315435696455751267393843516816184960409682061087304903375923142300", 10);
                var result = bi1 - bi2;
                if (!result.Equals(expected))
                {
                    passed = false;
                    Console.WriteLine("Test 2 failed");
                }
            }
            {
                var bi1 = new BigInteger("41754548797798156409875433", 10);
                var bi2 = new BigInteger("417512375068", 10);
                var expected = new BigInteger("17433040838461412371558840121854904444", 10);
                var result = bi1 * bi2;
                if (!result.Equals(expected))
                {
                    passed = false;
                    Console.WriteLine("Test 3 failed");
                }
            }
            {
                var bi1 = new BigInteger("17433040838461412371558840121854904444", 10);
                var bi2 = new BigInteger("41754548797798156409875433", 10);
                var expected = new BigInteger("417512375068", 10);
                var result = bi1 / bi2;
                if (!result.Equals(expected))
                {
                    passed = false;
                    Console.WriteLine("Test 4 failed");
                }
            }
            {
                var bi1 = new BigInteger("4175454879779815640987543323453452347763456345689678977", 10);
                var bi2 = new BigInteger("56345645324534568679465435241321857", 10);
                var expected = new BigInteger("29672943768156529020931269722026021", 10);
                var result = bi1 % bi2;
                if (!result.Equals(expected))
                {
                    passed = false;
                    Console.WriteLine("Test 5 failed");
                }
            }
            return passed ? "Yes" : "No";
        }
    }
    public class BigInteger
    {
        private const int maxLength = 100;

        private uint[] data;
        public int length;
        public class RSAMachine
        {
            private BigInteger p;
            private BigInteger q;
            private BigInteger n;
            private BigInteger e;
            private BigInteger d;
            private BigInteger fi;


            public RSAMachine()
            {
                p = new BigInteger("9876432123434639246809386093468324096823049682304968230496823049682349628034968203496820349682304083", 10);
                q = new BigInteger("9876432123434639246809386093468324096823049682304968230496823049682349628034968203496820349682304671", 10);
                fi = (p - 1) * (q - 1);
                n = p * q;
                e = new BigInteger(3);
                d = e.modInverse(fi);
            }

            public BigInteger Encryption(BigInteger P)
            {
                return P.modPow(e, n);
            }

            public BigInteger Decryption(BigInteger E)
            {
                return E.modPow(d, n);
            }

            public static string Encoding(string str)
            {
                string sum;
                char symbol = str[0];
                int symbolCode = symbol;
                string result = symbolCode.ToString();

                for (int i = 1; i < str.Length; i++)
                {
                    symbol = str[i];
                    symbolCode = symbol;
                    sum = symbolCode.ToString();
                    switch (sum.Length)
                    {
                        case 1:
                            sum = "00" + sum;
                            break;
                        case 2:
                            sum = "0" + sum;
                            break;
                    }
                    result += sum;
                }
                return result;
            }

            public static string Decoding(string str)
            {
                string result = "";
                int i = 0;
                string sym;
                switch (str.Length % 3)
                {
                    case 1:
                        i = 1;
                        result = str[0].ToString();
                        result = Convert.ToChar(int.Parse(result)).ToString();
                        break;
                    case 2:
                        i = 2;
                        result = str[0].ToString() + str[1].ToString();
                        result = Convert.ToChar(int.Parse(result)).ToString();
                        break;
                }

                for (; i < str.Length; i = i + 3)
                {
                    sym = str[i].ToString() + str[i + 1].ToString() + str[i + 2].ToString();
                    if (sym[0] == 0)
                    {
                        if (sym[1] == 0)
                        {
                            sym = sym[3].ToString();
                        }
                        else
                        {
                            sym = sym[1..];
                        }
                    }
                    result += Convert.ToChar(int.Parse(sym)).ToString();
                }
                return result;
            }
        }

        public BigInteger()
        {
            data = new uint[maxLength];
            length = 1;
        }

        public BigInteger(long value)
        {
            data = new uint[maxLength];
            long tempVal = value;

            length = 0;
            while (value != 0 && length < maxLength)
            {
                data[length] = (uint)(value & 0xFFFFFFFF);
                value >>= 32;
                length++;
            }

            if (tempVal > 0)
            {
                if (value != 0 || (data[maxLength - 1] & 0x80000000) != 0)
                    throw (new ArithmeticException("Positive overflow in constructor."));
            }
            else if (tempVal < 0)
            {
                if (value != -1 || (data[length - 1] & 0x80000000) == 0)
                    throw (new ArithmeticException("Negative underflow in constructor."));
            }

            if (length == 0)
                length = 1;
        }

        public BigInteger(BigInteger bigInt)
        {
            data = new uint[maxLength];

            length = bigInt.length;

            for (int i = 0; i < length; i++)
                data[i] = bigInt.data[i];
        }


        public BigInteger(string value, int radix)
        {
            BigInteger multiplier = new BigInteger(1);
            BigInteger result = new BigInteger();
            value = value.ToUpper().Trim();
            int limit = 0;

            if (value[0] == '-')
                limit = 1;

            for (int i = value.Length - 1; i >= limit; i--)
            {
                int posVal = (int)value[i];

                if (posVal >= '0' && posVal <= '9')
                    posVal -= '0';
                else if (posVal >= 'A' && posVal <= 'Z')
                    posVal = (posVal - 'A') + 10;
                else
                    posVal = 9999999;


                if (posVal >= radix)
                    throw (new ArithmeticException("Invalid string in constructor."));
                else
                {
                    if (value[0] == '-')
                        posVal = -posVal;

                    result = result + (multiplier * posVal);

                    if ((i - 1) >= limit)
                        multiplier = multiplier * radix;
                }
            }

            if (value[0] == '-')
            {
                if ((result.data[maxLength - 1] & 0x80000000) == 0)
                    throw (new ArithmeticException("Negative underflow in constructor."));
            }
            else
            {
                if ((result.data[maxLength - 1] & 0x80000000) != 0)
                    throw (new ArithmeticException("Positive overflow in constructor."));
            }

            data = new uint[maxLength];
            for (int i = 0; i < result.length; i++)
                data[i] = result.data[i];

            length = result.length;
        }



        public BigInteger(uint[] inData)
        {
            length = inData.Length;

            if (length > maxLength)
                throw (new ArithmeticException("Byte overflow in constructor."));

            data = new uint[maxLength];

            for (int i = length - 1, j = 0; i >= 0; i--, j++)
                data[j] = inData[i];

            while (length > 1 && data[length - 1] == 0)
                length--;
        }

        public static implicit operator BigInteger(long value)
        {
            return (new BigInteger(value));
        }

        public static implicit operator BigInteger(ulong value)
        {
            return (new BigInteger(value));
        }

        public static implicit operator BigInteger(int value)
        {
            return (new BigInteger((long)value));
        }

        public static implicit operator BigInteger(uint value)
        {
            return (new BigInteger((ulong)value));
        }

        public static BigInteger operator +(BigInteger bi1, BigInteger bi2)
        {
            BigInteger result = new BigInteger()
            {
                length = (bi1.length > bi2.length) ? bi1.length : bi2.length
            };

            long carry = 0;
            for (int i = 0; i < result.length; i++)
            {
                long sum = (long)bi1.data[i] + (long)bi2.data[i] + carry;
                carry = sum >> 32;
                result.data[i] = (uint)(sum & 0xFFFFFFFF);
            }

            if (carry != 0 && result.length < maxLength)
            {
                result.data[result.length] = (uint)(carry);
                result.length++;
            }

            while (result.length > 1 && result.data[result.length - 1] == 0)
                result.length--;

            int lastPos = maxLength - 1;
            if ((bi1.data[lastPos] & 0x80000000) == (bi2.data[lastPos] & 0x80000000) &&
               (result.data[lastPos] & 0x80000000) != (bi1.data[lastPos] & 0x80000000))
            {
                throw (new ArithmeticException());
            }

            return result;
        }

        public static BigInteger operator -(BigInteger bi1, BigInteger bi2)
        {
            BigInteger result = new BigInteger()
            {
                length = (bi1.length > bi2.length) ? bi1.length : bi2.length
            };

            long carryIn = 0;
            for (int i = 0; i < result.length; i++)
            {
                long diff;

                diff = (long)bi1.data[i] - (long)bi2.data[i] - carryIn;
                result.data[i] = (uint)(diff & 0xFFFFFFFF);

                if (diff < 0)
                    carryIn = 1;
                else
                    carryIn = 0;
            }

            if (carryIn != 0)
            {
                for (int i = result.length; i < maxLength; i++)
                    result.data[i] = 0xFFFFFFFF;
                result.length = maxLength;
            }

            while (result.length > 1 && result.data[result.length - 1] == 0)
                result.length--;

            int lastPos = maxLength - 1;
            if ((bi1.data[lastPos] & 0x80000000) != (bi2.data[lastPos] & 0x80000000) &&
               (result.data[lastPos] & 0x80000000) != (bi1.data[lastPos] & 0x80000000))
            {
                throw (new ArithmeticException());
            }

            return result;
        }

        public static BigInteger operator *(BigInteger bi1, BigInteger bi2)
        {
            int lastPos = maxLength - 1;
            bool bi1Neg = false, bi2Neg = false;

            try
            {
                if ((bi1.data[lastPos] & 0x80000000) != 0)
                {
                    bi1Neg = true; bi1 = -bi1;
                }
                if ((bi2.data[lastPos] & 0x80000000) != 0)
                {
                    bi2Neg = true; bi2 = -bi2;
                }
            }
            catch (Exception) { }

            BigInteger result = new BigInteger();

            try
            {
                for (int i = 0; i < bi1.length; i++)
                {
                    if (bi1.data[i] == 0) continue;

                    ulong mcarry = 0;
                    for (int j = 0, k = i; j < bi2.length; j++, k++)
                    {
                        ulong val = ((ulong)bi1.data[i] * (ulong)bi2.data[j]) +
                                    (ulong)result.data[k] + mcarry;

                        result.data[k] = (uint)(val & 0xFFFFFFFF);
                        mcarry = (val >> 32);
                    }

                    if (mcarry != 0)
                        result.data[i + bi2.length] = (uint)mcarry;
                }
            }
            catch (Exception)
            {
                throw (new ArithmeticException("Multiplication overflow."));
            }


            result.length = bi1.length + bi2.length;
            if (result.length > maxLength)
                result.length = maxLength;

            while (result.length > 1 && result.data[result.length - 1] == 0)
                result.length--;

            if ((result.data[lastPos] & 0x80000000) != 0)
            {
                if (bi1Neg != bi2Neg && result.data[lastPos] == 0x80000000)
                {
                    if (result.length == 1)
                        return result;
                    else
                    {
                        bool isMaxNeg = true;
                        for (int i = 0; i < result.length - 1 && isMaxNeg; i++)
                        {
                            if (result.data[i] != 0)
                                isMaxNeg = false;
                        }

                        if (isMaxNeg)
                            return result;
                    }
                }

                throw (new ArithmeticException("Multiplication overflow."));
            }

            if (bi1Neg != bi2Neg)
                return -result;

            return result;
        }

        public static BigInteger operator <<(BigInteger bi1, int shiftVal)
        {
            BigInteger result = new BigInteger(bi1);
            result.length = shiftLeft(result.data, shiftVal);

            return result;
        }
        private static int shiftLeft(uint[] buffer, int shiftVal)
        {
            int shiftAmount = 32;
            int bufLen = buffer.Length;

            while (bufLen > 1 && buffer[bufLen - 1] == 0)
                bufLen--;

            for (int count = shiftVal; count > 0;)
            {
                if (count < shiftAmount)
                    shiftAmount = count;

                ulong carry = 0;
                for (int i = 0; i < bufLen; i++)
                {
                    ulong val = ((ulong)buffer[i]) << shiftAmount;
                    val |= carry;

                    buffer[i] = (uint)(val & 0xFFFFFFFF);
                    carry = val >> 32;
                }

                if (carry != 0)
                {
                    if (bufLen + 1 <= buffer.Length)
                    {
                        buffer[bufLen] = (uint)carry;
                        bufLen++;
                    }
                }
                count -= shiftAmount;
            }
            return bufLen;
        }

        public static BigInteger operator >>(BigInteger bi1, int shiftVal)
        {
            BigInteger result = new BigInteger(bi1);
            result.length = shiftRight(result.data, shiftVal);


            if ((bi1.data[maxLength - 1] & 0x80000000) != 0)
            {
                for (int i = maxLength - 1; i >= result.length; i--)
                    result.data[i] = 0xFFFFFFFF;

                uint mask = 0x80000000;
                for (int i = 0; i < 32; i++)
                {
                    if ((result.data[result.length - 1] & mask) != 0)
                        break;

                    result.data[result.length - 1] |= mask;
                    mask >>= 1;
                }
                result.length = maxLength;
            }

            return result;
        }


        private static int shiftRight(uint[] buffer, int shiftVal)
        {
            int shiftAmount = 32;
            int invShift = 0;
            int bufLen = buffer.Length;

            while (bufLen > 1 && buffer[bufLen - 1] == 0)
                bufLen--;

            for (int count = shiftVal; count > 0;)
            {
                if (count < shiftAmount)
                {
                    shiftAmount = count;
                    invShift = 32 - shiftAmount;
                }

                ulong carry = 0;
                for (int i = bufLen - 1; i >= 0; i--)
                {
                    ulong val = ((ulong)buffer[i]) >> shiftAmount;
                    val |= carry;

                    carry = (((ulong)buffer[i]) << invShift) & 0xFFFFFFFF;
                    buffer[i] = (uint)(val);
                }

                count -= shiftAmount;
            }

            while (bufLen > 1 && buffer[bufLen - 1] == 0)
                bufLen--;

            return bufLen;
        }

        public static BigInteger operator -(BigInteger bi1)
        {
            if (bi1.length == 1 && bi1.data[0] == 0)
                return (new BigInteger());

            BigInteger result = new BigInteger(bi1);

            for (int i = 0; i < maxLength; i++)
                result.data[i] = (uint)(~(bi1.data[i]));

            long val, carry = 1;
            int index = 0;

            while (carry != 0 && index < maxLength)
            {
                val = (long)(result.data[index]);
                val++;

                result.data[index] = (uint)(val & 0xFFFFFFFF);
                carry = val >> 32;

                index++;
            }

            if ((bi1.data[maxLength - 1] & 0x80000000) == (result.data[maxLength - 1] & 0x80000000))
                throw (new ArithmeticException("Overflow in negation.\n"));

            result.length = maxLength;

            while (result.length > 1 && result.data[result.length - 1] == 0)
                result.length--;
            return result;
        }

        public static bool operator ==(BigInteger bi1, BigInteger bi2)
        {
            return bi1.Equals(bi2);
        }

        public static bool operator !=(BigInteger bi1, BigInteger bi2)
        {
            return !(bi1.Equals(bi2));
        }

        public override bool Equals(object o)
        {
            BigInteger bi = (BigInteger)o;

            if (this.length != bi.length)
                return false;

            for (int i = 0; i < this.length; i++)
            {
                if (this.data[i] != bi.data[i])
                    return false;
            }
            return true;
        }


        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public static bool operator >(BigInteger bi1, BigInteger bi2)
        {
            int pos = maxLength - 1;

            if ((bi1.data[pos] & 0x80000000) != 0 && (bi2.data[pos] & 0x80000000) == 0)
                return false;

            else if ((bi1.data[pos] & 0x80000000) == 0 && (bi2.data[pos] & 0x80000000) != 0)
                return true;

            int len = (bi1.length > bi2.length) ? bi1.length : bi2.length;
            for (pos = len - 1; pos >= 0 && bi1.data[pos] == bi2.data[pos]; pos--) ;

            if (pos >= 0)
            {
                if (bi1.data[pos] > bi2.data[pos])
                    return true;
                return false;
            }
            return false;
        }

        public static bool operator <(BigInteger bi1, BigInteger bi2)
        {
            int pos = maxLength - 1;

            if ((bi1.data[pos] & 0x80000000) != 0 && (bi2.data[pos] & 0x80000000) == 0)
                return true;

            else if ((bi1.data[pos] & 0x80000000) == 0 && (bi2.data[pos] & 0x80000000) != 0)
                return false;

            int len = (bi1.length > bi2.length) ? bi1.length : bi2.length;
            for (pos = len - 1; pos >= 0 && bi1.data[pos] == bi2.data[pos]; pos--) ;

            if (pos >= 0)
            {
                if (bi1.data[pos] < bi2.data[pos])
                    return true;
                return false;
            }
            return false;
        }

        public static bool operator >=(BigInteger bi1, BigInteger bi2)
        {
            return (bi1 == bi2 || bi1 > bi2);
        }

        public static bool operator <=(BigInteger bi1, BigInteger bi2)
        {
            return (bi1 == bi2 || bi1 < bi2);
        }

        private static void multiByteDivide(BigInteger bi1, BigInteger bi2,
                                            BigInteger outQuotient, BigInteger outRemainder)
        {
            uint[] result = new uint[maxLength];

            int remainderLen = bi1.length + 1;
            uint[] remainder = new uint[remainderLen];

            uint mask = 0x80000000;
            uint val = bi2.data[bi2.length - 1];
            int shift = 0, resultPos = 0;

            while (mask != 0 && (val & mask) == 0)
            {
                shift++; mask >>= 1;
            }

            for (int i = 0; i < bi1.length; i++)
                remainder[i] = bi1.data[i];
            shiftLeft(remainder, shift);
            bi2 = bi2 << shift;

            int j = remainderLen - bi2.length;
            int pos = remainderLen - 1;

            ulong firstDivisorByte = bi2.data[bi2.length - 1];
            ulong secondDivisorByte = bi2.data[bi2.length - 2];

            int divisorLen = bi2.length + 1;
            uint[] dividendPart = new uint[divisorLen];

            while (j > 0)
            {
                ulong dividend = ((ulong)remainder[pos] << 32) + (ulong)remainder[pos - 1];

                ulong q_hat = dividend / firstDivisorByte;
                ulong r_hat = dividend % firstDivisorByte;

                bool done = false;
                while (!done)
                {
                    done = true;

                    if (q_hat == 0x100000000 ||
                       (q_hat * secondDivisorByte) > ((r_hat << 32) + remainder[pos - 2]))
                    {
                        q_hat--;
                        r_hat += firstDivisorByte;

                        if (r_hat < 0x100000000)
                            done = false;
                    }
                }

                for (int h = 0; h < divisorLen; h++)
                    dividendPart[h] = remainder[pos - h];

                BigInteger kk = new BigInteger(dividendPart);
                BigInteger ss = bi2 * (long)q_hat;

                while (ss > kk)
                {
                    q_hat--;
                    ss -= bi2;
                }
                BigInteger yy = kk - ss;

                for (int h = 0; h < divisorLen; h++)
                    remainder[pos - h] = yy.data[bi2.length - h];

                result[resultPos++] = (uint)q_hat;

                pos--;
                j--;
            }

            outQuotient.length = resultPos;
            int y = 0;
            for (int x = outQuotient.length - 1; x >= 0; x--, y++)
                outQuotient.data[y] = result[x];
            for (; y < maxLength; y++)
                outQuotient.data[y] = 0;

            while (outQuotient.length > 1 && outQuotient.data[outQuotient.length - 1] == 0)
                outQuotient.length--;

            if (outQuotient.length == 0)
                outQuotient.length = 1;

            outRemainder.length = shiftRight(remainder, shift);

            for (y = 0; y < outRemainder.length; y++)
                outRemainder.data[y] = remainder[y];
            for (; y < maxLength; y++)
                outRemainder.data[y] = 0;
        }

        private static void singleByteDivide(BigInteger bi1, BigInteger bi2,
                                             BigInteger outQuotient, BigInteger outRemainder)
        {
            uint[] result = new uint[maxLength];
            int resultPos = 0;

            for (int i = 0; i < maxLength; i++)
                outRemainder.data[i] = bi1.data[i];
            outRemainder.length = bi1.length;

            while (outRemainder.length > 1 && outRemainder.data[outRemainder.length - 1] == 0)
                outRemainder.length--;

            ulong divisor = (ulong)bi2.data[0];
            int pos = outRemainder.length - 1;
            ulong dividend = (ulong)outRemainder.data[pos];

            if (dividend >= divisor)
            {
                ulong quotient = dividend / divisor;
                result[resultPos++] = (uint)quotient;

                outRemainder.data[pos] = (uint)(dividend % divisor);
            }
            pos--;

            while (pos >= 0)
            {
                dividend = ((ulong)outRemainder.data[pos + 1] << 32) + (ulong)outRemainder.data[pos];
                ulong quotient = dividend / divisor;
                result[resultPos++] = (uint)quotient;

                outRemainder.data[pos + 1] = 0;
                outRemainder.data[pos--] = (uint)(dividend % divisor);
            }

            outQuotient.length = resultPos;
            int j = 0;
            for (int i = outQuotient.length - 1; i >= 0; i--, j++)
                outQuotient.data[j] = result[i];
            for (; j < maxLength; j++)
                outQuotient.data[j] = 0;

            while (outQuotient.length > 1 && outQuotient.data[outQuotient.length - 1] == 0)
                outQuotient.length--;

            if (outQuotient.length == 0)
                outQuotient.length = 1;

            while (outRemainder.length > 1 && outRemainder.data[outRemainder.length - 1] == 0)
                outRemainder.length--;
        }

        public static BigInteger operator /(BigInteger bi1, BigInteger bi2)
        {
            BigInteger quotient = new BigInteger();
            BigInteger remainder = new BigInteger();

            int lastPos = maxLength - 1;
            bool divisorNeg = false, dividendNeg = false;

            if ((bi1.data[lastPos] & 0x80000000) != 0)
            {
                bi1 = -bi1;
                dividendNeg = true;
            }
            if ((bi2.data[lastPos] & 0x80000000) != 0)
            {
                bi2 = -bi2;
                divisorNeg = true;
            }

            if (bi1 < bi2)
            {
                return quotient;
            }

            else
            {
                if (bi2.length == 1)
                    singleByteDivide(bi1, bi2, quotient, remainder);
                else
                    multiByteDivide(bi1, bi2, quotient, remainder);

                if (dividendNeg != divisorNeg)
                    return -quotient;

                return quotient;
            }
        }

        public static BigInteger operator %(BigInteger bi1, BigInteger bi2)
        {
            BigInteger quotient = new BigInteger();
            BigInteger remainder = new BigInteger(bi1);

            int lastPos = maxLength - 1;
            bool dividendNeg = false;

            if ((bi1.data[lastPos] & 0x80000000) != 0)
            {
                bi1 = -bi1;
                dividendNeg = true;
            }
            if ((bi2.data[lastPos] & 0x80000000) != 0)
                bi2 = -bi2;

            if (bi1 < bi2)
            {
                return remainder;
            }

            else
            {
                if (bi2.length == 1)
                    singleByteDivide(bi1, bi2, quotient, remainder);
                else
                    multiByteDivide(bi1, bi2, quotient, remainder);

                if (dividendNeg)
                    return -remainder;

                return remainder;
            }
        }

        public static BigInteger operator ^(BigInteger bi1, BigInteger bi2)
        {
            BigInteger result = new BigInteger();

            int len = (bi1.length > bi2.length) ? bi1.length : bi2.length;

            for (int i = 0; i < len; i++)
            {
                uint sum = (uint)(bi1.data[i] ^ bi2.data[i]);
                result.data[i] = sum;
            }

            result.length = maxLength;

            while (result.length > 1 && result.data[result.length - 1] == 0)
                result.length--;

            return result;
        }

        public override string ToString()
        {
            return ToString(10);
        }

        public string ToString(int radix)
        {
            if (radix < 2 || radix > 36)
                throw (new ArgumentException("Radix must be >= 2 and <= 36"));

            string charSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string result = "";

            BigInteger a = this;

            bool negative = false;
            if ((a.data[maxLength - 1] & 0x80000000) != 0)
            {
                negative = true;
                try
                {
                    a = -a;
                }
                catch (Exception) { }
            }

            BigInteger quotient = new BigInteger();
            BigInteger remainder = new BigInteger();
            BigInteger biRadix = new BigInteger(radix);

            if (a.length == 1 && a.data[0] == 0)
                result = "0";
            else
            {
                while (a.length > 1 || (a.length == 1 && a.data[0] != 0))
                {
                    singleByteDivide(a, biRadix, quotient, remainder);

                    if (remainder.data[0] < 10)
                        result = remainder.data[0] + result;
                    else
                        result = charSet[(int)remainder.data[0] - 10] + result;

                    a = quotient;
                }
                if (negative)
                    result = "-" + result;
            }

            return result;
        }

        public BigInteger modPow(BigInteger exp, BigInteger n)
        {
            if ((exp.data[maxLength - 1] & 0x80000000) != 0)
                throw (new ArithmeticException("Positive exponents only."));

            BigInteger resultNum = 1;
            BigInteger tempNum;
            bool thisNegative = false;

            if ((this.data[maxLength - 1] & 0x80000000) != 0)
            {
                tempNum = -this % n;
                thisNegative = true;
            }
            else
                tempNum = this % n;

            if ((n.data[maxLength - 1] & 0x80000000) != 0)
                n = -n;

            BigInteger constant = new BigInteger();

            int i = n.length << 1;
            constant.data[i] = 0x00000001;
            constant.length = i + 1;

            constant = constant / n;
            int totalBits = exp.bitCount();
            int count = 0;

            for (int pos = 0; pos < exp.length; pos++)
            {
                uint mask = 0x01;

                for (int index = 0; index < 32; index++)
                {
                    if ((exp.data[pos] & mask) != 0)
                        resultNum = BarrettReduction(resultNum * tempNum, n, constant);

                    mask <<= 1;

                    tempNum = BarrettReduction(tempNum * tempNum, n, constant);


                    if (tempNum.length == 1 && tempNum.data[0] == 1)
                    {
                        if (thisNegative && (exp.data[0] & 0x1) != 0)
                            return -resultNum;
                        return resultNum;
                    }
                    count++;
                    if (count == totalBits)
                        break;
                }
            }

            if (thisNegative && (exp.data[0] & 0x1) != 0)
                return -resultNum;

            return resultNum;
        }

        private BigInteger BarrettReduction(BigInteger x, BigInteger n, BigInteger constant)
        {
            int k = n.length,
                kPlusOne = k + 1,
                kMinusOne = k - 1;

            BigInteger q1 = new BigInteger();

            for (int i = kMinusOne, j = 0; i < x.length; i++, j++)
                q1.data[j] = x.data[i];
            q1.length = x.length - kMinusOne;
            if (q1.length <= 0)
                q1.length = 1;


            BigInteger q2 = q1 * constant;
            BigInteger q3 = new BigInteger();

            for (int i = kPlusOne, j = 0; i < q2.length; i++, j++)
                q3.data[j] = q2.data[i];
            q3.length = q2.length - kPlusOne;
            if (q3.length <= 0)
                q3.length = 1;

            BigInteger r1 = new BigInteger();
            int lengthToCopy = (x.length > kPlusOne) ? kPlusOne : x.length;
            for (int i = 0; i < lengthToCopy; i++)
                r1.data[i] = x.data[i];
            r1.length = lengthToCopy;

            BigInteger r2 = new BigInteger();
            for (int i = 0; i < q3.length; i++)
            {
                if (q3.data[i] == 0) continue;

                ulong mcarry = 0;
                int t = i;
                for (int j = 0; j < n.length && t < kPlusOne; j++, t++)
                {
                    ulong val = ((ulong)q3.data[i] * (ulong)n.data[j]) +
                                 (ulong)r2.data[t] + mcarry;

                    r2.data[t] = (uint)(val & 0xFFFFFFFF);
                    mcarry = (val >> 32);
                }

                if (t < kPlusOne)
                    r2.data[t] = (uint)mcarry;
            }
            r2.length = kPlusOne;
            while (r2.length > 1 && r2.data[r2.length - 1] == 0)
                r2.length--;

            r1 -= r2;
            if ((r1.data[maxLength - 1] & 0x80000000) != 0)
            {
                BigInteger val = new BigInteger();
                val.data[kPlusOne] = 0x00000001;
                val.length = kPlusOne + 1;
                r1 += val;
            }

            while (r1 >= n)
                r1 -= n;

            return r1;
        }

        public int bitCount()
        {
            while (length > 1 && data[length - 1] == 0)
                length--;

            uint value = data[length - 1];
            uint mask = 0x80000000;
            int bits = 32;

            while (bits > 0 && (value & mask) == 0)
            {
                bits--;
                mask >>= 1;
            }
            bits += ((length - 1) << 5);

            return bits == 0 ? 1 : bits;
        }

        public BigInteger modInverse(BigInteger modulus)
        {
            BigInteger[] p = { 0, 1 };
            BigInteger[] q = new BigInteger[2];
            BigInteger[] r = { 0, 0 };

            int step = 0;

            BigInteger a = modulus;
            BigInteger b = this;

            while (b.length > 1 || (b.length == 1 && b.data[0] != 0))
            {
                BigInteger quotient = new BigInteger();
                BigInteger remainder = new BigInteger();

                if (step > 1)
                {
                    BigInteger pval = (p[0] - (p[1] * q[0])) % modulus;
                    p[0] = p[1];
                    p[1] = pval;
                }

                if (b.length == 1)
                    singleByteDivide(a, b, quotient, remainder);
                else
                    multiByteDivide(a, b, quotient, remainder);

                q[0] = q[1];
                r[0] = r[1];
                q[1] = quotient; r[1] = remainder;

                a = b;
                b = remainder;

                step++;
            }
            if (r[0].length > 1 || (r[0].length == 1 && r[0].data[0] != 1))
                throw (new ArithmeticException("No inverse!"));

            BigInteger result = ((p[0] - (p[1] * q[0])) % modulus);

            if ((result.data[maxLength - 1] & 0x80000000) != 0)
                result += modulus;

            return result;
        }

    }
}