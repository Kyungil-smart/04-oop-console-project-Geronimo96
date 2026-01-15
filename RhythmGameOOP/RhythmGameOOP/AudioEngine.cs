using System;
using System.Runtime.InteropServices; // 윈도우 API 사용을 위해 필요
using System.Text;

namespace RhythmGameOOP
{
    public class AudioEngine
    {
        // winmm.dll의 mciSendString 함수를 가져옵니다. (음악 재생용)
        [DllImport("winmm.dll")]
        private static extern long mciSendString(string strCommand, StringBuilder strReturn, int iReturnLength, IntPtr hwndCallback);

        // 현재 재생 중인 음악의 고유 ID (별명)
        private string currentAlias;

        // [음악 재생 함수]
        public void Play(string filePath)
        {
            // 1. 만약 이미 재생 중인 음악이 있다면 끄고 메모리를 정리합니다.
            Close();

            // 2. 음악 별명을 시간값 기반으로 생성해서 중복을 막습니다.
            currentAlias = "Media" + DateTime.Now.Ticks;

            // 3. [WAV 최적화 핵심] 'type waveaudio'를 명시하여 엽니다.
            // 이렇게 하면 WAV 파일을 가장 가볍고 빠르게 재생하며, 멈춤 현상을 방지합니다.
            string openCommand = $"open \"{filePath}\" type waveaudio alias {currentAlias}";
            mciSendString(openCommand, null, 0, IntPtr.Zero);

            // 4. 재생 명령 전송
            mciSendString($"play {currentAlias}", null, 0, IntPtr.Zero);
        }

        // [볼륨 조절 함수]
        public void SetVolume(int volume)
        {
            // [안정성 조치] .wav 파일(waveaudio 타입)은 볼륨 조절 명령(setaudio)이 불안정합니다.
            // 게임이 멈추거나 튕기는 것을 막기 위해, WAV 모드에서는 볼륨 조절을 아예 하지 않도록 막아둡니다.
            // (볼륨 조절이 꼭 필요하다면 mp3 파일을 사용해야 합니다.)
            return;
        }

        // [재생 길이 확인 함수] (초 단위 반환)
        public double GetDuration()
        {
            if (string.IsNullOrEmpty(currentAlias)) return 0;

            StringBuilder lengthBuf = new StringBuilder(32);
            // 윈도우에게 "이 음악 길이 좀 알려줘"라고 명령
            mciSendString($"status {currentAlias} length", lengthBuf, 32, IntPtr.Zero);

            // 결과값(밀리초)을 초 단위로 변환해서 반환
            if (long.TryParse(lengthBuf.ToString(), out long ms))
            {
                return ms / 1000.0;
            }
            return 0;
        }

        // [정지 함수]
        public void Stop()
        {
            if (!string.IsNullOrEmpty(currentAlias))
            {
                mciSendString($"stop {currentAlias}", null, 0, IntPtr.Zero);
                Close();
            }
        }

        // [자원 해제 함수] 파일을 닫아서 메모리 누수를 막습니다.
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