using System;
using System.Collections.Generic;

namespace RhythmGameOOP
{

    public class NoteManager
    {
        // 화면에 존재하는 모든 노트를 담아두는 리스트
        private List<Note> notes;
        private Random rand; // 랜덤 생성을 위한 도구
   

        // 타이밍 계산 변수들
        private double lastSpawnTime; // 마지막으로 노트가 나온 시간
        private double spawnInterval; // 노트가 나오는 간격 (초 단위)

        public NoteManager()
        {
            notes = new List<Note>();
            rand = new Random();

            // 60초 / BPM = 1박자 시간
            // 예: BPM 60이면 1초마다, BPM 120이면 0.5초마다 생성
            spawnInterval = 60.0 / GlobalSettings.BPM;

        }

        // [핵심 로직 1] 노트 생성 (시간 체크)
        // [변경] 시간만 되면 대기열에서 노트를 꺼냅니다.
        public void SpawnLogic(double currentTime)
        {
            // 1. 노래 시작 후 싱크 시간 지남?
            if (currentTime > GlobalSettings.SyncDelay)
            {
                // 2. 마지막 생성 후 '간격'만큼 시간 지남?
                if (currentTime - lastSpawnTime > spawnInterval)
                {
                    // [핵심] 현재 모드(4키/8키)에 맞춰서 랜덤 레인 뽑기
                    // LaneCount가 4면 (0~3), 8이면 (0~7) 나옴
                    int lane = rand.Next(0, GlobalSettings.LaneCount);

                    notes.Add(new Note(lane));

                    lastSpawnTime = currentTime;
                }
            }
        }

        // [핵심 로직 2] 모든 노트 이동 및 삭제
        public void UpdateNotes(ScoreManager scoreManager)
        {
            // [중요] 리스트에서 요소를 삭제할 때는 '뒤에서부터' 반복문을 돌려야 안전합니다.
            // 앞에서부터 지우면 인덱스가 밀려서 에러가 나거나 건너뛰게 됩니다.
            for (int i = notes.Count - 1; i >= 0; i--)
            {
                // 노트의 Y 좌표를 속도만큼 증가 (아래로 이동)
                notes[i].Y += GlobalSettings.NoteSpeed;

                // 노트가 화면 바닥(TrackHeight)을 뚫고 지나갔다면?
                if (notes[i].Y >= GlobalSettings.TrackHeight)
                {
                    // 맞추지 못한 노트라면 콤보 초기화 (MISS 처리)
                    if (!notes[i].IsHit)
                    {
                        scoreManager.ResetCombo();// 여기서 목숨 -1
                    }
                    // 미스 처리를 했든 안 했든, 바닥 지난 노트는 무조건 삭제!
                    // 이걸 안 하면 다음 프레임에 또 미스 처리가 되어 목숨이 순삭됨
                    notes.RemoveAt(i);
                }
            }
        }

        // 렌더링(그리기) 클래스가 노트 목록을 볼 수 있게 빌려주는 함수
        // IReadOnlyList로 리턴하여 외부에서 마음대로 수정 못 하게 보호함
        public IReadOnlyList<Note> GetNotes()
        {
            return notes;
        }

        // 특정 레인(Lane)에서 판정할 수 있는 노트를 찾아주는 함수
        public Note GetClosestNote(int laneIndex)
        {
            Note target = null;
            float maxY = -1; // 가장 아래에 있는(Y값이 큰) 노트를 찾기 위함

            foreach (var note in notes)
            {
                // 키를 누른 레인과 같고 && 아직 안 맞춘 노트 중에서
                if (note.LaneIndex == laneIndex && !note.IsHit)
                {
                    // 더 아래쪽에 있는(Y가 더 큰) 노트가 있다면 타겟 갱신
                    if (note.Y > maxY)
                    {
                        maxY = note.Y;
                        target = note;
                    }
                }
            }
            return target; // 못 찾으면 null 반환
        }
    }
}