using System;
using System.Collections.Generic;
using System.IO;

namespace RhythmGameOOP
{
    public class MusicSelectScene
    {
        private List<Song> songs;
        private int selectedIndex = 0;

        public MusicSelectScene()
        {
            songs = new List<Song>();
            string basePath = AppDomain.CurrentDomain.BaseDirectory;

            // [곡 추가] 'Songs' 폴더 안에 있는 .wav 파일 경로를 지정합니다.
            // (WAV 파일이 실제로 존재해야 합니다)

            // 1번 곡 추가
            string path1 = Path.Combine(basePath, "Songs", "yung-kai-blue.wav");
            songs.Add(new Song("1. yung kai - blue", path1, 97)); // 제목, 경로, BPM

            // 2번 곡 추가
            string path2 = Path.Combine(basePath, "Songs", "BSS-Fighting.wav");
            songs.Add(new Song("2. BSS - Fighting", path2, 152)); // 제목, 경로, BPM
        }

        // [메뉴 실행 함수]
        public Song Run()
        {
            while (true)
            {
                Render(); // 메뉴 그리기
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow: if (selectedIndex > 0) selectedIndex--; break;
                    case ConsoleKey.DownArrow: if (selectedIndex < songs.Count) selectedIndex++; break;
                    case ConsoleKey.Enter:
                        // '게임 종료' 메뉴를 선택했을 때
                        if (selectedIndex == songs.Count) return null;

                        // 곡을 선택하면 난이도 선택 화면으로 이동
                        SelectDifficulty();

                        // 선택된 곡 정보 반환
                        return songs[selectedIndex];
                }
            }
        }

        // [난이도 선택 화면] - 하드모드 설정
        private void SelectDifficulty()
        {
            Console.Clear();
            Console.WriteLine("\n\n    ==========================================");
            Console.WriteLine("           난이도를 선택하세요 (DIFFICULTY)       ");
            Console.WriteLine("    ==========================================\n\n");
            Console.WriteLine("      [1] NORMAL MODE (4 KEY) - D F J K");
            Console.WriteLine("\n");
            Console.WriteLine("      [2] HARD MODE   (8 KEY) - A S D F H J K L");
            Console.Write("\n\n      선택 >> ");

            while (true)
            {
                var key = Console.ReadKey(true).Key;
                // 1번 누르면 4키 모드
                if (key == ConsoleKey.D1 || key == ConsoleKey.NumPad1)
                {
                    GlobalSettings.IsHardMode = false;
                    break;
                }
                // 2번 누르면 8키(하드) 모드
                if (key == ConsoleKey.D2 || key == ConsoleKey.NumPad2)
                {
                    GlobalSettings.IsHardMode = true;
                    break;
                }
            }
        }

        // 메뉴 화면 출력
        private void Render()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n\n     ==========================================");
            Console.WriteLine("      R  H  Y  T  H  M    C  O  N  S  O  L  E ");
            Console.WriteLine("     ==========================================\n\n\n");
            Console.ResetColor();

            for (int i = 0; i < songs.Count + 1; i++)
            {
                // 현재 선택된 항목 강조 (노란색 화살표)
                if (i == selectedIndex)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("   ▶ ");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write("     ");
                }

                if (i < songs.Count)
                    Console.WriteLine($"{songs[i].Title} (BPM: {songs[i].BPM})");
                else
                    Console.WriteLine("게임 종료 (EXIT)");

                Console.ResetColor();
            }
        }
    }
}