namespace RhythmGameOOP
{
    public class Note
    {
        // 몇 번째 줄(레인)에 있는 노트인가? (0~3)
        // set; 을 private으로 막아서, 한 번 생성되면 레인을 못 바꾸게 함.
        public int LaneIndex { get; private set; }

        // 노트의 현재 세로 위치 (실수형 float를 써서 부드럽게 내려오게 함)
        public float Y { get; set; }

        // 플레이어가 이 노트를 맞췄는가? (중복 판정 방지용)
        public bool IsHit { get; set; }

        // 생성자 (Constructor): 노트를 처음 만들 때(new Note) 실행됨
        public Note(int laneIndex)
        {
            LaneIndex = laneIndex;
            Y = 0; // 화면 맨 꼭대기(0)에서 시작
            IsHit = false; // 처음엔 안 맞춘 상태
        }
    }
}