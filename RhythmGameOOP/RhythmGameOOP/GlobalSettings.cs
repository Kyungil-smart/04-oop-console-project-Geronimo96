using System;

namespace RhythmGameOOP
{
    public static class GlobalSettings
    {
        // 현재 선택된 음악 정보 (메뉴에서 값이 바뀜)
        public static string MusicPath = "";
        public static double BPM = 120.0;

        // 싱크 조절 (초 단위)
        public static double SyncDelay = 1.0;

        // 볼륨 (0 ~ 100)
        public static int Volume = 50;

        // 키 설정 (D, F, J, K)
        public static readonly ConsoleKey[] Keys = {
            ConsoleKey.D, ConsoleKey.F, ConsoleKey.J, ConsoleKey.K
        };

        // 화면 설정 (화면 잘림 방지를 위해 20으로 수정됨)
        public const int TrackHeight = 20;
        public const int HitLine = 17;
        public const int LaneCount = 4;

        // 노트 낙하 속도
        public static float NoteSpeed => (float)(BPM / 150.0);
    }
}