using System;
using System.Collections.Generic;
using System.IO; // 경로(Path) 처리를 위해 필수

namespace RhythmGameOOP
{
    public class MusicSelectScene
    {
        private List<Song> songs; // 곡 목록
        private int selectedIndex = 0; // 현재 선택된 메뉴 번호

        public MusicSelectScene()
        {
            songs = new List<Song>();

            // [핵심] 현재 실행 파일(.exe)이 있는 폴더의 경로를 가져옴
            string basePath = AppDomain.CurrentDomain.BaseDirectory;

            // =============================================================
            // [곡 추가 구역] Path.Combine을 써서 경로를 안전하게 합칩니다.
            // =============================================================

            // 곡 1: Songs 폴더 안의 Kalimba.wav
            string path1 = Path.Combine(basePath, "Songs", "yung-kai-blue.wav");
            songs.Add(new Song("1. yung kai - blue", path1, 97)); // 제목, 경로, BPM

            // 곡 2
            string path2 = Path.Combine(basePath, "Songs", "BSS-Fighting.wav");
            songs.Add(new Song("2. BSS - 파이팅 해야지 (Feat. 이영지)", path2, 152));

            // 필요한 만큼 계속 추가 가능......
        }

        // 메뉴 실행 함수 (사용자가 곡을 고를 때까지 무한 반복)
        public Song Run()
        {
            while (true)
            {
                Render(); // 화면 그리기

                // 키 입력 받기 (화면에 표시는 안 함: true)
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow: // 위로 이동
                        if (selectedIndex > 0) selectedIndex--;
                        break;
                    case ConsoleKey.DownArrow: // 아래로 이동
                        if (selectedIndex < songs.Count) selectedIndex++;
                        break;
                    case ConsoleKey.Enter: // 선택 완료
                        // 맨 마지막 '게임 종료'를 골랐다면 null 반환
                        if (selectedIndex == songs.Count) return null;

                        // [신규] 곡을 선택했으면 난이도 물어보기
                        Song selectedSong = songs[selectedIndex];
                        SelectDifficulty();
                        // 선택한 곡 정보 반환
                        return songs[selectedIndex];
                }
            }
        }

        // [신규] 난이도 선택 화면 함수
        private void SelectDifficulty()
        {
            Console.Clear();
            Console.WriteLine("\n\n");
            Console.WriteLine("    ==========================================");
            Console.WriteLine("           난이도를 선택하세요 (DIFFICULTY)       ");
            Console.WriteLine("    ==========================================");
            Console.WriteLine("\n\n");
            Console.WriteLine("      [1] NORMAL MODE (4 KEY) - D F J K");
            Console.WriteLine("\n");
            Console.WriteLine("      [2] HARD MODE   (8 KEY) - A S D F H J K L");
            Console.WriteLine("\n\n");
            Console.Write("      선택 >> ");

            while (true)
            {
                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.D1 || key == ConsoleKey.NumPad1)
                {
                    GlobalSettings.IsHardMode = false; // 4키
                    break;
                }
                if (key == ConsoleKey.D2 || key == ConsoleKey.NumPad2)
                {
                    GlobalSettings.IsHardMode = true; // 8키
                    break;
                }
            }
        }

        // 메뉴 화면 그리기
        private void Render()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n\n");
            Console.WriteLine("     ==========================================");
            Console.WriteLine("      R  H  Y  T  H  M    C  O  N  S  O  L  E ");
            Console.WriteLine("     ==========================================");
            Console.Write("\n\n\n");
            Console.ResetColor();

            // 곡 목록 출력
            for (int i = 0; i < songs.Count + 1; i++) // +1은 종료 버튼 때문
            {
                // 현재 선택된 항목이면 노란색 화살표 표시
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