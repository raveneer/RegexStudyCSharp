using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Study
{
    public class 한정사
    {
        [Test]
        public void u가0또는1있기()
        {
            var pattern = @"colou?r";
            Assert.AreEqual(true, TestMatch.IsAllMatch(pattern, "color", "colour"));
            Assert.AreEqual(true, TestMatch.IsAllNotMatch(pattern, "coluur"));
        }

        [Test]
        public void u가하나이상있을것()
        {
            var pattern = @"colou+r";
            Assert.AreEqual(true, TestMatch.IsAllMatch(pattern, "colour", "colouur"));
            Assert.AreEqual(true, TestMatch.IsAllNotMatch(pattern, "colr"));
        }

        [Test]
        public void u가하나만있을것()
        {
            var pattern = @"colou{1}r";
            Assert.AreEqual(true, TestMatch.IsAllMatch(pattern, "colour"));
            Assert.AreEqual(true, TestMatch.IsAllNotMatch(pattern, "colr", "colouur"));
        }

        [Test]
        public void u가있거나없거나()
        {
            var pattern = @"colou*r";
            Assert.AreEqual(true, TestMatch.IsAllMatch(pattern, "colour", "color", "colouur"));
        }

        [Test]
        public void 게으르게하기()
        {
            var pattern = @"\d.*?\d";
            Assert.AreEqual("1aa1", Regex.Match("1aa1 1bb1", pattern).Value);
        }
    }

    public class 문자집합
    {
        [Test]
        public void 숫자를뽑아내기()
        {
            var pattern = @"\d\d";
            Assert.AreEqual("11", Regex.Match("11aa", pattern).Value);
            Assert.AreEqual("11", Regex.Match("aa11", pattern).Value);
            Assert.AreEqual("11", Regex.Match("a11aa", pattern).Value);
            Assert.AreEqual("11", Regex.Match("a111aa", pattern).Value);
        }

        [Test]
        public void 글자를뽑아내기()
        {
            var pattern = @"\w\w";
            Assert.AreEqual("11", Regex.Match("11aa", pattern).Value);
            Assert.AreEqual("aa", Regex.Match("aa11", pattern).Value);
            Assert.AreEqual("a1", Regex.Match("a11aa", pattern).Value);
            Assert.AreEqual("a1", Regex.Match("a111aa", pattern).Value);
        }

        
        [Test]
        public void 알파벳만뽑아내기()
        {
            var pattern = @"\D\D";
            Assert.AreEqual("aa", Regex.Match("11aa", pattern).Value);
            Assert.AreEqual("aa", Regex.Match("aa11", pattern).Value);
            Assert.AreEqual("aa", Regex.Match("a11aa", pattern).Value);
            Assert.AreEqual("aa", Regex.Match("a111aa", pattern).Value);
        }

        [Test]
        public void 띄어쓰기를_로교체할것()
        {
            var pattern = @"\s";
            Assert.AreEqual("1_1", Regex.Replace("1 1", pattern, "_"));
        }
        
    }

    public class 치환그룹
    {

    }

    public class 너비0단언
    {
        [Test]
        public void 시작지점에서부터()
        {
            var pattern = @"^..";
            Assert.AreEqual("aa", Regex.Match("aa", pattern).Value);
            Assert.AreEqual("aa", Regex.Match("aaa", pattern).Value);
            Assert.AreEqual("aa", Regex.Match("aaaa", pattern).Value);
        }

        [Test]
        public void 끝에서부터()
        {
            var pattern = @"..$";
            Assert.AreEqual("bb", Regex.Match("aabb", pattern).Value);
            Assert.AreEqual("bb", Regex.Match("aaccbb", pattern).Value);
            Assert.AreEqual("bb", Regex.Match("aacbb", pattern).Value);
        }
        [Test]
        public void 단어경계로()
        {
            //여기서 .을 쓰지 않는 것에 주의. 헷갈림.
            var pattern = @"\b\w+\b";
            Assert.AreEqual(3, Regex.Matches("aa bb cc", pattern).Count);
        }

        [Test]
        public void 양성전방탐색()
        {
            //상당히 헷갈리는 것인데, consume 과 not consume 을 생각하면 좋다.
            //또한 '전방탐색' 이라고 하지만 실제로는 앞에 것을 검출해낸다는 것도 헷갈림. (그래서 전방탐색이라고 하는 걸까...)
            //보통 이렇게 해서 :까지 찾을 수 있지만...
            var pattern2 = @".+:";
            Assert.AreEqual("http:", Regex.Match("http://www.forta.com", pattern2).Value);
            Assert.AreEqual("https:", Regex.Match("https://www.forta.com", pattern2).Value);
            Assert.AreEqual("ftp:", Regex.Match("ftp://www.forta.com", pattern2).Value);

            //양성전방탐색을 씀으로, 해당 부분을 찾되 '포함하지 않을 수' 있다.
            var pattern = @".+(?=:)";
            Assert.AreEqual("http", Regex.Match("http://www.forta.com", pattern).Value);
            Assert.AreEqual("https", Regex.Match("https://www.forta.com", pattern).Value);
            Assert.AreEqual("ftp", Regex.Match("ftp://www.forta.com", pattern).Value);
        }

        [Test]
        public void 음성전방탐색()
        {
            //양성과 반대다. 주의할 점. 여러글자를 체크하므로, 앞글자에서 걸릴수도 있고 그렇다...
            var pattern = @"\d*(?!:)";
            Assert.AreEqual("5", Regex.Match("5/", pattern).Value);
            Assert.AreEqual("", Regex.Match("1:", pattern).Value);
            Assert.AreEqual("", Regex.Match("3:", pattern).Value);
        }

        [Test]
        public void 양성후방탐색()
        {
            //후방탐색은 조건식 이후를 찾아낸다. (이쪽이 좀 더 이해하기 쉽다...)
            var pattern = @"(?<=:).*";
            Assert.AreEqual("10", Regex.Match("http:10", pattern).Value);
            Assert.AreEqual("20", Regex.Match("https:20", pattern).Value);
            Assert.AreEqual("30 40 50", Regex.Match("ftp:30 40 50", pattern).Value);
        }

        [Test]
        public void 음성후방탐색()
        { 
            var pattern = @"(?<!:)\d\d";
            //2자릿수의 숫자일 것. 앞에 : 가 '없을 것' (역시 후방탐색이 이해하기 쉽다)
            Assert.AreEqual("20", Regex.Match("http:10 20", pattern).Value); 
        }

    }

    public class 분할
    {
        //Regex.Split 이용해서 string.Split 보다 강력한 교체가 가능하다.
        [Test]
        public void 강력하다()
        {
            var pattern = @"\d";
            Assert.AreEqual("aa", Regex.Split("aa1bb2cc", pattern)[0]);
            Assert.AreEqual("bb", Regex.Split("aa1bb2cc", pattern)[1]);
            Assert.AreEqual("cc", Regex.Split("aa1bb2cc", pattern)[2]);

            //나눌때 조건 사이에 정보가 없으면 ""로 나눠지는 것에 주의할 것.
            Assert.AreEqual("aa", Regex.Split("aa11bb22cc", pattern)[0]);
            Assert.AreEqual("", Regex.Split("aa11bb22cc", pattern)[1]);
            Assert.AreEqual("bb", Regex.Split("aa11bb22cc", pattern)[2]);
            Assert.AreEqual("", Regex.Split("aa11bb22cc", pattern)[3]);
            Assert.AreEqual("cc", Regex.Split("aa11bb22cc", pattern)[4]);
        }
    }

    public class 치환
    {
        [Test]
        public void 강력하다()
        {
            var pattern = @"\d";
            Assert.AreEqual("aaAbbAcc", Regex.Replace("aa1bb2cc", pattern,"A"));
        }
    }
    

    public static class TestMatch
    {
        public static bool IsAllMatch(string pattern, params string[] strs)
        {
            return strs.All(s => Regex.IsMatch(s, pattern));
        }

        public static bool IsAllNotMatch(string pattern, params string[] strs)
        {
            return strs.All(s => !Regex.IsMatch(s, pattern));
        }
    }

    public class 그루핑
    {
        [Test]
        public void 그룹으로뽑아낼수있다()
        {
            //주의! group[0]은 항상 정규식 매칭 '전체'를 반환한다!
            //따라서 각각의 그룹은 1부터 시작할 것.
            var pattern = @"(\d{3})-(\d{3}-\d{4})";
            Assert.AreEqual("123", Regex.Match("123-456-7890", pattern).Groups[1].Value);
            Assert.AreEqual("456-7890", Regex.Match("123-456-7890", pattern).Groups[2].Value);

            Assert.AreEqual("111", Regex.Match("111-222-3333", pattern).Groups[1].Value);
            Assert.AreEqual("222-3333", Regex.Match("111-222-3333", pattern).Groups[2].Value);


        }
    }

    public class Study{

    //match로 매칭결과를 상세히 볼 수 있다. 단 여러개 매칭하지는 못함.
        [Test]
        public void Match_ReturnResultObject()
        {
            var m = Regex.Match("i have color and colour", @"color");
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

        // | 연산자를 써서 부분을 여러가지 중 하나임을 뽑아낼 수 있다.
        // 이 경우 () 를 써야 함에 주의. 간단하지만 왜?
        [Test]
        public void UseOf_Or()
        {
            var ms = Regex.Matches("i have color and colour", @"col(o|ou)r");
            Assert.AreEqual("color", ms[0].Value);
            Assert.AreEqual("colour", ms[1].Value);
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
            Assert.AreEqual(true, Regex.IsMatch("A", @"", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)); // 대소문자가 다르니까 안된다.
        }

        //옵션은 해당 구문 앞에 ()로 처리할 수있다. 상당히...헷갈린다.
        [Test]
        public void CanBeShort_RegexOptions()
        {
            Assert.AreEqual(true, Regex.IsMatch("A", @"(?i)a")); //대소문자 무시. Ignore
            Assert.AreEqual(true, Regex.IsMatch("A", @"(?s)A")); // 한줄만. SingleLine

            Assert.AreEqual(true, Regex.IsMatch("A", @"(?m)A")); // 멀티라인. MultiLine
            Assert.AreEqual(true, Regex.IsMatch("B\r\nA", @"(?m)A")); // 멀티라인과 싱글라인 차이를 잘 모르겠네...
            Assert.AreEqual(true, Regex.IsMatch("BA", @"(?i)ba")); 

            //멀티라인과 싱글라인이 정확히 뭘 하는건지 모르겠다. 차차 알게 되겠지...
            Assert.AreEqual(true, Regex.IsMatch("B\r\nA", @"")); // 싱글라인에서도 줄바꿈후의 A를 잘 찾고...

            //문자열의 끝에서부터 역으로 검색할 수도 있다. 그렇다고 해서 문자열이 뒤집히는 것은 아니다. 단지 우선순위를 뒤에 준다는 것 뿐임. 
            //문자열을 '뒤집지 않는다' 라는 점이 의외로 좋은 해결책일 수도 있다. 왜냐면 문자열 자체를 뒤집으면 찾는 문자도 뒤집혀야 하기 때문이다. 
            Assert.AreEqual("3", Regex.Match("123", @"\d", RegexOptions.RightToLeft).Value);
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
            Assert.AreEqual(true, Regex.IsMatch("1", @"[0-1]"));

            //숫자는 숫자로 처리도 가능.
            Assert.AreEqual(true, Regex.IsMatch("1", @"\d"));
            //두자릿수의 숫자라면 이렇게도 가능. 
            Assert.AreEqual(true, Regex.IsMatch("11", @"\d\d"));
            Assert.AreEqual(true, Regex.IsMatch("00", @"\d\d"));
            Assert.AreEqual(true, Regex.IsMatch("99", @"\d\d"));
            Assert.AreEqual(false, Regex.IsMatch("1", @"\d\d"));
        }

        [Test]
        public void CheckKorean()
        {
            //한'글자' 만 받아내라.
            Assert.AreEqual("철", Regex.Match("철수", @"\w").Value);
            Assert.AreEqual("ㅊ", Regex.Match("ㅊㅋ", @"\w").Value);

            //한글만 받아내기. 상용구니까 외워버려라. (단 아래 정규식들은 1글자 만 판정하고 있음을 잊지말것)
            Assert.AreEqual(true, Regex.IsMatch("ㅊㅋ", @"[ㄱ-ㅎㅏ-ㅣ가-힣]"));
            Assert.AreEqual(true, Regex.IsMatch("ㅗㅜㅑ", @"[ㄱ-ㅎㅏ-ㅣ가-힣]"));
            Assert.AreEqual(true, Regex.IsMatch("철수", @"[ㄱ-ㅎㅏ-ㅣ가-힣]"));
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

        #region 한정사 - 회수 조건.
        //한정사를 씀으로서 여러개의 글자가 존재하는지 체크할 수 있다.
        
        // * : 0 1 2...
        // + : 1 2 3...
        // ? : 0 1
        // {1} : 1
        // {1,} : 1 2 3...
        // {1,3} : 1 2 3


        [Test]
        public void Check_Count()
        {
            Assert.AreEqual("123", Regex.Match("123", @"123").Value);

            Assert.AreEqual("12345", Regex.Match("12345", @"\d*5").Value); //숫자를 1개이상 가지고, 5로 끝나는 것

            Assert.AreEqual("45", Regex.Match("12345", @"\d5").Value); // 숫자를 하나만 가지고, 5로 끝나는 것은 "45"
            
            Assert.AreEqual(2, Regex.Matches("12345 23456", @"\d+5").Count); // 숫자를 1개 이상 가지고, 5로 끝나는 것은 12345, 2345 2가지 있다.
            Assert.AreEqual(3, Regex.Matches("12345 23456 5", @"\d?5").Count); // 숫자를 0 또는 1개 가지는 것. 12345, 2345, 5 3가지 있다.
        }


        [Test]
        public void Lazy_한정사()
        {
            //뒤에 ? 붙이는 걸로 한정사가 첫번째 매치만 찾도록 할 수 있다.
            
            //A와 A 사이에 아무 문자라도 있거나 없거나 하면 됨. (탐욕적)
            Assert.AreEqual("ABABA", Regex.Match("ABABA", @"A.*A").Value); 
            
            //A와 A 사이에 아무 문자라도 있거나 없거나 할 것. (게으른)
            Assert.AreEqual("ABA", Regex.Match("ABABA", @"A.*?A").Value);

            //위 경우는 당연히 여러개의 답이 존재하게 된다. 단, 겹치지는 않음에 주의할 것.
            Assert.AreEqual(2, Regex.Matches("ABAABA", @"A.*?A").Count); // 이 경우 2개의 답이 되지만...
            Assert.AreEqual(1, Regex.Matches("ABABA", @"A.*?A").Count); //이 경우 1개의 답이 되버린다. 중앙의 A가 겹치지 않기 때문에.

        }
        #endregion

        #region ZeroWidth

        //지금까지의 정규식들은 식을 이용해 특정 값을 체크했다.
        //전방탐색, 후방탐색, 앵커, 단어경계를 이용하여 특정 값 이전과 이후의 조건을 체크하는 것이 가능하다.
        //당연히 이것은 '조건' 이므로 정규식의 검색 결과에 포함되지 않는다. (포함되지 않는게 장점일때도 있을 것이다)
        //예를 들어 숫자 뒤에 글자가 오는 경우의 패턴을 찿고 싶다면, '앞에 글자가 있다'는 것이 조건이며, 실제 찾는 것은 '글자' 인 것이다.

        //(?=정규식) 으로 묶어줌으로서,전방탐색을 할 수 있다.
        //(?!정규식) 으로 음성 전방탐색 (전방탐색의 조건과 반대)를 할 수 있다.
        
        //주의해야 할 것. (?i)는 '소문자 옵션' 이다. 상당히...헷갈리지.
        //어쨌든 () 안에 오는게 '정규식' 임에 유의할 것.

        [Test]
        public void Test_LookAhead()
        {
            //miles가 뒤따라 오는 숫자를 구하기.
            //(단 이 경우 예제의 명확함을 위해 /s가 앞에 나온것에 주의)
            Assert.AreEqual("25 ", Regex.Match("say 25 miles more", @"\d+\s(?=miles)").Value);
            //순수히 숫자만 구하겠다면...\s 를 안으로 집어넣어야 하는데, 이러면 꼭 smiles 처럼 보인단 말이지...
            Assert.AreEqual("25", Regex.Match("say 25 miles more", @"\d+(?=\smiles)").Value);
            //이렇게 조건을 만족하지 않는다면 검출되지 않는다. (mile 과 meter 는 다르니까)
            Assert.AreEqual("", Regex.Match("say 25 meter more", @"\d+(?=\smiles)").Value);
            //단지 조건일 뿐이므로, 이런 식으로 그 이후를 받아내는 것도 가능하다. (뒤에 .*를 넣어 끝까지 뽑아냈음) -> 관용구라 할 만 하네.
            Assert.AreEqual("25 miles more", Regex.Match("say 25 miles more", @"\d+(?=\smiles).*").Value);

            //강한 패스워드 규칙을 정할때 자주 쓰게 된다 (관용구)
            //적어도 6자 여야 하며, 숫자가 적어도 하나는 있어야 한다고 하자.

            //1. (?=) 로 조건을 지정했다. "숫자가 1개 이상 있을 것" 
            //2. 그 조건을 만족했다면, 만족한 곳으로 돌아가서 "길이가 6이상인지 체크" 한다.
            //todo : 이 예제에서 헷갈리는 것은 '만족한 곳으로 돌아간다' 인데. 그게 어딘지 정확히 모르겠다.
            var pattern = @"(?=.*\d).{6,}";  
            Assert.AreEqual(true, Regex.IsMatch("abcde1", pattern));
            Assert.AreEqual(false, Regex.IsMatch("ABCD1", pattern)); // 길이가 모자람
            Assert.AreEqual(true, Regex.IsMatch("ABCDEF11", pattern));//길이가 넘치는 건 괜찮음.
            Assert.AreEqual(false, Regex.IsMatch("ABCDEF", pattern));//숫자가 하나는 있어야 함
            Assert.AreEqual(true, Regex.IsMatch("1234567", pattern)); //숫자만으로 되어도 문제없다.

            
        }


        [Test]
        public void NegativeLookAhead()
        {
            //움성 전방탐색은 (?! 로 표시한다. ?= 와 ?! 의 쌍임)
            //(?!)
            //이후의 텍스트가 정규식과 부합하지 않아야 만족함.
            //Q: good 을 가지되, 그 다음에 but 이나 however 가 없을 것
            //대소문자 구분을 하지 않을 것.
            var pattern = @"(?i)good(?!.*(however|but))"; 
            Assert.AreEqual(true, Regex.IsMatch("good", pattern));
            Assert.AreEqual(true, Regex.IsMatch("Good", pattern));
            Assert.AreEqual(false, Regex.IsMatch("good. but...", pattern));
            Assert.AreEqual(false, Regex.IsMatch("good. But...", pattern));
            Assert.AreEqual(false, Regex.IsMatch("good. and But...", pattern));
            Assert.AreEqual(false, Regex.IsMatch("good. however...", pattern));
            Assert.AreEqual(false, Regex.IsMatch("good. However...", pattern));
            Assert.AreEqual(false, Regex.IsMatch("good. and so However...", pattern));

            //Q : 그 역을 구하시오. good을 포함하되, 뒤에 but이나 however 가 따라와야 성립하도록 할 것. 대소문자 무시할 것.
            var antiRegex = @"(?i)good(?=.*(but|however))";
            Assert.AreEqual(false, Regex.IsMatch("good", antiRegex));
            Assert.AreEqual(true, Regex.IsMatch("good. but...", antiRegex));
        }


        [Test]
        public void NegativeLookBehind()
        {
            //(?<=정규식)
            //양성 후방탐색
            //부합위치 이전의 텍스트가 정규식과 부합해야 함. 
            var pattern = @"(?i)(?<=however.*)good"; //대소문자 무시하고, however 가 앞에 있어야 한다.
            Assert.AreEqual(false, Regex.IsMatch("very good", pattern));
            Assert.AreEqual(true, Regex.IsMatch("However good", pattern));
            Assert.AreEqual(true, Regex.IsMatch("However and good", pattern));
        }

        [Test]
        public void PositiveLookBehind()
        {
            //(?<!정규식)
            //음성 후방탐색
            //부합위치 이전의 텍스트사 정규식과 부합하지 않아야 함.
            var pattern = @"(?i)(?<!however.*)good"; //대소문자 무시하고, however 가 앞에 없어야 함. however 뒤에 .*는 그 사이에 글자가 몇개 있을 수도 있다는 것.
            Assert.AreEqual(true,  Regex.IsMatch("very good", pattern)); 
            Assert.AreEqual(false,  Regex.IsMatch("However good", pattern));
            Assert.AreEqual(false,  Regex.IsMatch("However and good", pattern)); //however 뒤에 .*이 붙었으므로, good 과 however 사이에 무엇이 있든 상관없음.

        }
        #endregion

        #region Anchor

        //^와 $ 로 앵커를 세팅할 수 있다. 이들은 문자가 아니라 특정 '위치'에 부합한다. 
        //^ : 문자열의 시작을 의미. 멀티라인에서는 한행의 시작과 부합. (여러개가 될 수도 있겠네?)
        //$ : 문자열의 끝을 의미. 멀티라인에서는 한행의 끝과 부합.
        [Test]
        public void Anchor_MeansPlace_NotWord()
        {
            //위치만 지정하고 글자 자체는 받아내지 못한다... 그럼 이거 왜 쓰는거야?
            Assert.AreEqual("", Regex.Match("a", @"^").Value);
            Assert.AreEqual("", Regex.Match("a", @"$").Value);

            //이래도 빈칸임. 즉 위치정보만 가지고 값 정보는 없다고 볼 수 있다. (애매하네용)000
            Assert.AreEqual("", Regex.Match("aa", @"^").Value);
            Assert.AreEqual("", Regex.Match("aa", @"$").Value);
        }

        [Test]
        public void ByWord()
        {
            var pattern = @"\b\w+\b";
            Assert.AreEqual(3, Regex.Matches("aa bb cc", pattern).Count);
            Assert.AreEqual(3, Regex.Matches("aa bbb cc", pattern).Count);
            Assert.AreEqual(4, Regex.Matches("aa bb1 ccc 1", pattern).Count);

            //단어단위로 끊지 않으므로, 2개 선택됨 (in, in)
            Assert.AreEqual(2, Regex.Matches("in inout", @"in").Count);
            //단어단위로 끊으니까 앞의 in 만 선택됨
            Assert.AreEqual(1, Regex.Matches("in inout", @"\bin\b").Count);
            Assert.AreEqual("in", Regex.Match("in inout", @"\bin\b").Value);

        }

        #endregion
    }
}