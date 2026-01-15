using System;

namespace RhythmGameOOP
{
    // static class로 만들어서 어디서든 'GlobalSettings.변수명'으로 접근하게 합니다.
    public static class GlobalSettings
    {
        // 현재 선택된 곡의 파일 경로
        public static string MusicPath = "";

        // 현재 곡의 빠르기 (BPM)
        public static double BPM = 120.0;

        // [싱크 조절] 음악 시작 후 첫 노트가 나올 때까지 기다리는 시간 (초)
        // PC 성능이나 오디오 파일 빈 공간에 따라 조절이 필요할 수 있습니다.
        public static double SyncDelay = 1.0;

        // [모드 설정] true면 8키(Hard), false면 4키(Normal)입니다.
        public static bool IsHardMode = false;

        // 4키 모드용 키 배치 (D, F, J, K)
        private static readonly ConsoleKey[] Keys4 = {
            ConsoleKey.D, ConsoleKey.F, ConsoleKey.J, ConsoleKey.K
        };

        // 8키 모드용 키 배치 (A, S, D, F / H, J, K, L)
        private static readonly ConsoleKey[] Keys8 = {
            ConsoleKey.A, ConsoleKey.S, ConsoleKey.D, ConsoleKey.F,
            ConsoleKey.H, ConsoleKey.J, ConsoleKey.K, ConsoleKey.L
        };

        // 현재 모드(IsHardMode)에 맞춰서 올바른 키 배열을 반환하는 속성(Property)
        public static ConsoleKey[] Keys => IsHardMode ? Keys8 : Keys4;

        // 현재 모드에 맞는 레인(줄) 개수 반환 (4개 또는 8개)
        public static int LaneCount => IsHardMode ? 8 : 4;

        // 화면 설정: 트랙 높이는 20칸, 판정선은 위에서 17번째 칸에 위치
        public const int TrackHeight = 20;
        public const int HitLine = 17;

        // [노트 속도 조절]
        // 값이 클수록 노트가 빨리 떨어집니다. 
        // 600.0으로 나누면 120 BPM 기준 약 2초 동안 천천히 내려옵니다.
        // (150.0으로 나누면 너무 빨라서 0.5초 만에 떨어짐)
        public static float NoteSpeed => (float)(BPM / 400.0);

        // 볼륨 값 (0 ~ 100). WAV 모드에서는 사용되지 않지만 구조상 남겨둡니다.
        public static int Volume = 50;
    }
}