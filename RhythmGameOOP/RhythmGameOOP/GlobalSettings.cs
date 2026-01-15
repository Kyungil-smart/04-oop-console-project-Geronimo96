using System;

namespace RhythmGameOOP
{
    // [설정 클래스] 게임 전체에서 공유하는 값들을 모아둔 곳
    public static class GlobalSettings
    {
        // 현재 선택된 곡의 경로 (메뉴에서 변경됨)
        public static string MusicPath = "";

        // 노래 속도 (BPM)
        public static double BPM = 120.0;

        // 싱크 조절 (초 단위): 음악 시작 후 노트가 나올 때까지 대기 시간
        public static double SyncDelay = 1.0;

        // [신규] 하드 모드 여부
        public static bool IsHardMode = false;

        // [볼륨 변수 삭제됨] 심플하게 가기 위해 삭제했습니다.

        // 4키 모드 키배치 (D F J K)
        private static readonly ConsoleKey[] Keys4 = {
            ConsoleKey.D, ConsoleKey.F, ConsoleKey.J, ConsoleKey.K
        };

        // [신규] 8키 모드 키배치 (A S D F / H J K L)
        private static readonly ConsoleKey[] Keys8 = {
            ConsoleKey.A, ConsoleKey.S, ConsoleKey.D, ConsoleKey.F,
            ConsoleKey.H, ConsoleKey.J, ConsoleKey.K, ConsoleKey.L
        };

        // 현재 모드에 맞는 키 배열을 반환 (이걸 가져다 씀)
        public static ConsoleKey[] Keys => IsHardMode ? Keys8 : Keys4;

        // 현재 모드에 맞는 레인 개수 (4개 또는 8개)
        public static int LaneCount => IsHardMode ? 8 : 4;

        // 화면 설정 (화면 잘림 방지용 높이 20)
        public const int TrackHeight = 20;
        public const int HitLine = 17;

        // 노트 낙하 속도 계산 (BPM에 비례)
        public static float NoteSpeed => (float)(BPM / 400.0);

        // [추가] 볼륨 (기본값 50)
        public static int Volume = 50;
    }
}