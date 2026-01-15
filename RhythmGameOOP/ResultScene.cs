using System;

namespace RhythmGameOOP
{
    public class ResultScene
    {
        public void Show(int score, int maxCombo, string rank)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;

            // 결과 화면 타이틀
            Console.WriteLine("\n\n\n");
            Console.WriteLine("    ==========================================");
            Console.WriteLine("             G A M E   R E S U L T            ");
            Console.WriteLine("    ==========================================");
            Console.ResetColor();
            Console.WriteLine("\n");

            // 점수 및 콤보 정보 표시
            Console.WriteLine($"         FINAL SCORE :  {score}");
            Console.WriteLine($"         MAX COMBO   :  {maxCombo}");
            Console.WriteLine("\n    ------------------------------------------");

            // 랭크 표시 (색깔 입히기)
            Console.ForegroundColor = GetRankColor(rank);
            Console.WriteLine($"\n             RANK  :  [  {rank}  ]  ");
            Console.ResetColor();

            Console.WriteLine("\n    ------------------------------------------");
            Console.WriteLine("\n\n         [Enter] 키를 누르면 메뉴로 돌아갑니다.");

            // 엔터 키 대기
            while (true)
            {
                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Enter) break;
            }
        }

        // 랭크에 따라 색상 반환
        private ConsoleColor GetRankColor(string rank)
        {
            if (rank == "S") return ConsoleColor.Cyan;
            if (rank == "A") return ConsoleColor.Green;
            if (rank == "B") return ConsoleColor.Yellow;
            if (rank == "C") return ConsoleColor.Magenta;
            return ConsoleColor.Red;
        }
    }
}