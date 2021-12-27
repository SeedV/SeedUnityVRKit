// Copyright 2021-2022 The SeedV Lab.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Collections;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class InputController : MonoBehaviour {
  public enum InputMode {
    Video,
    Webcam,
  };

  [SerializeField] private InputMode _inputMode;
  [SerializeField] private RawImage _mainUI;
  // TODO: Hide this field if _inputMode isn't set to video.
  [SerializeField] private RenderTexture _videoTexture;
  // TODO: Hide this field if _inputMode isn't set to video.
  [SerializeField] private VideoClip _videoClip;
  [SerializeField] private GameObject _webcamPretab;
  [SerializeField] private GameObject _videoPretab;

  public IEnumerator Start() {
    if (_inputMode == InputMode.Video) {
      VideoPlayer videoPlayer =
          Instantiate(_videoPretab, gameObject.transform).GetComponent<VideoPlayer>();
      videoPlayer.clip = _videoClip;
      yield return new WaitUntil(() => !videoPlayer.isPrepared);
      videoPlayer.targetTexture = _videoTexture;
      videoPlayer.Play();
    } else if (_inputMode == InputMode.Webcam) {
      Instantiate(_webcamPretab, gameObject.transform);
    } else {
      Debug.LogError("Invalid InputMode.");
    }
    _mainUI.texture = _videoTexture;
  }
}
