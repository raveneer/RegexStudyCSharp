using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Study
{
    public class Study
    {
        #region 문자범주

        public class 문자범주
        {
            // /p{범주}를 이용해서 좀 더 세밀한 체크가 가능하다.
            //단, C#의 특수 정규식이라고 할 수 있다. 일반 정규식들은 지원하지 않음.
            [Test]
            public void 글자검출()
            {
                var pattern = @"\p{L}"; //Language
                Assert.AreEqual("a", Regex.Match("a1", pattern).Value); //소문자 검출
                Assert.AreEqual("A", Regex.Match("A1", pattern).Value); //대문자 검출
                Assert.AreEqual("", Regex.Match("11", pattern).Value); // 숫자는 검출하지 아니한다.
            }

            [Test]
            public void 기호검출()
            {
                //?? 뭐가 기호인건데 대체?
            }

            [Test]
            public void 대문자검출()
            {
                var pattern = @"\p{Lu}"; //Language upper
                Assert.AreEqual("A", Regex.Match("A1", pattern).Value); //대문자 검출
                Assert.AreEqual("", Regex.Match("a1", pattern).Value);
                Assert.AreEqual("", Regex.Match("11", pattern).Value);
            }

            [Test]
            public void 문장부호검출()
            {
                var pattern = @"\p{P}";
                Assert.AreEqual(",", Regex.Match(",A1", pattern).Value);
                Assert.AreEqual(".", Regex.Match(".A1", pattern).Value);
                Assert.AreEqual("[", Regex.Match("[A1", pattern).Value);
                Assert.AreEqual("!", Regex.Match("A1!", pattern).Value);
                Assert.AreEqual("?", Regex.Match("A1?", pattern).Value);
                Assert.AreEqual("\"", Regex.Match("\"A1?", pattern).Value);
                Assert.AreEqual("\'", Regex.Match("\'A1?", pattern).Value);
                Assert.AreEqual("", Regex.Match("a1", pattern).Value);
                Assert.AreEqual("", Regex.Match("11", pattern).Value);
            }

            [Test]
            public void 소문자검출()
            {
                var pattern = @"\p{Ll}"; //Language lower
                Assert.AreEqual("a", Regex.Match("a1", pattern).Value);
                Assert.AreEqual("", Regex.Match("A1", pattern).Value);
                Assert.AreEqual("", Regex.Match("11", pattern).Value);
            }

            [Test]
            public void 숫자검출()
            {
                var pattern = @"\p{N}"; //Number
                Assert.AreEqual("1", Regex.Match("A1", pattern).Value);
                Assert.AreEqual("1", Regex.Match("a1", pattern).Value);
                Assert.AreEqual("1", Regex.Match("11", pattern).Value);
            }
        }

        #endregion

        #region C#의Regex클래스활용하기

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

            //Regex.Split 이용해서 string.Split 보다 강력한 교체가 가능하다.
            [Test]
            public void 분할()
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

            [Test]
            public void 치환()
            {
                var pattern = @"\d";
                Assert.AreEqual("aaAbbAcc", Regex.Replace("aa1bb2cc", pattern, "A"));
            }

            [Test]
            public void 그룹으로뽑아낼수있다()
            {
                //주의! group[0]은 항상 정규식 매칭 '전체'를 반환한다!
                //따라서 각각의 그룹은 1부터 시작함.
                //2개의 그룹으로 나누시오.
                var pattern = @"(\d{1,3})-(.*)";
                Assert.AreEqual("123", Regex.Match("123-456-7890", pattern).Groups[1].Value);
                Assert.AreEqual("456-7890", Regex.Match("123-456-7890", pattern).Groups[2].Value);

                Assert.AreEqual("111", Regex.Match("111-222-3333", pattern).Groups[1].Value);
                Assert.AreEqual("222-3333", Regex.Match("111-222-3333", pattern).Groups[2].Value);
            }

        #endregion

        #region Group

        /// <summary>
        ///     () 로 묶어줌으로서 그룹으로 지정할 수 있다.
        ///     그룹은 \1 \2 등으로 지칭하는게 가능하다. (앞에 나온 패턴을 반복해서 쓴다거나 할 수 있다)
        ///     지칭에는 명명된 그룹을 쓸 수도 있는데, (?'그룹-이름'그룹-정규식) 으로 지정하고 \k'그룹-이름' 으로 사용한다. (복잡하다...)
        ///     그룹을 조건에만 포함하고 '캡쳐' 에는 쓰지 않고 싶을 때가 있는데, 이때는 (?:) 를 앞에 붙여주면 된다. -> 파이썬만 되나?
        ///     C#에서는 Match의 결과를 Match.Group[1],[2] 등으로 갈무리 한다. ([0]은 매치 전체이다. m.value 와 같다)
        /// </summary>
        [Test]
        public void 그룹을이용하여첫글자와끝글자가같은것을찾아내자()
        {
            var pattern = @"(\w)\w+\1"; //앞의 캡쳐그룹(\w)를 뒤에서 \1 로 '반복' 하고 있음이 포인트. 
            Assert.AreEqual("pop", Regex.Match("pop pee", pattern).Value);
            Assert.AreEqual("A00A", Regex.Match("A00A  A0BC", pattern).Value);
        }

        [Test]
        public void 그룹을이용하여_국번과번호를뽑아내자()
        {
            var pattern = @"(\d\d\d)-(\d\d\d-\d\d\d\d)";
            Assert.AreEqual("123", Regex.Match("123-456-7890", pattern).Groups[1].Value);
            Assert.AreEqual("456-7890", Regex.Match("123-456-7890", pattern).Groups[2].Value);
            Assert.AreEqual("123-456-7890", Regex.Match("123-456-7890", pattern).Groups[0].Value);
        }

        [Test]
        public void 명명된그룹()
        {
            var pattern = @"(?'repeat'\w)\w+\k'repeat'"; //그냥 번호 그룹이 더 쉬운것 같은데 -_-
            Assert.AreEqual("pop", Regex.Match("pop pee", pattern).Value);
            Assert.AreEqual("A00A", Regex.Match("A00A  A0BC", pattern).Value);
        }

        public class 그룹을이용한치환
        {
            [Test]
            public void 그룹번호를이용한치환하기()
            {
                var pattern = @"(\d)(\w)";
                var replace = @"_$1<$2>";
                Assert.AreEqual("1a", Regex.Match("1a2b3c", pattern).Groups[0].Value);
                Assert.AreEqual("1", Regex.Match("1a2b3c", pattern).Groups[1].Value);
                Assert.AreEqual("a", Regex.Match("1a2b3c", pattern).Groups[2].Value);
                Assert.AreEqual("", Regex.Match("1a2b3c", pattern).Groups[3].Value);
                Assert.AreEqual("_1<a>_2<b>_3<c>", Regex.Replace("1a2b3c", pattern, replace));
            }

            //$0으로 매치의 모든 그룹을 표현할 수 있다.
            [Test]
            public void 단순한그룹치환_숫자앞에언더바를붙이시오()
            {
                var pattern = @"(\d)";
                Assert.AreEqual("_1_2_3", Regex.Replace("123", pattern, @"_$0"));
            }

            // ()를 이용해서 특정 정규식의 매치를 $1 등으로 치환할 수 있다.(정규식에서 지정할때는 \1을 썻음에 주의)
            //이때 $는 0부터 시작해도 되고 1부터 시작해도 됨
            [Test]
            public void 단순한치환()
            {
                var pattern = @"(\d)";
                var replace = @"_$0";
                Assert.AreEqual("_1_2_3", Regex.Replace("123", pattern, replace));
            }

            [Test]
            public void 대문자를소문자로()
            {
                var pattern = @"(\D)";
                Assert.AreEqual("1A2B3C", Regex.Replace("1a2b3c", pattern, m => m.ToString().ToUpper()));
            }

            [Test]
            public void 람다식으로원본데이터를변조()
            {
                //https://stackoverflow.com/questions/16117043/regular-expression-replace-in-c-sharp
                var pattern = @"(\d)(\w)";

                //뭔가 굉장히...돌아가는 기분인데, 이정도면 그냥 링큐쓰는게 나을수도 있겠다 생각이 든다...
                Assert.AreEqual("_!_!_!", Regex.Replace("1a2b3c", pattern, m => string.Concat(
                                                                                              Regex.Replace(m.Groups[1].Value, @".", "_")
                                                                                            , Regex.Replace(m.Groups[2].Value, @".", "!")
                                                                                             ))
                               );
            }
        }

        #endregion

        #region RegexOptions
        ///옵션은 해당 구문 앞에 (?x)로 처리할 수있다.

        [Test]
        public void CaseSensitive()
        {
            var pattern = @"a";
            Assert.AreEqual(false, Regex.IsMatch("A", pattern)); // 대소문자가 다르니까 안된다.
            Assert.AreEqual(true, Regex.IsMatch("A", pattern, RegexOptions.IgnoreCase)); // 대소문자를 무시하고 찾을 수 있다.
            var match = Regex.Match("A", pattern, RegexOptions.IgnoreCase);
            Assert.AreEqual("A", match.Value); // 무시하고 찾았어도, 찾은 값 자체는 불변이다. (뭐 당연한 거긴 하지...)
        }

        [Test]
        public void CaseSensitive_IgnoreCulture()
        {
            //문화권에 관계없는 대소문자 무시도 가능하다.
            //근데 인자에 | 를 넣는건 대체 무슨 문법이지??? -> 비트플래그였다!!!
            var pattern = @"a";
            Assert.AreEqual(true, Regex.IsMatch("A", pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)); // 대소문자가 다르니까 안된다.
        }
        
        [Test]
        public void CanBeShort_RegexOptions()
        {
            //대소문자 무시. Ignore
            Assert.AreEqual("A", Regex.Match("A", @"(?i)a").Value);
            Assert.AreEqual("a", Regex.Match("a", @"(?i)a").Value);

            // 한줄만. SingleLine
            Assert.AreEqual("A", Regex.Match("A\r\nA", @"(?s)A").Value);

            // 멀티라인. MultiLine
            Assert.AreEqual(3, Regex.Matches("AA\r\nA", @"(?m)A").Count);

            // 명명되거나 번호가 지정된 그룹만 갈무리하기 -> todo : 아직 이해를 못하였다.
            //Assert.AreEqual(3, Regex.Matches("AA\r\nA", @"(?m)A").Count); 

            //정규식내 공백문자제거하여 체크하기 -> 상당히 야리꾸리한 문제인데, 정규식 '내'의 공백문자 조건을 '무시' 한다는 것이다.
            //결코 결과나 샘플의 공백문자를 무시하는게 아님에 주의. 
            //어디다 쓰냐 이걸 대체?
            Assert.AreEqual("AAAA", Regex.Match("AAAA", @"(?x)A A  AA").Value);
        }

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
            var pattern = @"[Tt]hat";
            Assert.AreEqual(true, Regex.IsMatch("that", pattern));
            Assert.AreEqual(true, Regex.IsMatch("That", pattern));
            Assert.AreEqual(false, Regex.IsMatch("hat", pattern));
        }

        [Test]
        public void CheckKorean()
        {
            //한글만 받아내기. 상용구니까 외워버려라. (단 아래 정규식들은 1글자 만 판정하고 있음을 잊지말것)

            var pattern = @"[가-힇|ㅏ-ㅣ|ㄱ-ㅎ]";
            Assert.AreEqual("ㅊ", Regex.Match("ㅊㅋ", pattern).Value);
            Assert.AreEqual("ㅗ", Regex.Match("ㅗㅜㅑ", pattern).Value);
            Assert.AreEqual("철", Regex.Match("철수", pattern).Value);
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
            var pattern = @"[a-z]{2}";
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
        public void u가0또는1있기()
        {
            var pattern = @"colou?r";
            Assert.AreEqual(true, TestHelper.IsAllMatch(pattern, "color", "colour"));
            Assert.AreEqual(true, TestHelper.IsAllNotMatch(pattern, "coluur"));
        }

        [Test]
        public void u가하나이상있을것()
        {
            var pattern = @"colou+r";
            Assert.AreEqual(true, TestHelper.IsAllMatch(pattern, "colour", "colouur"));
            Assert.AreEqual(true, TestHelper.IsAllNotMatch(pattern, "color"));
        }

        [Test]
        public void u가하나만있을것()
        {
            var pattern = @"colou{1}r";
            Assert.AreEqual(true, TestHelper.IsAllMatch(pattern, "colour"));
            Assert.AreEqual(true, TestHelper.IsAllNotMatch(pattern, "color", "colouur"));
        }

        [Test]
        public void u가있거나없거나()
        {
            var pattern = @"colou*r";
            Assert.AreEqual(true, TestHelper.IsAllMatch(pattern, "colour", "color", "colouur"));
        }

        [Test]
        public void 게으르게하기()
        {
            var pattern = @"1.*?1";
            Assert.AreEqual("1aa1", Regex.Match("1aa1 1bb1", pattern).Value);
        }

        [Test]
        public void Check_Count()
        {
            Assert.AreEqual("123", Regex.Match("123", @"\d{1,3}").Value);
            Assert.AreEqual("12345", Regex.Match("12345", @"\d{1,5}").Value);
            Assert.AreEqual("45", Regex.Match("12345", @"(?<=\d\d\d)\d\d").Value);

            Assert.AreEqual(2, Regex.Matches("12345 23456", @"\d+5").Count); // 숫자를 1개 이상 가지고, 5로 끝나는 것은 12345, 2345 2가지 있다.
            Assert.AreEqual(3, Regex.Matches("12345 23456 5", @"\d+").Count); // 숫자를 0 또는 1개 가지는 것. 12345, 2345, 5 3가지 있다.
        }

        [Test]
        public void Lazy_한정사()
        {
            //뒤에 ? 붙이는 걸로 한정사가 첫번째 매치만 찾도록 할 수 있다.

            var pattern1 = @"A.*A";
            var pattern2 = @"A.*?A";

            Assert.AreEqual("ABABA", Regex.Match("ABABA", pattern1).Value);
            Assert.AreEqual(1, Regex.Matches("ABABA", pattern1).Count);

            Assert.AreEqual("ABA", Regex.Match("ABABA", pattern2).Value);
            Assert.AreEqual(2, Regex.Matches("ABAABA", pattern2).Count);
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

        #endregion

        #region ZeroWidth

        //전방 후방탐색 헷갈릴때 참고 https://sub0709.tistory.com/54
        //지금까지의 정규식들은 식을 이용해 특정 값을 체크했다.
        //전방탐색, 후방탐색, 앵커, 단어경계를 이용하여 특정 값 이전과 이후의 조건을 체크하는 것이 가능하다.
        //당연히 이것은 '조건' 이므로 정규식의 검색 결과에 포함되지 않는다. (포함되지 않는게 장점일때도 있을 것이다)
        //예를 들어 숫자 뒤에 글자가 오는 경우의 패턴을 찿고 싶다면, '앞에 글자가 있다'는 것이 조건이며, 실제 찾는 것은 '글자' 인 것이다.

        //(?=정규식) 으로 묶어줌으로서,전방탐색을 할 수 있다.
        //(?!정규식) 으로 음성 전방탐색 (전방탐색의 조건과 반대)를 할 수 있다.

        //주의해야 할 것. (?i)는 '소문자 옵션' 이다. 상당히...헷갈리지.
        //어쨌든 () 안에 오는게 '정규식' 임에 유의할 것.

        [Test]
        public void 시작지점에서부터()
        {
            var pattern = @"^\w{2}";
            Assert.AreEqual("aa", Regex.Match("aa", pattern).Value);
            Assert.AreEqual("aa", Regex.Match("aaa", pattern).Value);
            Assert.AreEqual("aa", Regex.Match("aaaa", pattern).Value);
        }

        [Test]
        public void 끝에서부터()
        {
            var pattern = @"\w{2}$";
            Assert.AreEqual("bb", Regex.Match("aabb", pattern).Value);
            Assert.AreEqual("bb", Regex.Match("aaccbb", pattern).Value);
            Assert.AreEqual("bb", Regex.Match("aacbb", pattern).Value);
        }

        [Test]
        public void 단어경계로()
        {
            var pattern = @"\b\w{2}\b";
            Assert.AreEqual(3, Regex.Matches("aa bb cc", pattern).Count);
            Assert.AreEqual("aa", Regex.Matches("aa bb cc", pattern)[0].Value);
            Assert.AreEqual("bb", Regex.Matches("aa bb cc", pattern)[1].Value);
            Assert.AreEqual("cc", Regex.Matches("aa bb cc", pattern)[2].Value);
        }

        [Test]
        public void 양성전방탐색()
        {
            //전탐을 쓰지 않고 일반적인 경우...
            var pattern2 = @".*:";
            Assert.AreEqual("http:", Regex.Match("http://www.forta.com", pattern2).Value);
            Assert.AreEqual("https:", Regex.Match("https://www.forta.com", pattern2).Value);
            Assert.AreEqual("ftp:", Regex.Match("ftp://www.forta.com", pattern2).Value);

            //양성전방탐색을 씀으로, 해당 부분을 찾되 '포함하지 않을 수' 있다.
            // -> 여전히 잘 이해가 되지 않는다 쩝...
            var pattern = @".*(?=:)";
            Assert.AreEqual("http", Regex.Match("http://www.forta.com", pattern).Value);
            Assert.AreEqual("https", Regex.Match("https://www.forta.com", pattern).Value);
            Assert.AreEqual("ftp", Regex.Match("ftp://www.forta.com", pattern).Value);
        }

        [Test]
        public void 음성전방탐색()
        {
            var pattern = @"\d(?!:)";
            Assert.AreEqual("5", Regex.Match("5/", pattern).Value);
            Assert.AreEqual("", Regex.Match("1:", pattern).Value);
            Assert.AreEqual("", Regex.Match("3:", pattern).Value);
        }

        [Test]
        public void 양성후방탐색()
        {
            var pattern = @"(?<=:).*";
            Assert.AreEqual("10", Regex.Match("http:10", pattern).Value);
            Assert.AreEqual("20", Regex.Match("https:20", pattern).Value);
            Assert.AreEqual("30 40 50", Regex.Match("ftp:30 40 50", pattern).Value);
        }

        [Test]
        public void 음성후방탐색()
        {
            //2자릿수의 숫자일 것. 앞에 : 가 '없을 것'
            var pattern = @"(?<!:)\d\d";
            Assert.AreEqual("20", Regex.Match("http:10 20", pattern).Value);
        }

        [Test]
        public void Test_LookAhead()
        {
            //miles가 뒤따라 오는 숫자를 구하기.
            //(단 이 경우 예제의 명확함을 위해 /s가 앞에 나온것에 주의)
            var pattern = @"\d+(?= mile)";
            Assert.AreEqual("25", Regex.Match("say 25 miles more", pattern).Value);
            //이렇게 조건을 만족하지 않는다면 검출되지 않는다. (mile 과 meter 는 다르니까)
            Assert.AreEqual("", Regex.Match("say 25 meter more", pattern).Value);
        }

        [Test]
        public void Example_LookAhead_PasswordCheck()
        {
            //강한 패스워드 규칙을 정할때 자주 쓰게 된다 (관용구)
            //적어도 6자 여야 하며, 숫자가 적어도 하나는 있어야 함.
            //1. (?=) 로 조건을 지정했다. "숫자가 1개 이상 있을 것" 
            //2. 그 조건을 만족했다면, 만족한 곳으로 돌아가서 "길이가 6이상인지 체크" 한다.
            //todo : 이 예제에서 헷갈리는 것은 '만족한 곳으로 돌아간다' 인데. 그게 어딘지 정확히 모르겠다.
            //https://sub0709.tistory.com/54
            bool IsValidPassword(string s)
            {
                var pattern = @"(?=.*\d).{6,}";
                return Regex.IsMatch(s, pattern);
            }
            Assert.AreEqual(true, IsValidPassword("abcde1"));
            Assert.AreEqual(true, IsValidPassword("1bcde1"));
            Assert.AreEqual(true, IsValidPassword("1bcdef"));
            Assert.AreEqual(true, IsValidPassword("123456"));
            Assert.AreEqual(true, IsValidPassword("1234567"));
            Assert.AreEqual(false, IsValidPassword("12345"));
            Assert.AreEqual(false, IsValidPassword("abcdef"));
            Assert.AreEqual(false, IsValidPassword("abcdef"));
        }

        [Test]
        public void 패스워드식의비밀()
        {
            //양성전방탐색은 찾은 위치의 인덱스를 보존한다.

            //이를 이용하여, 전방탐색을 앞에 보냄으로서 (으 엄청 헷갈리네) 식의 처음부터 다시 검색할 수 있다. (패스워드 찾는 건 이것이 비밀이었음)
            Assert.AreEqual("a", Regex.Match("abcde1", @"(?=abc).").Value); //전방탐색이 찾은 것은 abc인데. 첫 인덱스는 a 이므로. a부터 체크하여 . 은 a
            Assert.AreEqual("b", Regex.Match("abcde1", @"(?=bc).").Value); //bc => b
            Assert.AreEqual("c", Regex.Match("abcde1", @"(?=cde).").Value); // cded => c

            //양성전방탐색을 뒤로 보내면 어떻게 되나? (일반적인 사용법)
            Assert.AreEqual("", Regex.Match("abcde1", @".(?=abc)").Value);
            Assert.AreEqual("a", Regex.Match("abcde1", @".(?=bc)").Value);
            Assert.AreEqual("b", Regex.Match("abcde1", @".(?=cde)").Value);
        }

        [Test]
        public void NegativeLookAhead()
        {
            //움성 전방탐색은 (?! 로 표시한다. ?= 와 ?! 의 쌍임)
            //(?!)
            //이후의 텍스트가 정규식과 부합하지 않아야 만족함.
            //Q: good 을 가지되, 그 다음에 but 이나 however 가 없을 것
            //대소문자 구분을 하지 않을 것.
            var pattern = @"(?i)good(?!.*(but|however))";
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
            Assert.AreEqual(true, Regex.IsMatch("very good", pattern));
            Assert.AreEqual(false, Regex.IsMatch("However good", pattern));
            Assert.AreEqual(false, Regex.IsMatch("However and good", pattern)); //however 뒤에 .*이 붙었으므로, good 과 however 사이에 무엇이 있든 상관없음.
        }

        #endregion

        #region Anchor

        //^와 $ 로 앵커를 세팅할 수 있다. 이들은 문자가 아니라 특정 '위치'에 부합한다. 
        //^ : 문자열의 시작을 의미. 멀티라인에서는 한행의 시작과 부합. (여러개가 될 수도 있겠네?)
        //$ : 문자열의 끝을 의미. 멀티라인에서는 한행의 끝과 부합.
        [Test]
        public void Anchor_MeansPlace_NotWord()
        {
            Assert.AreEqual("a", Regex.Match("a", @"^\w").Value);
            Assert.AreEqual("b", Regex.Match("ab", @"\w$").Value);
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

    public static class TestHelper
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
}