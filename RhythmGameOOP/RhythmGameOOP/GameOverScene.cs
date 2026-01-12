using System;

namespace RhythmGameOOP
{
    public class GameOverScene
    {
        public void Show()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red; // 빨간색

            Console.WriteLine("\n\n\n\n");
            Console.WriteLine("    ##########################################");
            Console.WriteLine("    #                                        #");
            Console.WriteLine("    #           G A M E    O V E R           #");
            Console.WriteLine("    #                                        #");
            Console.WriteLine("    ##########################################");

            Console.ResetColor();
            Console.WriteLine("\n\n\n");
            Console.WriteLine("             목숨을 모두 잃었습니다...");
            Console.WriteLine("\n\n         [Enter] 키를 누르면 메뉴로 돌아갑니다.");

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    if (Console.ReadKey(true).Key == ConsoleKey.Enter) break;
                }
            }
        }
    }
}