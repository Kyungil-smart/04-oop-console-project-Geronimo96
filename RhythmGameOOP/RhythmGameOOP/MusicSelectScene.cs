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
            string path2 = Path.Combine(basePath, "Songs", "BSS-파이팅-해야지-_Feat.-이영지_.wav");
            songs.Add(new Song("2. BSS - 파이팅 해야지 (Feat. 이영지)", path2, 152));

            // 필요한 만큼 계속 추가 가능...
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

                    // 볼륨 조절 키 (+ 키)
                    case ConsoleKey.OemPlus:
                    case ConsoleKey.Add:
                        GlobalSettings.Volume = Math.Min(100, GlobalSettings.Volume + 5);
                        break;

                    // 볼륨 조절 키 (- 키)
                    case ConsoleKey.OemMinus:
                    case ConsoleKey.Subtract:
                        GlobalSettings.Volume = Math.Max(0, GlobalSettings.Volume - 5);
                        break;

                    case ConsoleKey.Enter: // 선택 완료
                        // 맨 마지막 '게임 종료'를 골랐다면 null 반환
                        if (selectedIndex == songs.Count) return null;

                        // 선택한 곡 정보 반환
                        return songs[selectedIndex];
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
            Console.WriteLine("          R  H  Y  T  H  M    C  O  N  S  O  L  E ");
            Console.WriteLine("     ==========================================");
            Console.ResetColor();

            // 현재 볼륨 표시
            Console.WriteLine("\n             [ 볼륨 : " + GlobalSettings.Volume.ToString().PadLeft(3) + "% ]  ( +/- 키로 조절 )\n");

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