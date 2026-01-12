using System;

namespace RhythmGameOOP
{
    // [결과 화면 클래스] 게임 종료 후 성적표를 보여줍니다.
    public class ResultScene
    {
        public void Show(int finalScore, int maxCombo, string rank)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;

            // 결과 화면 디자인
            Console.WriteLine("\n\n\n");
            Console.WriteLine("    ==========================================");
            Console.WriteLine("             G A M E   R E S U L T            ");
            Console.WriteLine("    ==========================================");
            Console.ResetColor();

            Console.WriteLine("\n");

            // 점수 표시
            Console.WriteLine($"         FINAL SCORE :  {finalScore}");
            Console.WriteLine($"         MAX COMBO   :  {maxCombo}");

            Console.WriteLine("\n    ------------------------------------------");

            // 랭크를 크게 표시
            Console.ForegroundColor = GetRankColor(rank); // 랭크별 색상 적용
            Console.WriteLine($"\n             RANK  :  [  {rank}  ]  ");
            Console.ResetColor();

            Console.WriteLine("\n    ------------------------------------------");
            Console.WriteLine("\n\n         [Enter] 키를 누르면 메뉴로 돌아갑니다.");

            // 엔터 키가 눌릴 때까지 대기
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;
                    if (key == ConsoleKey.Enter) break;
                }
            }
        }

        // 랭크에 따라 색깔을 다르게 주는 꾸미기 함수
        private ConsoleColor GetRankColor(string rank)
        {
            if (rank == "S") return ConsoleColor.Cyan;    // S랭크는 하늘색
            if (rank == "A") return ConsoleColor.Green;   // A랭크는 초록색
            if (rank == "B") return ConsoleColor.Yellow;  // B랭크는 노란색
            if (rank == "C") return ConsoleColor.Magenta; // C랭크는 자주색
            return ConsoleColor.Red;                      // 나머지는 빨간색
        }
    }
}