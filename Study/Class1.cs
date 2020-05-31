using System.Text.RegularExpressions;
using NUnit.Framework;

/// <summary>
/// C#의 정규식은 @버바팀을 씀으로서 /? 등을 ?로 퉁칠 수 있어 편하다...
/// 
/// </summary>
namespace Study
{
    public class Class1
    {
        [Test]
        public void FirstStep_UseQuestionMark_OneOrZeroCharacterMatch()
        {
            Assert.IsTrue(Regex.IsMatch("a", @"a"));
            Assert.IsTrue(Regex.IsMatch("color", @"colou?r")); //u? 에서 u는 있어도 좋고 없어도 좋고이다. 단 1회만.
            Assert.IsTrue(Regex.IsMatch("colour", @"colou?r")); // 없는 경우.
            Assert.IsFalse(Regex.IsMatch("colouur", @"colou?r")); // 있지만 2개 있어서 실패!

            Assert.IsTrue(Regex.IsMatch("colo1r", @"colo1?r")); // 있지만 2개 있어서 실패!
        }

        //match로 매칭결과를 상세히 볼 수 있다. 단 여러개 매칭하지는 못함.
        [Test]
        public void Match_ReturnResultObject()
        {
            var m = Regex.Match("i have color and colour", @"colo?r");
            Assert.True(m.Success);
            Assert.AreEqual(7, m.Index); //처음으로 매칭된 인덱스를 반환.
            Assert.AreEqual(5, m.Length); //매칭된 길이 반환.
            Assert.AreEqual("color", m.Value); //매칭된 문자 반환.
        }

        //matchs로 매칭결과를 상세히 볼 수 있다. 여러개 매칭할 수 있으므로, 어쩌면 이게 표준? 일지도?
        [Test]
        public void Matches_ReturnResultObjects()
        {
            var ms = Regex.Matches("i have color and colour", @"colou?r");
            Assert.AreEqual(2, ms.Count); // 당연히 2개 들어있겠지?
            Assert.AreEqual("color", ms[0].Value);
            Assert.AreEqual("colour", ms[1].Value);
        }

        //다음 결과를 보고 싶으면 NextMatch를 쓸 것
        [Test]
        public void NextMatch_ReturnResultObject()
        {
            var m = Regex.Match("i have color and colour", @"colou?r");
            var nextM = m.NextMatch();
            Assert.True(nextM.Success);
            Assert.AreEqual(17, nextM.Index); //처음으로 매칭된 인덱스를 반환.
            Assert.AreEqual(6, nextM.Length); //매칭된 길이 반환.
            Assert.AreEqual("colour", nextM.Value); //매칭된 문자 반환.
        }

        //or 연산자를 써서 부분을 여러가지 중 하나임을 뽑아낼 수 있다.
        [Test]
        public void UseOf_Or()
        {
            //이 경우 ? 를 써서 ()? 로, () 안이 한번이라도 있는지를 체크한다. 없어도 됨...
            var ms = Regex.Matches("i have color and colour", @"colo(r|ur)?");
            Assert.AreEqual("color", ms[0].Value);
            Assert.AreEqual("colour", ms[1].Value);

            //이렇게 없어도 되므로 문제가 있다. 반드시 둘 중 하나길 바랄때는 어떻게 해야 할까? (그건 차차 나오겠지)
            Assert.AreEqual(true, Regex.IsMatch("colo", @"colo(r|ur)?"));
        }

        #region RegexOptions

        [Test]
        public void CaseSensitive()
        {
            Assert.AreEqual(false, Regex.IsMatch("A", @"a")); // 대소문자가 다르니까 안된다.
            Assert.AreEqual(true, Regex.IsMatch("A", @"a", RegexOptions.IgnoreCase)); // 대소문자를 무시하고 찾을 수 있다.

            var match = Regex.Match("A", @"a", RegexOptions.IgnoreCase);
            Assert.AreEqual("A", match.Value); // 무시하고 찾았어도, 찾은 값 자체는 불변이다. (뭐 당연한 거긴 하지...)
        }

        [Test]
        public void CaseSensitive_IgnoreCulture()
        {
            //문화권에 관계없는 대소문자 무시도 가능하다.
            //근데 인자에 | 를 넣는건 대체 무슨 문법이지??? -> 비트플래그였다!!!
            Assert.AreEqual(true, Regex.IsMatch("A", @"a", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)); // 대소문자가 다르니까 안된다.
        }

        [Test]
        public void CanBeShort_RegexOptions()
        {
            Assert.AreEqual(true, Regex.IsMatch("A", @"(?i)a")); //대소문자 무시. Ignore
            Assert.AreEqual(true, Regex.IsMatch("A", @"(?s)A")); // 한줄만. SingleLine

            Assert.AreEqual(true, Regex.IsMatch("A", @"(?m)A")); // 멀티라인. MultiLine
            Assert.AreEqual(true, Regex.IsMatch("B\r\nA", @"(?m)A")); // 멀티라인과 싱글라인 차이를 잘 모르겠네...
            Assert.AreEqual(true, Regex.IsMatch("BA", @"(?m)A")); //

            //멀티라인과 싱글라인이 정확히 뭘 하는건지 모르겠다. 차차 알게 되겠지...
            Assert.AreEqual(true, Regex.IsMatch("B\r\nA", @"(?s)A")); // 싱글라인에서도 줄바꿈후의 A를 잘 찾고...

            //문자열의 끝에서부터 역으로 검색할 수도 있다. 그렇다고 해서 문자열이 뒤집히는 것은 아니다. 단지 우선순위를 뒤에 준다는 것 뿐임. 
            //문자열을 '뒤집지 않는다' 라는 점이 의외로 좋은 해결책일 수도 있다. 왜냐면 문자열 자체를 뒤집으면 찾는 문자도 뒤집혀야 하기 때문이다. 
            Assert.AreEqual("3", Regex.Match("123", @".", RegexOptions.RightToLeft).Value);
        }

        /*
         * 참고. RegexOptions Enums
            namespace System.Text.RegularExpressions
            {
              /// <summary>Provides enumerated values to use to set regular expression options.</summary>
              [Flags]
              [__DynamicallyInvokable]
              public enum RegexOptions
              {
                /// <summary>Specifies that no options are set. For more information about the default behavior of the regular expression engine, see the "Default Options" section in the Regular Expression Options topic. </summary>
                [__DynamicallyInvokable] None = 0,
                /// <summary>Specifies case-insensitive matching. For more information, see the "Case-Insensitive Matching " section in the Regular Expression Options topic. </summary>
                [__DynamicallyInvokable] IgnoreCase = 1,
                /// <summary>Multiline mode. Changes the meaning of ^ and $ so they match at the beginning and end, respectively, of any line, and not just the beginning and end of the entire string. For more information, see the "Multiline Mode" section in the Regular Expression Options topic. </summary>
                [__DynamicallyInvokable] Multiline = 2,
                /// <summary>Specifies that the only valid captures are explicitly named or numbered groups of the form (?&lt;name&gt;…). This allows unnamed parentheses to act as noncapturing groups without the syntactic clumsiness of the expression (?:…). For more information, see the "Explicit Captures Only" section in the Regular Expression Options topic. </summary>
                [__DynamicallyInvokable] ExplicitCapture = 4,
                /// <summary>Specifies that the regular expression is compiled to an assembly. This yields faster execution but increases startup time. This value should not be assigned to the <see cref="P:System.Text.RegularExpressions.RegexCompilationInfo.Options" /> property when calling the <see cref="M:System.Text.RegularExpressions.Regex.CompileToAssembly(System.Text.RegularExpressions.RegexCompilationInfo[],System.Reflection.AssemblyName)" /> method. For more information, see the "Compiled Regular Expressions" section in the Regular Expression Options topic. </summary>
                [__DynamicallyInvokable] Compiled = 8,
                /// <summary>Specifies single-line mode. Changes the meaning of the dot (.) so it matches every character (instead of every character except \n). For more information, see the "Single-line Mode" section in the Regular Expression Options topic. </summary>
                [__DynamicallyInvokable] Singleline = 16, // 0x00000010
                /// <summary>Eliminates unescaped white space from the pattern and enables comments marked with #. However, this value does not affect or eliminate white space in , numeric , or tokens that mark the beginning of individual . For more information, see the "Ignore White Space" section of the Regular Expression Options topic. </summary>
                [__DynamicallyInvokable] IgnorePatternWhitespace = 32, // 0x00000020
                /// <summary>Specifies that the search will be from right to left instead of from left to right. For more information, see the "Right-to-Left Mode" section in the Regular Expression Options topic. </summary>
                [__DynamicallyInvokable] RightToLeft = 64, // 0x00000040
                /// <summary>Enables ECMAScript-compliant behavior for the expression. This value can be used only in conjunction with the <see cref="F:System.Text.RegularExpressions.RegexOptions.IgnoreCase" />, <see cref="F:System.Text.RegularExpressions.RegexOptions.Multiline" />, and <see cref="F:System.Text.RegularExpressions.RegexOptions.Compiled" /> values. The use of this value with any other values results in an exception.For more information on the <see cref="F:System.Text.RegularExpressions.RegexOptions.ECMAScript" /> option, see the "ECMAScript Matching Behavior" section in the Regular Expression Options topic. </summary>
                [__DynamicallyInvokable] ECMAScript = 256, // 0x00000100
                /// <summary>Specifies that cultural differences in language is ignored. For more information, see the "Comparison Using the Invariant Culture" section in the Regular Expression Options topic.</summary>
                [__DynamicallyInvokable] CultureInvariant = 512, // 0x00000200
              }
            }
         */

        #endregion

        #region 문자집합

        /// []를 이용하면 그 안의 '1개' 와 부합한다. (그 안에는 범위를 넣을 수 있다)
        /// \d 로 십진숫자 하나와 부합가능. 2자릿수는 안됨.
        /// \w 로 단어의 문자하나와 부합가능. 단 이것은 해당문화권의 특성을 따름. 영어에서는 [a-zA-Z_0-9] 와 같다. (뭔소리야?)
        /// \s 공백문자 하나와 같음. [\n\r\t\f\v] 와 동일
        /// \p{범주} 로 지정된 범주의 '한글자' 와 부합한다.
        /// . 점을 찍으면 \n 을 제외한 모든 문자와 부합함.
        [Test]
        public void Test_문자집합()
        {
            Assert.AreEqual(true, Regex.IsMatch("that", @"[tT]hat"));
            Assert.AreEqual(true, Regex.IsMatch("That", @"[tT]hat"));

            // ? 와 다른 점은, ?는 있어도 없어도 되지만 []는 반드시 있어야 함.
            Assert.AreEqual(false, Regex.IsMatch("hat", @"[tT]hat"));

            //범위지정 가능
            Assert.AreEqual(true, Regex.IsMatch("1", @"[0-9]"));

            //숫자는 숫자로 처리도 가능.
            Assert.AreEqual(true, Regex.IsMatch("1", @"\d"));
            //두자릿수의 숫자라면 이렇게도 가능. 
            Assert.AreEqual(true, Regex.IsMatch("11", @"\d\d"));
            Assert.AreEqual(true, Regex.IsMatch("00", @"\d\d"));
            Assert.AreEqual(true, Regex.IsMatch("99", @"\d\d"));

            //숫자가 2개 있어야 하는데 하나만 있으므로 거짓
            Assert.AreEqual(false, Regex.IsMatch("1", @"\d\d"));

            //단 숫자가 더 많을 경우는 상관없음. 정규식이 좀 그런게, 앞에만 맞으면 걍 반환하니까 그럼.
            Assert.AreEqual(true, Regex.IsMatch("100", @"\d\d"));
            Assert.AreEqual("10", Regex.Match("100", @"\d\d").Value); //결과를 출력해보면, 자명하다.

            // [숫자-숫자]는 반드시 숫자 첫자리만 체크함. 그 뒤의 숫자들에 대해서는 무시되는것에 주의. 헷갈리기 좋다.
            //놀랍게도 이게 됨. 이거야 그렇다 치고
            Assert.AreEqual(true, Regex.IsMatch("11", @"[1-10]"));
            //왜 이게 되지??? 아무래도 잘못 쓰고 있는 것 같은데. 그냥 첫글자만 쓰는 게 아닐까?
            Assert.AreEqual(true, Regex.IsMatch("1", @"[1-12]"));
            Assert.AreEqual(true, Regex.IsMatch("10", @"[11-12]")); //1은 1-1 사이에 있으므로 참
            Assert.AreEqual(true, Regex.IsMatch("30", @"[0-9]")); //3은 0-9 사이에 있으므로 참
            Assert.AreEqual(true, Regex.IsMatch("30", @"[1-3]")); //3은 1-2 안에 있으므로 참
            Assert.AreEqual(false, Regex.IsMatch("30", @"[4-5]")); //3은 4-52 안에 없으므로 거짓

            //출력해보면 자명하다. []안의 범위가 두자릿수 이상이라도 한자리만 계산함.
            Assert.AreEqual("1", Regex.Match("11", @"[0-9]").Value);
            Assert.AreEqual("1", Regex.Match("11", @"[0-99]").Value);
        }

        [Test]
        public void CheckKorean()
        {
            //한글도 받을 수 있다. 
            Assert.AreEqual("철", Regex.Match("철수", @"\w").Value);
            Assert.AreEqual("ㅊ", Regex.Match("ㅊㅋ", @"\w").Value);

            //한글만 받아내기. 상용구니까 외워버려라. (단 아래 정규식들은 1글자 만 판정하고 있음을 잊지말것)
            Assert.AreEqual(true, Regex.IsMatch("ㅊㅋ", @"[ㄱ-ㅎ]"));
            Assert.AreEqual(true, Regex.IsMatch("ㅗㅜㅑ", @"[ㅏ-ㅣ]"));
            Assert.AreEqual(true, Regex.IsMatch("철수", @"[가-힣]"));
        }

        [Test]
        public void CheckSpace()
        {
            //빈칸일 것
            Assert.AreEqual(true, Regex.IsMatch(" ", @"\s"));
            Assert.AreEqual(false, Regex.IsMatch("a", @"\s"));

            //빈칸일것의 역. 대문자가 된다.
            Assert.AreEqual(false, Regex.IsMatch(" ", @"\S"));
            Assert.AreEqual(true, Regex.IsMatch("a", @"\S"));
        }

        [Test]
        public void Check_Digit()
        {
            Assert.AreEqual(true, Regex.IsMatch("1", @"\d"));
            Assert.AreEqual(false, Regex.IsMatch("a", @"\d"));

            //역은 대문자.
            Assert.AreEqual(false, Regex.IsMatch("1", @"\D"));
            Assert.AreEqual(true, Regex.IsMatch("a", @"\D"));
        }

        #endregion

        #region 한정사 - 회수 조건
        // * : 0 1 2...
        // + : 1 2 3...
        // ? : 0 1
        // {1} : 1
        // {1,} : 1 2 3...
        // {1,3} : 1 2 3


        [Test]
        public void Check_Count()
        {
            Assert.AreEqual("123", Regex.Match("123", @"\d{1,2}3").Value);
            Assert.AreEqual("12345", Regex.Match("12345", @"\d{1,}5").Value); //숫자를 1개이상 가지고, 5로 끝나는 것
            Assert.AreEqual("12345", Regex.Match("12345", @"\d{1,4}5").Value);
            Assert.AreEqual("45", Regex.Match("12345", @"\d{1}5").Value); // 숫자를 하나만 가지고, 5로 끝나는 것은 "45"
            
            Assert.AreEqual("12345", Regex.Match("12345 23456", @"\d*5").Value); // 숫자를 0개 이상 가지고, 5로 끝나는 것은
            Assert.AreEqual(2, Regex.Matches("12345 23456", @"\d+5").Count); // 숫자를 1개 이상 가지고, 5로 끝나는 것은 12345, 2345 2가지 있다.
            Assert.AreEqual(3, Regex.Matches("12345 23456 5", @"\d?5").Count); // 숫자를 0 또는 1개 가지는 것. 12345, 2345, 5 3가지 있다.
        }

        #endregion
    }
}