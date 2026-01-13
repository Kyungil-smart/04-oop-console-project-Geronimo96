using System;
using System.Runtime.InteropServices; // 윈도우 명령어 사용을 위해 필요
using System.Text;

namespace RhythmGameOOP
{
    // [오디오 클래스] 윈도우 기본 기능을 이용해 음악을 재생합니다.
    public class AudioEngine
    {
        // 윈도우의 mciSendString 명령어를 가져옵니다.
        [DllImport("winmm.dll")]
        private static extern long mciSendString(string strCommand, StringBuilder strReturn, int iReturnLength, IntPtr hwndCallback);

        // "소리 크기 바꿔줘" 라고 명령하는 함수
        public void SetVolume(int volume)
        {
            if (string.IsNullOrEmpty(currentAlias)) return;

            // 우리가 쓰는 0~100 값을 윈도우 기준 0~1000으로 변환
            int mciVolume = volume * 10;

            // 명령어 전송: setaudio 별명 volume to 숫자
            string command = $"setaudio {currentAlias} volume to {mciVolume}";
            mciSendString(command, null, 0, IntPtr.Zero);
        }

        // 현재 재생 중인 음악의 별명(ID)
        private string currentAlias;

        // 음악 재생 함수 (경로만 받음, 볼륨 파라미터 삭제)
        public void Play(string filePath)
        {
            // 기존 음악이 있다면 끕니다.
            Close();

            // 음악 별명 생성 (겹치지 않게 시간 사용)
            currentAlias = "Media" + DateTime.Now.Ticks;

            // 1. 파일 열기 (type을 명시하지 않아 mp3, wav 자동 감지)
            string openCommand = $"open \"{filePath}\" alias {currentAlias}";
            mciSendString(openCommand, null, 0, IntPtr.Zero);

            // 2. 재생 시작
            mciSendString($"play {currentAlias}", null, 0, IntPtr.Zero);
        }

        //노래의 총 길이(초 단위)를 가져오는 함수
        public double GetDuration()
        {
            if (string.IsNullOrEmpty(currentAlias)) return 0;

            StringBuilder lengthBuf = new StringBuilder(32);
            // 윈도우에게 "status 길이 알려줘"라고 명령
            mciSendString($"status {currentAlias} length", lengthBuf, 32, IntPtr.Zero);

            // 결과값(밀리초)을 숫자로 변환 후 초 단위(나누기 1000)로 반환
            if (long.TryParse(lengthBuf.ToString(), out long ms))
            {
                return ms / 1000.0;
            }
            return 0;
        }

        // 음악 정지 함수
        public void Stop()
        {
            if (!string.IsNullOrEmpty(currentAlias))
            {
                mciSendString($"stop {currentAlias}", null, 0, IntPtr.Zero);
                Close();
            }
        }

        // 자원 해제 (파일 닫기)
        private void Close()
        {
            if (!string.IsNullOrEmpty(currentAlias))
            {
                mciSendString($"close {currentAlias}", null, 0, IntPtr.Zero);
                currentAlias = null;
            }
        }
    }
}