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

        // [볼륨 변수 삭제됨] 심플하게 가기 위해 삭제했습니다.

        // 키보드 세팅 (D, F, J, K)
        public static readonly ConsoleKey[] Keys = {
            ConsoleKey.D, ConsoleKey.F, ConsoleKey.J, ConsoleKey.K
        };

        // 화면 설정 (화면 잘림 방지용 높이 20)
        public const int TrackHeight = 20;
        public const int HitLine = 17;
        public const int LaneCount = 4;

        // 노트 낙하 속도 계산 (BPM에 비례)
        public static float NoteSpeed => (float)(BPM / 150.0);
    }
}